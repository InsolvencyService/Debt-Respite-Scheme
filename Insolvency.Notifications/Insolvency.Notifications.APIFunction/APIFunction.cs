using System;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Insolvency.Notifications.APIFunction
{
    public class APIFunction
    {
        public INotificationRepository NotificationRepository { get; }

        public APIFunction(INotificationRepository notificationRepository)
        {
            this.NotificationRepository = notificationRepository;
        }

        [FunctionName("API")]
        public async Task RunAsync([ServiceBusTrigger("%Topic%", "%Subscription%", Connection = "SubscriptionConnection")]string mySbMsg, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<NotificationMessage>(mySbMsg);
            var content = JsonConvert.DeserializeObject<APIContent>(message.PayLoad);

            message.MessageVersion = content.Version;
            message.MessageType = content.Message;
            message.Owner = new NotificationOwner { Email = content.Email, Id = content.Id, TemplateId = content.TemplateId };
            
            await this.NotificationRepository.StoreAsync(message, () => Task.FromResult(Guid.NewGuid().ToString()));
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
