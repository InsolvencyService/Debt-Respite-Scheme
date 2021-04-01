using System;

namespace Insolvency.Identity.Models
{
    public class OrganisationScopeRoleUserAssignedClient
    {
        public Guid OrganisationScopeId { get; set; }
        public virtual OrganisationScope OrganisationScope { get; set; }
        public Guid RoleUserAssignedClientId { get; set; }
        public virtual RoleUserAssignedClient RoleUserAssignedClient { get; set; }
    }
}
