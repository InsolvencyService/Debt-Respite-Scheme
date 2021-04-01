using Insolvency.Data.Notifications;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.GovNotify;
using Insolvency.Notifications.Letter;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify.Client;
using Notify.Interfaces;

[assembly: FunctionsStartup(typeof(Startup))]


namespace Insolvency.Notifications.Letter
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotifyGateway, NotifyGateway>();
            builder.Services.AddScoped<IAsyncNotificationClient, NotificationClient>(serviceProvider =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var apiKey = config["GovNotifyAPIKEY"];
                return new NotificationClient(apiKey);
            });

            builder.Services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var connectionString = config["PostgresConnection"];
                options.UseNpgsql(connectionString, x=>x.MigrationsAssembly("NInsolvency.Notifications.Data"));
            });
        }
    }
}
