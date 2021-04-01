using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.RoleManagement
{
    public class OrganisationRoleUsersViewModel
    {
        public OrganisationRoleUsersViewModel()
        {
            this.RoleUsers = new List<RoleUserViewModel>();
        }

        public OrganisationRoleUsersViewModel(
            Organisation organisation,
            RoleUser myRoleUser)
        {
            this.OrganisationId = organisation.Id;
            this.OrganisationName = organisation.Name;
            this.RoleUsers = organisation.RoleUsers.Select(x => new RoleUserViewModel
            {
                Id = x.Id,
                Email = x.Email,
                Role = x.Role,
                OrganisationId = x.OrganisationId,
                Scopes = string.Join(", ", x.Scopes.OrderBy(z => z.OrganisationScopeName).Select(y => y.OrganisationScopeName).ToList()),
                DisableDelete = myRoleUser != null && myRoleUser.Id.Equals(x.Id)
            })
            .ToList();
        }

        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public List<RoleUserViewModel> RoleUsers { get; set; }
    }
}
