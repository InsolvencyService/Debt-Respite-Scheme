using Insolvency.Data.Notifications;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.GovNotify;
using Insolvency.Notifications.Messaging;
using Insolvency.Notifications.StatusUpdate;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify.Client;
using Notify.Interfaces;

[assembly: FunctionsStartup(typeof(Startup))]


namespace Insolvency.Notifications.StatusUpdate
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotifyGateway, NotifyGateway>();
            builder.Services.AddScoped<IMessagingClient, MessagingClient>();

            builder.Services.AddScoped<IAsyncNotificationClient, NotificationClient>(serviceProvider =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var apiKey = config["GovNotifyAPIKEY"];
                return new NotificationClient(apiKey);
            });

            builder.Services.AddScoped<ITopicClient, TopicClient>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var serviceBusConnectionString = config["SubscriptionConnection"];
                var topicName = config["Topic"];
                return new TopicClient(serviceBusConnectionString, topicName);
            });

            builder.Services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var connectionString = config["PostgresConnection"];
                options.UseNpgsql(connectionString, x=>x.MigrationsAssembly("Insolvency.Notifications.Data"));
            });
        }
    }
}
