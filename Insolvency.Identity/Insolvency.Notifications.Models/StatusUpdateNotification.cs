namespace Insolvency.Notifications.Models
{
    public class StatusUpdateNotification
    {
        public string Id { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationType Type { get; set; }
    }
}
