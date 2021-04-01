using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Notifications.Models
{
    public class NotificationMessage
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public Sender SenderSystem { get; set; }
        public NotificationOwner Owner { get; set; }
        public NotificationStatus Status { get; set; }
        public string PayLoad { get; set; }
        public string ExternalId { get; set; }
        public string ExternalSenderSystemId { get; set; }
        [StringLength(200)]
        public string MessageType { get; set; }
        [StringLength(200)]
        public string MessageVersion { get; set; }       
    }
}
