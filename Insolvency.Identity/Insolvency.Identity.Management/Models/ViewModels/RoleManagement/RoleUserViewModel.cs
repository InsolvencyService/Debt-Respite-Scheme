using System;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.RoleManagement
{
    public class RoleUserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public RoleType Role { get; set; }
        public Guid OrganisationId { get; set; }
        public string Scopes { get; set; }
        public bool DisableDelete { get; set; }
    }
}
