using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Insolvency.Notifications.Email
{
    public class Email
    {
        public INotifyGateway NotifyGateway { get; }
        public INotificationRepository NotificationRepository { get; }

        public Email(INotificationRepository notificationRepository, INotifyGateway notifyGateway)
        {
            this.NotificationRepository = notificationRepository;
            this.NotifyGateway = notifyGateway;
        }

        [FunctionName("Email")]
        public async Task Run([ServiceBusTrigger("%Topic%", "%Subscription%", Connection = "SubscriptionConnection")]string mySbMsg, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<NotificationMessage>(mySbMsg);
            var emailContent = JsonConvert.DeserializeObject<EmailContent>(message.PayLoad);
            await this.NotificationRepository.StoreAsync(message, async () => await this.NotifyGateway.SendEmailAsync(emailContent));
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}