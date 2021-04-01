using System.Linq;
using System.Reflection;
using Insolvency.Data;
using Insolvency.Data.Notifications;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.API.Authentication;
using Insolvency.Notifications.API.Authorization;
using Insolvency.Notifications.API.OrganisationSelector;
using Insolvency.Notifications.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Insolvency.Notifications.API
{
    public class Startup
    {
        /// <summary>
        /// ///////////////////////////
        /// </summary>
        public string Version { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Version = Configuration["ApiVersion"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMessagingClient, MessagingClient>();

            services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
            {
                var identityServerStoreMigrationAsembly = typeof(ContextFactory<DbContext>).GetTypeInfo().Assembly.GetName().Name;
                var connectionString = Configuration["PostgresConnection"];
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly(identityServerStoreMigrationAsembly));
            });

            services.AddScoped<ITopicClient, TopicClient>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var serviceBusConnectionString = config["SubscriptionConnection"];
                var topicName = config["Topic"];
                return new TopicClient(serviceBusConnectionString, topicName);
            });

            services.AddControllers(config =>
                {
                    config.Filters.Add(new ProducesResponseTypeAttribute(200));
                    config.Filters.Add(new ProducesResponseTypeAttribute(401));
                    config.Filters.Add(new ProducesResponseTypeAttribute(404));
                    config.Filters.Add(new ProducesResponseTypeAttribute(500));
                })
                .AddNewtonsoftJson();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc($"v{Version}", new OpenApiInfo
                {
                    Version = $"v{Version}",
                    Title = $"v{Version} Notification API",
                    Description = $"Notification API V{Version}"
                });

                options.SwaggerDoc($"raw_v{Version}", new OpenApiInfo
                {
                    Version = $"raw_v{Version}",
                    Title = $"v{Version} Notification API",
                    Description = $"Notification API V{Version}"
                });
            });

            services.ConfigureAuthentication(opt => Configuration.GetSection("Authentication").Bind(opt));
            services.AddAuthorization(opt =>
            {
                opt.ConfigureAuthorizationPolicies();
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, OrganisationHandler>();

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var prefix = Configuration["SwaggerUrlPrefix"] ?? "";

            app.UseSwagger(x =>
                x.PreSerializeFilters.Add((y, z) =>
                {
                    if (!y.Info.Version.Contains("raw"))
                    {
                        var paths = y.Paths.ToList();
                        y.Paths.Clear();
                        paths.ForEach(path => y.Paths.Add(prefix + path.Key, path.Value));
                    }
                })
            );

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{prefix}/swagger/v{Version}/swagger.json", $"Notification API V{Version}");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseVersioningEndpoint();
            app.UseHealthChecks("/health");

            app.UseAuthentication();
            app.UseOrganisationSelector();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var initScope = app.ApplicationServices.CreateScope())
            {
                var context = initScope.ServiceProvider.GetService<ApplicationContext>();
                DbInitialise.Init(context);
            }
        }
    }
}
