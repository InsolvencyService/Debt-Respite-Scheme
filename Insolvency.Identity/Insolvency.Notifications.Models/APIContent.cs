using System;
using System.Collections.Generic;

namespace Insolvency.Notifications.Models
{
    public class APIContent
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string TemplateId { get; set; }
        public Dictionary<string, object> Personalisation { get; set; }
        public string Message { get; set; }
        public string Version { get; set; }
    }
}
