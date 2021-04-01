using System.Collections.Generic;
using System.Linq;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.RoleManagement
{
    public class AddDeveloperRoleUserViewModel : AddRoleUserBase
    {
        public List<OrganisationScope> AssignableScopes { get; set; }
        public List<string> AssignedScopes { get; set; }

        public RoleUser MapToRoleUserEntity()
        {
            return new RoleUser
            {
                Email = this.Email,
                NormalisedEmail = this.Email.Trim().ToUpper(),
                Role = RoleType.Developer,
                Scopes = AssignedScopes?.Select(x => new RoleUsersOrganisationScope 
                { 
                    OrganisationScopeName = x
                })
                .ToList()
            };
        }
    }
}
