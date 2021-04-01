using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.BSS.Notifications;
using Insolvency.Notifications.Messaging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Insolvency.Notifications.BSS.Notifications
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IMessagingClient, MessagingClient>();
            builder.Services.AddScoped<ITopicClient, TopicClient>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var serviceBusConnectionString = config["ServiceBusConnectionString"];
                var topicName = config["TopicName"];
                return new TopicClient(serviceBusConnectionString, topicName);
            });
        }
    }
}
