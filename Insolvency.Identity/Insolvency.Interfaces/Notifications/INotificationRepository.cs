using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Notifications.Models;

namespace Insolvency.Interfaces.Notifications
{
    public interface INotificationRepository
    {
        Task StoreAsync(NotificationMessage message, Func<Task<string>> inTransactionExecutionAsync);

        Task UpdateStatusForSentMessages(
            Func<Guid, IEnumerable<NotificationMessage>, Task<StatusUpdateMessage>> checkStatusAsync,
            Func<IEnumerable<StatusUpdateMessage>, Task> notifyChangesAsync);

        Task<IEnumerable<NotificationViewModel>> GetPendingAPINotificationsAsync(
            Guid ownerId,
            Func<IEnumerable<StatusUpdateMessage>, Task> notifyChangesAsync);
        Task<IEnumerable<NotificationViewModel>> GetAllNotificationsFromTimeStampAsync(
            Guid ownerId,
            DateTime timeStamp,
            Func<IEnumerable<StatusUpdateMessage>, Task> notifyChangesAsync);

        Task<int> RefreshClientData(Guid ownerId);        

        Task NotifyOwnersForPendingNotificationsAsync(Func<IEnumerable<NotificationOwner>, Task> notifyChangesAsync);
    }
}
