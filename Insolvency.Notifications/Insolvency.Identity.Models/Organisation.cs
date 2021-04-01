using System;
using System.Collections.Generic;

namespace Insolvency.Identity.Models
{
    public class Organisation
    {        
        public string Name { get; set; }
        
        public Guid Id { get; set; }

        public string ScpGroupId { get; set; }

        public string Type { get; set; }

        public string OnboarderEmail { get; set; }
        public string NormalisedOnboarderEmail { get; set; }

        public string ExternalId { get; set; }

        public virtual List<OrganisationScope> Scopes { get; set; }

        public virtual List<RoleUser> RoleUsers { get; set; }
        public virtual List<OrganisationRoleUserAssignedClient> OrganisationRoleUserAssignedClients { get; set; }
    }
}
