using System.Collections.Generic;

namespace Insolvency.Notifications.BSS.Models
{
    public class BSSStatusUpdateNotification
    {
        public IEnumerable<NotificationUpdate> notificationUpdates { get; set; }
    }
}
