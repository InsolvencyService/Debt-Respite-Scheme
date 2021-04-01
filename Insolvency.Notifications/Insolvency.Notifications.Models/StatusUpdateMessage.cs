using System;
using System.Collections.Generic;

namespace Insolvency.Notifications.Models
{
    public class StatusUpdateMessage
    {
        public Guid SenderSystemId { get; set; }
        public IEnumerable<StatusUpdateNotification> Updates { get; set; }
    }
}
