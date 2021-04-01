using Insolvency.Notifications.Models;

namespace Insolvency.Notifications.BSS.Models
{
    public class NotificationUpdate
    {
        public string notificationId { get; set; }
        public NotificationType notificationType { get; set; }
        public int notifyStatus { get; set; }
    }
}
