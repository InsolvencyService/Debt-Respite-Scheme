using System;

namespace Insolvency.Identity.Models
{
    public class OrganisationRoleUserAssignedClient
    {
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public Guid RoleUserAssignedClientId { get; set; }
        public virtual RoleUserAssignedClient RoleUserAssignedClient { get; set; }
    }
}
