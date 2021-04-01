using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Notify.Interfaces;

namespace Insolvency.Notifications.GovNotify
{
    public class NotifyGateway : INotifyGateway
    {
        public IAsyncNotificationClient Client { get; }

        public NotifyGateway(IAsyncNotificationClient client)
        {
            this.Client = client;
        }

        public async Task<string> SendEmailAsync(EmailContent emailContent)
        {
            var response = await this.Client.SendEmailAsync(emailContent.EmailAddress, emailContent.TemplateId, emailContent.Personalisation);
            return response.id;
        }

        public async Task<string> SendLetterAsync(LetterContent letterContent)
        {
            var response = await this.Client.SendLetterAsync(letterContent.TemplateId, letterContent.Personalisation);
            return response.id;
        }

        public async Task<StatusUpdateMessage> CheckStatusUpdatesAsync(Guid senderId, IEnumerable<NotificationMessage> messages)
        {
            var changedNotifications = new List<StatusUpdateNotification>();
            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message.ExternalId))
                {
                    changedNotifications.Add(new StatusUpdateNotification
                    {
                        Id = message.ExternalSenderSystemId,
                        Type = message.Type,
                        Status = NotificationStatus.Failed
                    });
                    message.Status = NotificationStatus.Failed;
                    continue;
                }
                var response = await this.Client.GetNotificationByIdAsync(message.ExternalId);
                this.UpdateMessage(message, response.status.ToLowerInvariant());
                changedNotifications.Add(new StatusUpdateNotification
                {
                    Id = message.ExternalSenderSystemId,
                    Type = message.Type,
                    Status = message.Status
                });
            }
            return new StatusUpdateMessage
            {
                SenderSystemId = senderId,
                Updates = changedNotifications
            };
        }

        protected virtual void UpdateMessage(NotificationMessage message, string responseStatus)
        {
            if (responseStatus == "delivered" || responseStatus == "received")
            {
                message.Status = NotificationStatus.Completed;
            }
            else if (message.Type == NotificationType.Letter && responseStatus.Contains("technical-failure"))
            {
                message.Status = NotificationStatus.TechnicalFailureLetter;
            }
            else if (responseStatus.Contains("permanent-failure"))
            {
                message.Status = NotificationStatus.PermanentFailure;
            }
            else if (responseStatus.Contains("temporary-failure"))
            {
                message.Status = NotificationStatus.TemporaryFailure;
            }
            else if (responseStatus.Contains("technical-failure"))
            {
                message.Status = NotificationStatus.TechnicalFailureEmail;
            }
            else if (message.Type == NotificationType.Letter && responseStatus.Contains("failure"))
            {
                message.Status = NotificationStatus.LetterFailed;
            }
            else
            {
                message.Status = NotificationStatus.Failed;
            }
            return;
        }
    }
}
