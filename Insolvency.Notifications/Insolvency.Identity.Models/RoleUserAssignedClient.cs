using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;

namespace Insolvency.Identity.Models
{
    public class RoleUserAssignedClient
    {
        public Guid Id { get; set; }
        public string CreatedByEmail { get; set; }
        public int ClientRecordId { get; set; }
        public virtual Client Client { get; set; }
        public virtual List<OrganisationRoleUserAssignedClient> OrganisationRoleUserAssignedClients { get; set; }
        public virtual List<OrganisationScopeRoleUserAssignedClient> OrganisationScopeRoleUserAssignedClients { get; set; }
    }
}
