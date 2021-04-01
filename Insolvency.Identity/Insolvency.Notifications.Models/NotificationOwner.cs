using System;

namespace Insolvency.Notifications.Models
{
    public class NotificationOwner
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string TemplateId { get; set; }
    }
}
