using System;
using System.Collections.Generic;

namespace Insolvency.Identity.Models
{
    public class OrganisationScope
    {
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public string Name { get; set; }
        public virtual List<OrganisationScopeRoleUserAssignedClient> OrganisationScopeRoleUserAssignedClients { get; set; }
    }
}