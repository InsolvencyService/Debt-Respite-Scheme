using System;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using Insolvency.Common;
using Insolvency.Interfaces;
using Insolvency.Portal.Gateways;
using Insolvency.Portal.Interfaces;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Insolvency.Portal.Authentication;
using Insolvency.Portal.Authorization;
using Insolvency.Portal.Http;
using Insolvency.Portal.Models;
using Insolvency.RestClient;
using Insolvency.RestClient.Experian;

using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Insolvency.Portal.Services.Banner;
using QAPortTypeClient = Insolvency.RestClient.Experian.QAPortTypeClient;
using Insolvency.RestClient.Experian;

namespace Insolvency.Portal
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddControllersWithViews()
                                  .AddSessionStateTempDataProvider()
                                  .AddMvcLocalization()
                                  .AddDataAnnotationsLocalization();

#if DEBUG
            if (_env.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
#endif
            var enableAdaptiveSampling = _configuration.GetValue<bool>("EnableAdaptiveSampling");
            if (!enableAdaptiveSampling)
            {
                var aiOptions = new ApplicationInsightsServiceOptions
                {
                    EnableAdaptiveSampling = false,
                    InstrumentationKey = _configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")
                };
                services.AddApplicationInsightsTelemetry(aiOptions);
            }

            var redisConnectionString = _configuration["redisServerUrl"];
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "BSS-Portal-DataProtection-Keys");

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "BSS-Portal-Session-";
            });

            var experianToken = _configuration["ExperianToken"];
            var integrationApiUrl = _configuration.GetValue<string>("IntegrationAPIUrl");
            var openIdAuthenticationConfigurationSection = _configuration.GetSection("Authentication");

            var coutryList = new CountryList();
            _configuration.Bind("CountryList", coutryList);
            services.AddSingleton(coutryList);

            var refreshTokenOptions = new RefreshTokenOptions();
            openIdAuthenticationConfigurationSection.Bind(refreshTokenOptions);
            services.AddSingleton(refreshTokenOptions);

            services.AddAuthorization(opt =>
            {
                opt.ConfigureAuthorizationPolicies();
            });
            var authenticationLifetimeFromHours = _configuration.GetValue<int>("AuthenticationLifetimeFromHours");
            services.ConfigureAuthentication(opt =>
            {
                openIdAuthenticationConfigurationSection.Bind(opt);
            }, authenticationLifetimeFromHours);

            services.AddScoped<IAuthorizationHandler, OrganisationAuthorizationHandler>();

            services.AddSingleton<CookieEvents>((serviceProvider) =>
            {
                var accessTokenExpirationThresholdMinutes = _configuration.GetValue<int>("AccessTokenExpirationThresholdMinutes");
                var accessTokenExpirationThreshold = TimeSpan.FromMinutes(accessTokenExpirationThresholdMinutes);
                return new CookieEvents(accessTokenExpirationThreshold);
            });

            services.AddScoped<BannerService>();

            // API Integration          

            services.AddHttpContextAccessor();

            services.AddScoped<IRestClientFactory, RestClientFactory>();
            services.AddScoped<IApiClient, ApiClient>(x => new OrganisationApiClient(x.GetRequiredService<IRestClientFactory>(), integrationApiUrl, x.GetRequiredService<IHttpContextAccessor>(), x.GetRequiredService<ILogger<OrganisationApiClient>>()));
            services.AddScoped<IIntegrationGateway, IntegrationGateway>();
            services.AddScoped<ICreditorServiceGateway, CreditorServiceGateway>();

            // Experian Integration
            services.AddScoped<IClientMessageInspector, TokenInjectionInspector>(x => new TokenInjectionInspector(experianToken));
            services.AddScoped<IEndpointBehavior, AuthTokenInjectorBehavior>(x => new AuthTokenInjectorBehavior(x.GetServices<IClientMessageInspector>().ToArray()));
            services.AddScoped<IQAPortTypeClient, QAPortTypeClient>(x => new QAPortTypeClient(x.GetRequiredService<IEndpointBehavior>()));
            services.AddScoped<IAddressLookupClient, AddressLookupClient>();
            services.AddScoped<IAddressLookupGateway, AddressLookupGateway>();
            //services.AddScoped<IAddressLookupGateway, MockAddressLookupGateway>();            

            services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Request.Scheme = "https";
                await next();
            });

            if (_env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            app.UseRouting();

            app.UseVersioningEndpoint();
            app.UseHealthChecks("/health");

            app.UseSession();

            var enableSessionLogging = _configuration.GetValue<bool>("EnableSessionLogging");
            if (enableSessionLogging)
            {
                app.Use(async (context, next) =>
                {
                    var sessionId = context.Session.Id;
                    var keys = context.Session.Keys;
                    var sessionKeys = keys.Count() > 0 ? string.Join(", ", keys) : "empty";
                    var logger = context.RequestServices.GetService<ILogger<Startup>>();
                    logger.LogInformation($"Incoming request with session Id: {sessionId} and session keys: {sessionKeys}.");
                    await next();
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=BreathingSpace}/{action=Index}/{id?}");
            });
        }
    }
}
