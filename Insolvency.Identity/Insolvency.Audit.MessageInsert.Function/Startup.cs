using Insolvency.Audit.MessageInsert.Function;
using Insolvency.Interfaces.Notifications;
using Insolvency.Models;
using Insolvency.Notifications.Messaging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Insolvency.Audit.MessageInsert.Function
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<IMessagingGateway, MessagingGateway>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var messagingConnection = new MessagingConnection
                {
                    ConnectionString = config["AuditSbConnection"],
                    QueueName = config["AuditSbQueueName"]
                };
                return new MessagingGateway(messagingConnection);
            });
        }
    }
}