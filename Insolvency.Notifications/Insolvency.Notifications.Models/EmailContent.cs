using System.Collections.Generic;

namespace Insolvency.Notifications.Models
{
    public class EmailContent
    {
        public string EmailAddress { get; set; }
        public string TemplateId { get; set; }
        public Dictionary<string, object> Personalisation { get; set; }
    }
}
