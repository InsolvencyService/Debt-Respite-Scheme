using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Notifications.Models;

namespace Insolvency.Interfaces.Notifications
{
    public interface IMessagingClient
    {
        Task SendStatusUpdateNotificationAsync(IEnumerable<StatusUpdateMessage> updates);
        Task SendNotificationMessageAsync(NotificationMessage notificationMessage);
    }
}
