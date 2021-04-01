using System;
using System.Collections.Generic;

namespace Insolvency.Identity.Models
{
    public class RoleUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string NormalisedEmail { get; set; }
        public RoleType Role { get; set; }
        public Guid OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual List<RoleUsersOrganisationScope> Scopes { get; set; }
    }
}
