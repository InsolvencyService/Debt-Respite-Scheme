using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Insolvency.Interfaces.Notifications;
using Insolvency.Models;

namespace Insolvency.Notifications.Messaging
{
    public class MessagingGateway : IMessagingGateway
    {
        private readonly MessagingConnection _sbConnection;

        public MessagingGateway(MessagingConnection sbConnection)
        {
            _sbConnection = sbConnection;
        }

        public async Task SendMessageAsync(byte[] message)
        {
            await using ServiceBusClient sbClient = new ServiceBusClient(_sbConnection.ConnectionString);
            ServiceBusSender sbSender = sbClient.CreateSender(_sbConnection.QueueName);
            ServiceBusMessage sbMessage = new ServiceBusMessage(message);
            await sbSender.SendMessageAsync(sbMessage);
        }
    }
}
