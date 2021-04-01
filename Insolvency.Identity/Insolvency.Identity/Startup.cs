// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using IdentityServer4.Services;
using Insolvency.Data;
using Insolvency.Data.AuditTracking;
using Insolvency.Data.IdentityManagement;
using Insolvency.Data.IdentityServer;
using Insolvency.Identity.Authentication;
using Insolvency.Identity.Configuration;
using Insolvency.Identity.Extensions;
using Insolvency.Identity.Profiles;
using Insolvency.Identity.Tokens;
using Insolvency.Interfaces;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Interfaces.IdentityServer;
using Insolvency.Models;
using Insolvency.Models.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddTransient<ITokenService, OrganisationTokenService>();

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
                options.Authentication.CookieLifetime = TimeSpan.FromHours(Constants.AuthenticationLifeTimeFromHours);
            })
            .AddConfigurationStore<IdentityManagementContext>(DataLayerConfiguration.ConfigureConfigurationStore)
            .AddOperationalStore(DataLayerConfiguration.ConfigureOperationalStore)
            .AddDeveloperSigningCredential() // not recommended for production - you need to store your key material somewhere secure         
            .ConfigureProfileServices();

            services.AddAuthentication()
                .AddScpOpenIdConnect(opt => Configuration.GetSection("ScpIdentity").Bind(opt));
            services.AddScoped(x => new Func<AuditDetail>(() => 
                x.GetRequiredService<IHttpContextAccessor>().HttpContext.GetIdentityAuditDetail()));
            services.AddScoped<IIdentityManagementRepository, IdentityManagementRepository>();
            services.AddScoped<IIdentityServerRepository, IdentityServerRepository>();
            services.AddScoped<IRestClientFactory, RestClientFactory>();
            services.AddScoped<IAuditService, AuditService>(x => new AuditService(
                x.GetService<IRestClientFactory>(),
                Configuration["AuditMessageInsertUrl"],
                x.GetService<ILogger<AuditService>>()
            ));
            services.AddScoped<IChangeTrackerAuditing, ChangeTrackerAuditing>();
            services.AddScoped<ScpAuthenticationProcessor>();

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app)
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

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseVersioningEndpoint();
            app.UseHealthChecks("/health");

            app.UseAuthentication();

            app.UseDataLayer();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
