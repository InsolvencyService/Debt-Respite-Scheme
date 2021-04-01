using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Insolvency.Data;
using Insolvency.Data.AuditTracking;
using Insolvency.Data.IdentityManagement;
using Insolvency.Data.IdentityServer;
using Insolvency.Identity.Models;
using Insolvency.Interfaces;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Interfaces.IdentityServer;
using Insolvency.Management.Authorization.Handlers;
using Insolvency.Management.Authorization.Requirements;
using Insolvency.Models;
using Insolvency.Models.Audit;
using Insolvency.Models.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Insolvency.Management
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            var authenticationLifetimeFromHours = Configuration.GetValue<int>("AuthenticationLifetimeFromHours");
            services.AddAuthentication(x =>
            {
                x.DefaultScheme = Constants.AuthSchemes.CookieAuthSckeme;
                x.DefaultChallengeScheme = Constants.AuthSchemes.OidcAuthScheme;
            })
            .AddCookie(Constants.AuthSchemes.CookieAuthSckeme, opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromHours(authenticationLifetimeFromHours);
                opt.Cookie.IsEssential = true;
                opt.SlidingExpiration = false;
            })
            .AddOpenIdConnect(Constants.AuthSchemes.OidcAuthScheme, x =>
            {
                x.Authority = Configuration.GetValue<string>("AuthorityUrl");
                x.ClientId = Configuration.GetValue<string>("ClientId");
                x.ClientSecret = Configuration.GetValue<string>("ClientSecret");
                x.ResponseType = "code";
                x.ResponseMode = "form_post";
                x.Scope.Clear();
                x.Scope.Add("openid");
                x.Scope.Add("inss");
                x.SaveTokens = true;

                x.Events.OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    context.ProtocolMessage.IdTokenHint = null;
                    return Task.CompletedTask;
                };

                x.Events.OnTicketReceived = (context) =>
                {
                    context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(authenticationLifetimeFromHours);
                    context.Properties.AllowRefresh = false;
                    context.Properties.IsPersistent = true;
                    var idTokenValue = context.Properties.GetTokenValue("id_token");
                    var idToken = new JwtSecurityToken(idTokenValue);
                    ((ClaimsIdentity)context.Principal.Identity).AddClaims(new Claim[] { new Claim(JwtClaimTypes.ClientId, idToken.Audiences.First()) });
                    return Task.CompletedTask;
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.AdministratorPolicyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RoleRequirement(new List<RoleType>() { RoleType.Administrator }));
                });

                options.AddPolicy(Constants.AnyRolePolicyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new RoleRequirement(new List<RoleType>() { RoleType.Administrator, RoleType.Developer }));
                });
            });
            services.AddTransient<IAuthorizationHandler, RoleHandler>();

            var mvcBuilder = services.AddControllersWithViews()
                                     .AddSessionStateTempDataProvider()
                                     .AddMvcLocalization()
                                     .AddDataAnnotationsLocalization();

#if DEBUG
            if (Environment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
#endif
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();
            services.AddScoped((provider) => new ConfigurationStoreOptions());
            Action<DbContextOptionsBuilder> contextOptionsBuilder = (opts) =>
                {
                    opts.UseNpgsql(Configuration.GetConnectionString("IdentityConnection"));
                };

            services.AddScoped(x => new Func<AuditDetail>(() => 
                x.GetRequiredService<IHttpContextAccessor>().HttpContext.GetAuditDetail("INSS.Identity.Management")));

            services.AddDbContext<IdentityManagementContext>(contextOptionsBuilder);
            services.AddScoped<IIdentityManagementRepository, IdentityManagementRepository>();

            services.AddDbContext<ConfigurationDbContext>(contextOptionsBuilder);
            services.AddScoped<IIdentityServerRepository, IdentityServerRepository>();
            services.AddScoped<IRestClientFactory, RestClientFactory>();
            services.AddScoped<IAuditService, AuditService>(x => new AuditService(
                x.GetService<IRestClientFactory>(), 
                Configuration["AuditMessageInsertUrl"], 
                x.GetService<ILogger<AuditService>>()
            ));
            services.AddScoped<IChangeTrackerAuditing, ChangeTrackerAuditing>();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Configuration.GetValue<bool>("DebuggingHeaderEnabled"))
            {
                app.Use(async (context, next) =>
                {
                    var allRequestHeadersToString = context.Request.Headers.Select(x => $"{x.Key}:{x.Value}").Aggregate((x, y) => $"{x}||{y}");
                    context.Response.Headers.Add("IncomingHeaders", allRequestHeadersToString);

                    // Call the next delegate/middleware in the pipeline
                    await next();
                });
            }

            if (env.IsDevelopment())
            {
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=UserProfile}/{action=Index}");
            });
        }
    }
}
