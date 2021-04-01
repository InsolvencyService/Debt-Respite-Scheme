using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Insolvency.Notifications.Messaging
{
    public class MessagingClient : IMessagingClient
    {
        public ITopicClient Client { get; }

        public MessagingClient(ITopicClient client)
        {
            this.Client = client;
        }

        public async Task SendStatusUpdateNotificationAsync(IEnumerable<StatusUpdateMessage> updates)
        {
            foreach (var update in updates)
            {
                var messageBody = JsonConvert.SerializeObject(update);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                message.UserProperties.Add("BusinessUnit", update.SenderSystemId.ToString());
                await this.Client.SendAsync(message);
            }
        }

        public async Task SendNotificationMessageAsync(NotificationMessage notificationMessage)
        {
            var messageBody = JsonConvert.SerializeObject(notificationMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            message.UserProperties.Add(notificationMessage.Type.ToMessageType());
            await this.Client.SendAsync(message);
        }
    }
}
