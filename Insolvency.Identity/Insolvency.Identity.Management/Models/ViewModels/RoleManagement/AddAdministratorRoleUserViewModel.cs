using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.RoleManagement
{
    public class AddAdministratorRoleUserViewModel : AddRoleUserBase
    {
        public RoleUser MapToRoleUserEntity()
        {
            return new RoleUser
            {
                Email = this.Email,
                NormalisedEmail = this.Email.Trim().ToUpper(),
                Role = RoleType.Administrator,
            };
        }
    }
}
