using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Identity.Models;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Management.Extensions;
using Insolvency.Management.Models.ViewModels.RoleManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Management.Controllers
{
    [Authorize(Policy = Constants.AdministratorPolicyName)]
    public class RoleManagementController : Controller
    {
        private const string _organisationId = "organisationId";

        private readonly IIdentityManagementRepository _identityManagementRepository;
        public RoleManagementController(IIdentityManagementRepository identityManagementRepository)
        {
            _identityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
        }

        public async Task<IActionResult> Organisations()
        {
            var currentRoleUserEmail = HttpContext.User.GetNormalisedEmail();
            var organisationIds = HttpContext.User.GetOrganisationIds();
            var organisations = await _identityManagementRepository.GetOrganisationsByEmailAndRoleAsync(currentRoleUserEmail, RoleType.Administrator, organisationIds);

            var model = new OrganisationsViewModel
            {
                Organisations = organisations
            };

            return View(model);
        }

        public async Task<IActionResult> OrganisationRoleUsers(Guid organisationId)
        {
            HttpContext.Session.SetString(_organisationId, organisationId.ToString());
            var currentRoleUserEmail = HttpContext.User.GetNormalisedEmail();
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var organisation = await _identityManagementRepository
                .GetOrganisationAndRoleUsersAndScopesByOrganisationIdAsync(organisationId, authorisedOrgIds);
            var myRoleUser = organisation.RoleUsers
                            .FirstOrDefault(x => x.NormalisedEmail.Equals(currentRoleUserEmail) &&
                                                 x.Role == RoleType.Administrator);
            var model = new OrganisationRoleUsersViewModel(organisation, myRoleUser);

            return View(model);
        }

        public async Task<IActionResult> AddDeveloperRoleUser()
        {

            var viewModel = new AddDeveloperRoleUserViewModel()
            {
                AssignableScopes = await GetOrganisationScopes()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDeveloperRoleUser(AddDeveloperRoleUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AssignableScopes = await GetOrganisationScopes();
                return View(viewModel);
            }

            var organisationId = new Guid(HttpContext.Session.GetString(_organisationId));
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var newRoleUser = viewModel.MapToRoleUserEntity();
            newRoleUser.OrganisationId = organisationId;
            var addResult = await _identityManagementRepository.AddRoleUserAsync(newRoleUser, authorisedOrgIds);

            if (addResult.IsError)
            {
                viewModel.AssignableScopes = await GetOrganisationScopes();
                ModelState.AddModelError(nameof(viewModel.Email), addResult.OperationError.Message);
                return View(viewModel);
            }

            return RedirectToAction(nameof(OrganisationRoleUsers), new { organisationId = organisationId });
        }

        public async Task<IActionResult> AddAdministratorRoleUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdministratorRoleUser(AddAdministratorRoleUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var organisationId = new Guid(HttpContext.Session.GetString(_organisationId));
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var newRoleUser = viewModel.MapToRoleUserEntity();
            newRoleUser.OrganisationId = organisationId;
            var addResult = await _identityManagementRepository.AddRoleUserAsync(newRoleUser, authorisedOrgIds);

            if (addResult.IsError)
            {
                ModelState.AddModelError(nameof(viewModel.Email), addResult.OperationError.Message);
                return View(viewModel);
            }

            return RedirectToAction(nameof(OrganisationRoleUsers), new { organisationId = organisationId });
        }

        public async Task<IActionResult> EditRoleUser(Guid id)
        {
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var roleUser = await _identityManagementRepository.FindRoleUserByIdAsync(id, authorisedOrgIds);

            var viewModel = new EditRoleUserViewModel
            {
                Id = id,
                Email = roleUser.Email,
                Role = roleUser.Role
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoleUser(EditRoleUserViewModel viewModel)
        {
            var roleUser = viewModel.MapToRoleUserEntity();
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var organisationId = new Guid(HttpContext.Session.GetString(_organisationId));
            roleUser.OrganisationId = organisationId;

            var emailAlreadyInUseResult = await _identityManagementRepository.IsEmailInUseAsync(roleUser, authorisedOrgIds);

            if (emailAlreadyInUseResult.Value)
            {
                ModelState.AddModelError("Email", emailAlreadyInUseResult.OperationError.Message);
                return View(viewModel);
            }

            var updatingRoleUser = viewModel.MapToRoleUserEntity();
            var updateResult = await _identityManagementRepository.UpdateRoleUserAsync(updatingRoleUser, authorisedOrgIds);

            if (updateResult.IsError)
            {
                ModelState.AddModelError(string.Empty, updateResult.OperationError.Message);
                return View(viewModel);
            }

            return RedirectToAction(nameof(OrganisationRoleUsers), new { organisationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleUser(Guid id)
        {
            var currentRoleUserEmail = HttpContext.User.GetNormalisedEmail();
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var currentRoleUser = await _identityManagementRepository.GetAdministratorRoleUserByEmailAsync(currentRoleUserEmail, authorisedOrgIds);
            var organisationId = new Guid(HttpContext.Session.GetString(_organisationId));

            if(!currentRoleUser.Any(x => x.Id == id))
            {
                var deleteResult = await _identityManagementRepository.DeleteRoleUserByIdAsync(id, authorisedOrgIds);

                if (deleteResult.IsError)
                {
                    return RedirectToAction(nameof(OrganisationRoleUsers), new { organisationId });
                }
            }

            return RedirectToAction(nameof(OrganisationRoleUsers), new { organisationId });
        }

        private async Task<List<OrganisationScope>> GetOrganisationScopes()
        {
            var organisationId = new Guid(HttpContext.Session.GetString(_organisationId));
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            return await _identityManagementRepository
                                        .GetOrganisationScopesByOrganisationIdAsync(organisationId, authorisedOrgIds);
        }
    }
}
