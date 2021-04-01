using System;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Identity.Models;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Management.Extensions;
using Insolvency.Management.Models.ViewModels.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Management.Controllers
{
    [Authorize(Policy = Constants.AnyRolePolicyName)]
    public class UserProfileController : Controller
    {
        private readonly IIdentityManagementRepository _identityManagementRepository;

        public UserProfileController(IIdentityManagementRepository identityManagementRepository)
        {
            _identityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
        }

        public async Task<IActionResult> Index()
        {
            var thisRoleUserName = HttpContext.User.GetName();
            var thisRoleUserEmail = HttpContext.User.GetNormalisedEmail();
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var manageGroupUrl = HttpContext.User.GetManageGroupProfileUrl();
            var manageProfileUrl = HttpContext.User.GetManageProfileUrl();

            var associatedOrgs = await _identityManagementRepository.GetRoleUsersOrganisationDetailsByEmailAsync(thisRoleUserEmail, authorisedOrgIds);
            var isAdmin = associatedOrgs.Any(x => x.RoleUsers.Any(y => y.Role == RoleType.Administrator));

            var model = new UserProfileViewModel 
            {
                Name = thisRoleUserName,
                Email = thisRoleUserEmail,
                Organisations = associatedOrgs,
                ManageGroupProfileUrl = manageGroupUrl,
                ManageProfileUrl = manageProfileUrl,
                IsAdmin = isAdmin
            };
         
            return View(model);
        }
    }
}
