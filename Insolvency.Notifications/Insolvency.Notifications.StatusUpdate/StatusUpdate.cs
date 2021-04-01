using System;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Insolvency.Notifications.StatusUpdate
{
    public class StatusUpdate
    {
        public IMessagingClient Client { get; }
        public INotifyGateway NotifyGateway { get; }
        public INotificationRepository NotificationRepository { get; }

        public StatusUpdate(INotificationRepository notificationRepository, INotifyGateway notifyGateway, IMessagingClient client)
        {
            this.NotificationRepository = notificationRepository;
            this.NotifyGateway = notifyGateway;
            this.Client = client;
        }

        [FunctionName("StatusUpdate")]
        public async Task Run([TimerTrigger("%CronPattern%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await this.NotificationRepository
                .UpdateStatusForSentMessages(this.NotifyGateway.CheckStatusUpdatesAsync, Client.SendStatusUpdateNotificationAsync);
        }
    }
}
