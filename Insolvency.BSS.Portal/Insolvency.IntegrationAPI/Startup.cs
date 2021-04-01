using System;
using System.Linq;
using Insolvency.Common;
using Insolvency.Integration.Gateways;
using Insolvency.Integration.Gateways.Audit;
using Insolvency.Integration.Gateways.Mapper;
using Insolvency.Integration.Gateways.OData;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.IntegrationAPI.Authorization;
using Insolvency.IntegrationAPI.Extensions;
using Insolvency.IntegrationAPI.HostedService;
using Insolvency.IntegrationAPI.Middlewares;
using Insolvency.IntegrationAPI.ODataBeforeRequestFunctions;
using Insolvency.Interfaces;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Insolvency.RestClient;
using Insolvency.RestClient.ODataBeforeRequestFunctions;
using Simple.OData.Client;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Insolvency.IntegrationAPI
{
    public class Startup
    {
        public static bool ServerStarted { get; set; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dynamicsOptions = new DynamicsGatewayOptions();
            Configuration.Bind("DynamicsGateway", dynamicsOptions);
            services.AddSingleton(dynamicsOptions);

            ConfigureMvc(services);

            ConfigureAuthentication(services);

            var enableAdaptiveSampling = Configuration.GetValue<bool>("EnableAdaptiveSampling");
            if (!enableAdaptiveSampling)
            {
                var aiOptions = new ApplicationInsightsServiceOptions
                {
                    EnableAdaptiveSampling = false,
                    InstrumentationKey = Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")
                };
                services.AddApplicationInsightsTelemetry(aiOptions);
            }

            var redisConnectionString = Configuration["redisServerUrl"];
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, $"{Constants.IntegrationAPICacheKey}DataProtection-Keys");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = Constants.IntegrationAPICacheKey;
            });

            var listLifetimeInDays = Configuration.GetValue<int>("CMPListCacheLifeTimeInDays");
            services.AddScoped<ICacheClient, CacheClient>(x => new CacheClient(x.GetService<IDistributedCache>(), TimeSpan.FromDays(listLifetimeInDays)));

            services.AddAuthorization(opt =>
            {
                opt.ConfigureAuthorizationPolicies();
            });

            services.AddScoped<IAuthorizationHandler, OrganisationAuthorizationHandler>();

            services.AddSingleton<IMapperService, MapperService>();

            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                {
                    var validationProblems = new ValidationProblemDetails(actionContext.ModelState);

                    return new ObjectResult(validationProblems)
                    {
                        StatusCode = 422
                    };
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                        return false;

                    var groups = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<SwaggerGroupAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToList();

                    groups.AddRange(methodInfo.GetCustomAttributes(true)
                        .OfType<SwaggerGroupAttribute>()
                        .SelectMany(attr => attr.Versions));

                    return groups.Any(v => v.ToString() == docName);
                });
                c.DocumentFilter<SwaggerAddEnumDescriptions>();
                c.OperationFilter<SwaggerParameterIgnore>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "INSS", Version = "v1" });
                c.SwaggerDoc("raw_v1", new OpenApiInfo { Title = "INSS", Version = "raw_v1" });
                c.SwaggerDoc("ma_v1", new OpenApiInfo { Title = "INSS", Version = "ma_v1" });
                c.SwaggerDoc("raw_ma_v1", new OpenApiInfo { Title = "INSS", Version = "raw_ma_v1" });
                c.SwaggerDoc("cr_v1", new OpenApiInfo { Title = "INSS", Version = "cr_v1" });
                c.SwaggerDoc("raw_cr_v1", new OpenApiInfo { Title = "INSS", Version = "raw_cr_v1" });
            });

            services.AddScoped<IODataBeforeRequestFunction>(x => new ODataMessageAuthenticatorFunction(
                new AuthorityDetails
                {
                    ClientId = Configuration.GetValue<string>("ClientId"),
                    ClientSecret = Configuration.GetValue<string>("ClientSecret"),
                    ClientUrl = Configuration.GetValue<string>("AuthorityUrl"),
                    ResourceUrl = Configuration.GetValue<string>("ClientResource")
                }, x.GetService<ICacheClient>()));
            services.AddScoped<IODataBeforeRequestFunction, ODataMaxPageSizePreferenceFunction>();

            services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext.GetAuditContext());
            services.AddScoped(x => GetODataClientSettings(x.GetRequiredService<AuditContext>, x.GetServices<IODataBeforeRequestFunction>().ToArray()));
            services.AddScoped<IODataClient, ODataClient>(x => new ODataClient(x.GetRequiredService<ODataClientSettings>()));

            services.AddSingleton<IRestClientFactory, RestClientFactory>();
            services.AddScoped<IMoneyAdviserServiceDynamicsGateway, MoneyAdviserServiceDynamicsGateway>();
            services.AddScoped<ICreditorServiceDynamicsGateway, CreditorServiceDynamicsGateway>();
            services.AddScoped<ICommonDynamicsGateway, CommonDynamicsGateway>();
            services.AddScoped<IDebtorSearchGateway, DebtorSearchGateway>();
            services.AddScoped<IBreathingSpaceBrowseGateway, BreathingSpaceBrowseGateway>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddHostedService<QueuedAuditingHostedService>(x => new QueuedAuditingHostedService(
                x.GetService<IRestClientFactory>(),
                Configuration["AuditMessageInsertUrl"],
                x.GetService<ILogger<QueuedAuditingHostedService>>(),
                x.GetService<IBackgroundAuditTaskQueue>()));
            services.AddSingleton<IBackgroundAuditTaskQueue, BackgroundTaskQueue>();
            
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddHealthChecks();
        }

        protected virtual IMvcBuilder ConfigureMvc(IServiceCollection services)
        {
            return services.AddControllers(config =>
            {
                config.Filters.Add(new ExceptionHandling(Configuration.GetValue<bool>("EnableErrorMessageResponse")));
                config.Filters.Add(new ProducesResponseTypeAttribute(200));
                config.Filters.Add(new ProducesResponseTypeAttribute(400));
                config.Filters.Add(new ProducesResponseTypeAttribute(401));
                config.Filters.Add(new ProducesResponseTypeAttribute(403));
                config.Filters.Add(new ProducesResponseTypeAttribute(404));
                config.Filters.Add(new ProducesResponseTypeAttribute(409));
                config.Filters.Add(new ProducesResponseTypeAttribute(422));
                config.Filters.Add(new ProducesResponseTypeAttribute(500));
            })
               .AddMvcLocalization()
               .AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseVersioningEndpoint();
            app.UseHealthChecks("/health");

            var prefix = Configuration["SwaggerUrlPrefix"];

            app.UseSwagger(x =>
            {
                x.PreSerializeFilters.Add((y, z) =>
                {
                    if (!y.Info.Version.Contains("raw"))
                    {
                        var paths = y.Paths.ToList();
                        y.Paths.Clear();
                        paths.ForEach(path => y.Paths.Add(prefix + path.Key, path.Value));
                    }
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(prefix + "/swagger/v1/swagger.json", "All API V1");
                c.SwaggerEndpoint(prefix + "/swagger/ma_v1/swagger.json", "Money Adviser API V1");
                c.SwaggerEndpoint(prefix + "/swagger/cr_v1/swagger.json", "Creditor API V1");
            });

            app.UseAuthentication();

            app.UseOrganisationSelector();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ServerStarted = true;
        }

        public ODataClientSettings GetODataClientSettings(Func<AuditContext> auditContextProvider, params IODataBeforeRequestFunction[] odataBeforeRequestFunctions)
        {
            var dynamicsUrl = Configuration.GetValue<string>("DynamicsUrl");

            var clientSettings = new ODataClientSettings
            {
                BaseUri = new Uri(dynamicsUrl),
                BeforeRequestAsync = async message =>
                {
                    foreach (var func in odataBeforeRequestFunctions)
                    {
                        await func.BeforeRequestAsync(message);
                    }
                },
                PayloadFormat = ODataPayloadFormat.Json
            };
            clientSettings.AdapterFactory = new CustomCreteAdapterLoaderFactory(
                (x, y) => new CompositeMetadataODataAdapter(x, y,
                    z => new WrapperMedataOmitNullValues(z),
                    z => new WrapperAuditMedataValues(z, auditContextProvider)));
            return clientSettings;
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services) => services.ConfigureOpenIdConnect(opt => Configuration.GetSection("Authentication").Bind(opt));
    }
}
