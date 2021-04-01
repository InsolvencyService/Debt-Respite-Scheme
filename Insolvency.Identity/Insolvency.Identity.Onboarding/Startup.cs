using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Insolvency.Data;
using Insolvency.Data.AuditTracking;
using Insolvency.Data.IdentityManagement;
using Insolvency.Data.IdentityServer;
using Insolvency.Interfaces;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Interfaces.IdentityServer;
using Insolvency.Models;
using Insolvency.Models.Audit;
using Insolvency.Models.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Insolvency.Identity.Onboarding
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IIdentityManagementRepository, IdentityManagementRepository>();
            services.AddScoped<IIdentityServerRepository, IdentityServerRepository>();
            services.AddHttpContextAccessor();
            services.AddScoped(x => new Func<AuditDetail>(() => 
                x.GetRequiredService<IHttpContextAccessor>().HttpContext.GetAuditDetail("INSS.Identity.Onboarding")));
            services.AddScoped<IRestClientFactory, RestClientFactory>();
            services.AddScoped<IAuditService, AuditService>(x => new AuditService(
                x.GetService<IRestClientFactory>(), 
                Configuration["AuditMessageInsertUrl"], 
                x.GetService<ILogger<AuditService>>())
            );
            services.AddScoped<IChangeTrackerAuditing, ChangeTrackerAuditing>();
            services.AddScoped((provider) => new ConfigurationStoreOptions());
            Action<DbContextOptionsBuilder> contextOptionsBuilder = (opts) =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("IdentityConnection"));
            };

            services.AddDbContext<IdentityManagementContext>(contextOptionsBuilder);
            services.AddDbContext<ConfigurationDbContext>(contextOptionsBuilder);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "INSS", Version = "v1" });
            });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseVersioningEndpoint();
            app.UseHealthChecks("/health");

            app.UseAuthorization();

            var swaggerUrlPrefix = Configuration["SwaggerUrlPrefix"];
            app.UseSwagger(swaggerOptions =>
            {
                swaggerOptions.PreSerializeFilters.Add((openApiDocument, httpRequest) =>
                {
                    var paths = openApiDocument.Paths.ToList();
                    openApiDocument.Paths.Clear();
                    paths.ForEach(path => openApiDocument.Paths.Add(swaggerUrlPrefix + path.Key, path.Value));

                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerUrlPrefix + "/swagger/v1/swagger.json", "Identity Onboarding V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
