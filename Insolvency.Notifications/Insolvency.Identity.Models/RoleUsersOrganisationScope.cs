using System;

namespace Insolvency.Identity.Models
{
    public class RoleUsersOrganisationScope
    {
        public Guid Id { get; set; }
        public Guid RoleUserId { get; set; }
        public string OrganisationScopeName { get; set; }
        public virtual RoleUser RoleUser { get; set; }

    }
}
