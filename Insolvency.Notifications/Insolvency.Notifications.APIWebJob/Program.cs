using System.Threading.Tasks;
using Insolvency.Data.Notifications;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Messaging;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Insolvency.Notifications.APIWebJob
{
    class Program
    {
        static async Task Main()
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
                b.AddTimers();

                b.Services.AddScoped<INotificationRepository, NotificationRepository>();
                b.Services.AddScoped<IMessagingClient, MessagingClient>();
                b.Services.AddScoped<ITopicClient, TopicClient>(x =>
                {
                    var config = x.GetService<IConfiguration>();
                    var serviceBusConnectionString = config["ServiceBusConnectionString"];
                    var topicName = config["TopicName"];
                    return new TopicClient(serviceBusConnectionString, topicName);
                });
                b.Services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
                {
                    var config = serviceProvider.GetService<IConfiguration>();
                    var connectionString = config["PostgresConnection"];
                    options.UseNpgsql(connectionString, x => x.MigrationsAssembly("Insolvency.Notifications.Data"));
                });
            });

            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });
            
            var host = builder.Build();
            using (host)
            {
                var jobHost = host.Services.GetService<IJobHost>();

                await host.StartAsync();
                await jobHost.CallAsync("SendEmailNotificationsToOwners");
                await host.StopAsync();
            }
        }
    }
}
