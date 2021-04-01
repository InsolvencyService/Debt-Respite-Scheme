using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insolvency.Models.Audit
{
    public class AuditMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string ClientId { get; set; }
        public string OrganisationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string SenderName { get; set; }
        public string PayLoad { get; set; }
    }
}