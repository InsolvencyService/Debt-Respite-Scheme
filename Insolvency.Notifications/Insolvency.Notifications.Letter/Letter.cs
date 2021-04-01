using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Insolvency.Notifications.Letter
{
    public class Letter
    {
        public INotifyGateway NotifyGateway { get; }
        public INotificationRepository NotificationRepository { get; }

        public Letter(INotificationRepository notificationRepository, INotifyGateway notifyGateway)
        {
            this.NotificationRepository = notificationRepository;
            this.NotifyGateway = notifyGateway;
        }

        [FunctionName("Letter")]
        public async Task Run([ServiceBusTrigger("%Topic%", "%Subscription%", Connection = "SubscriptionConnection")]string mySbMsg, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<NotificationMessage>(mySbMsg);
            var letterContent = JsonConvert.DeserializeObject<LetterContent>(message.PayLoad);
            await this.NotificationRepository.StoreAsync(message, async () => await this.NotifyGateway.SendLetterAsync(letterContent));
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}