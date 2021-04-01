using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Notifications.Models;

namespace Insolvency.Interfaces.Notifications
{
    public interface INotifyGateway
    {
        Task<string> SendEmailAsync(EmailContent emailContent);
        Task<string> SendLetterAsync(LetterContent letterContent);
        Task<StatusUpdateMessage> CheckStatusUpdatesAsync(Guid senderId, IEnumerable<NotificationMessage> messages);
    }
}
