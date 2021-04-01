using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Insolvency.Notifications.APIWebJob
{
    public class Function
    {
        public ILogger<Function> Logger { get; }
        public IMessagingClient Client { get; }
        public INotificationRepository Repository { get; }

        public Function(IMessagingClient client, INotificationRepository notificationRepository, ILogger<Function> logger)
        {
            this.Client = client;
            this.Logger = logger;
            this.Repository = notificationRepository;
        }

        [NoAutomaticTrigger]
        [FunctionName("SendEmailNotificationsToOwners")]
        public async Task RunAsync()
        {
            Logger.LogInformation($"C# Timer trigger web job executed at: {DateTime.Now}");

            await this.Repository.NotifyOwnersForPendingNotificationsAsync(SendNotificationToOwnersAsync);
        }

        protected async virtual Task SendNotificationToOwnersAsync(IEnumerable<NotificationOwner> owners)
        {
            foreach (var owner in owners)
            {
                await this.Client.SendNotificationMessageAsync(CreateNotificationMessage(owner));
            }
        }

        protected virtual NotificationMessage CreateNotificationMessage(NotificationOwner owner)
        {
            var emailContent = new EmailContent
            {
                EmailAddress = owner.Email,
                TemplateId = owner.TemplateId
            };

            var notificationMessage = new NotificationMessage
            {
                CreatedOn = DateTime.UtcNow,
                Id = Guid.Empty,
                SenderSystem = new Sender
                {
                    Id = new Guid("D033125A-26DD-4A52-A378-14531CDB4410")
                },
                ExternalSenderSystemId = Guid.Empty.ToString(), // No-one listens, Ian to confirm
                Status = NotificationStatus.Transit,
                Type = NotificationType.Email,
                PayLoad = JsonConvert.SerializeObject(emailContent)
            };
            return notificationMessage;
        }
    }
}
