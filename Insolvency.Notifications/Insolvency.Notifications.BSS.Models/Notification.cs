using System;
using System.Collections.Generic;
using Insolvency.Notifications.Models;

namespace Insolvency.Notifications.BSS.Models
{
    public class Notification
    {
        public Guid BusinessUnit { get; set; }
        public NotificationType Channel { get; set; }
        public Guid OriginatingNotification { get; set; }
        public Dictionary<string, object> Content { get; set; }
    }
}
