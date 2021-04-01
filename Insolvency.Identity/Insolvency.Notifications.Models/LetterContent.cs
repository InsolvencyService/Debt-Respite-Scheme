using System.Collections.Generic;

namespace Insolvency.Notifications.Models
{
    public class LetterContent
    {
        public string TemplateId { get; set; }
        public Dictionary<string, object> Personalisation { get; set; }
    }
}
