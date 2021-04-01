
using System.Collections.Generic;

namespace Insolvency.Integration.Interfaces.Models
{
    public class AuditDetail
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string OrganisationId { get; set; }
        public string SenderName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public IDictionary<string, object> Content { get; set; }
    }
}
