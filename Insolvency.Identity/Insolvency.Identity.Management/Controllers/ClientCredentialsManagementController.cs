using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using IdentityServer4.Models;
using Insolvency.Identity.Models;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Interfaces.IdentityServer;
using Insolvency.Management.Extensions;
using Insolvency.Management.Models;
using Insolvency.Management.Models.ViewModels.ClientCredentials;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Management.Controllers
{
    [Authorize(Policy = Constants.AnyRolePolicyName)]
    public class ClientCredentialsManagementController : Controller
    {
        private const string _roleUserEmail = "roleUserEmail";
        private const string _selectedOrganisationIds = "selectedOrganisationIds";
        private const string _clientCredentials = "clientCredentials";
        private const string _selectedScopes = "selectedScopes";
        private const string _addClientSecretResultViewModel = "addClientSecretResultViewModel";

        private readonly IIdentityManagementRepository _identityManagementRepository;
        private readonly IIdentityServerRepository _identityServerRepository;
        public ClientCredentialsManagementController(
            IIdentityManagementRepository identityManagementRepository,
            IIdentityServerRepository identityServerRepository)
        {
            _identityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
            _identityServerRepository = identityServerRepository ?? throw new ArgumentNullException(nameof(identityServerRepository));
        }
        public async Task<IActionResult> ClientCredentials()
        {
            if (TempData.ContainsKey(_clientCredentials))
            {
                TempData.Remove(_clientCredentials);
            }
            
            var currentRoleUserEmail = HttpContext.User.GetNormalisedEmail();
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            HttpContext.Session.SetString(_roleUserEmail, currentRoleUserEmail);

            var clients = await _identityManagementRepository.GetAuthorisedClientsByEmailAsync(currentRoleUserEmail, authorisedOrgIds);
            var isDeveloper = await _identityManagementRepository.CheckHasDeveloperRoleUserByEmailAsync(currentRoleUserEmail, authorisedOrgIds);
            var model = new ClientCredentialsViewModel()
            {
                AbleToAddClient = isDeveloper,
                CurrentRoleUserEmail = currentRoleUserEmail,
                Clients = clients.Select(x => new ClientViewModel(x.Id, x.ClientName))
                                 .ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> AvailableOrganisations()
        {
            var model = new AvailableOrganisationsViewModel()
            {
                SelectableOrganisation = await GetRoleUsersOrganisations(
                    HttpContext.Session.GetString(_roleUserEmail))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AvailableOrganisations(AvailableOrganisationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SelectableOrganisation = await GetRoleUsersOrganisations(
                    HttpContext.Session.GetString(_roleUserEmail));
                return View(model);
            }

            TempData[_selectedOrganisationIds] = JsonSerializer.Serialize(model.SelectedOrganisationIds);
            return RedirectToAction(nameof(AvailableScopes));
        }

        public async Task<IActionResult> AvailableScopes()
        {
            var organisationIds = JsonSerializer.Deserialize<List<Guid>>(
                TempData.Peek(_selectedOrganisationIds)
                .ToString());
            var currentRoleUserEmail = HttpContext.Session.GetString(_roleUserEmail);
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var model = new AvailableScopesViewModel()
            {
                AvailableScopes = await _identityManagementRepository
                .GetAccessibleOrganisationScopesAsync(organisationIds, currentRoleUserEmail, authorisedOrgIds)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AvailableScopes(AvailableScopesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var selectedOrganisationIds = JsonSerializer.Deserialize<List<Guid>>(
                                                                  TempData.Peek(_selectedOrganisationIds)
                                                                  .ToString());
                var currentRoleUserEmail = HttpContext.Session.GetString(_roleUserEmail);
                var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
                model.AvailableScopes = await _identityManagementRepository
                                        .GetAccessibleOrganisationScopesAsync(selectedOrganisationIds, currentRoleUserEmail, authorisedOrgIds);
                return View(model);
            }

            TempData[_selectedScopes] = JsonSerializer.Serialize(model.SelectedScopes);

            return RedirectToAction(nameof(CreateClientCredentials));
        }

        public async Task<IActionResult> CreateClientCredentials()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClientCredentials(CreateClientCredentialsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!TempData.ContainsKey(_selectedScopes))
            {
                return RedirectToAction(nameof(ClientCredentials));
            }

            var selectedScopes = JsonSerializer.Deserialize<List<string>>(
                TempData.Peek(_selectedScopes)
                .ToString());

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var newClient = CreateBasicClientEntity(model.Name, selectedScopes);
            var createResult = await _identityServerRepository.CreateIdentityClientAsync(newClient);
            if (createResult.IsError)
            {
                scope.Dispose();
                ModelState.AddModelError(string.Empty, createResult.OperationError.Message);
                return View(model);
            }

            var selectedOrganisationIds = JsonSerializer.Deserialize<List<Guid>>(
                          TempData.Peek(_selectedOrganisationIds)
                          .ToString());
            var currentRoleUserEmail = HttpContext.Session.GetString(_roleUserEmail);
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var clientDbId = createResult.Value.Id;
            var linkResult = await _identityManagementRepository.LinkRoleUserToClientAsync(
                  clientDbId,
                  currentRoleUserEmail,
                  selectedOrganisationIds,
                  selectedScopes,
                  authorisedOrgIds);

            if (linkResult.IsError)
            {
                scope.Dispose();
                ModelState.AddModelError(string.Empty, createResult.OperationError.Message);
                return View(model);
            }

            TempData.Remove(_selectedScopes);
            TempData.Remove(_selectedOrganisationIds);
            var clientCredentials = new ClientCredentials(createResult.Value.ClientId, createResult.Value.Secret);
            TempData[_clientCredentials] = JsonSerializer.Serialize(clientCredentials);
            scope.Complete();
            return RedirectToAction(nameof(CreateClientCredentialsResult));
        }

        public async Task<IActionResult> CreateClientCredentialsResult()
        {
            var clientCredentials = JsonSerializer.Deserialize<ClientCredentials>(
                TempData.Peek(_clientCredentials)
                .ToString());

            var model = new CreateClientCredentialsResultViewModel()
            {
                ClientId = clientCredentials.ClientId,
                Secret = clientCredentials.Secret
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClientCredentials(int id)
        {
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var currentRoleUserEmail = HttpContext.Session.GetString(_roleUserEmail);
            var deleteResult = await _identityManagementRepository.DeleteClientCredentialsByClientRecordIdAsync(id, currentRoleUserEmail, authorisedOrgIds);

            if (deleteResult.IsError)
            {
                return RedirectToAction(nameof(ClientCredentials));
            }

            return RedirectToAction(nameof(ClientCredentials));
        }

        public async Task<IActionResult> ManageClientCredentials(int id)
        {
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var currentRoleUserEmail = HttpContext.Session.GetString(_roleUserEmail);
            var extendedClient = await _identityManagementRepository.GetRoleUserAssignedClientByClientRecordIdAsync(id, currentRoleUserEmail, authorisedOrgIds);
            var model = new ManageClientCredentialsViewModel()
            {
                Client = new ClientViewModel(extendedClient, currentRoleUserEmail),
                ExtendedClient = extendedClient,
                Scopes = extendedClient.OrganisationScopeRoleUserAssignedClients
                         .Select(x => x.OrganisationScope)
                         .GroupBy(x => x.Name)
                         .OrderBy(x => x.Key)
                         .ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> AddClientSecret(int id)
        {
            var model = new AddClientSecretViewModel()
            {
                Id = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClientSecret(AddClientSecretViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var addResult = await _identityServerRepository.AddSecretToClient(model.Id, model.Description, authorisedOrgIds);

            if (addResult.IsError)
            {
                return RedirectToAction(nameof(ClientCredentials));
            }

            var addClientSecretResultViewModel = new AddClientSecretResultViewModel()
            {
                InternalClientId = addResult.Value.InternalClientId,
                ClientId = addResult.Value.ClientId,
                Secret = addResult.Value.Secret,
                Description = addResult.Value.Description
            };

            TempData[_addClientSecretResultViewModel] = JsonSerializer.Serialize(addClientSecretResultViewModel);
            return RedirectToAction(nameof(AddClientSecretResult));
        }

        public async Task<IActionResult> AddClientSecretResult()
        {
            var viewModel = JsonSerializer.Deserialize<AddClientSecretResultViewModel>(
                TempData.Peek(_addClientSecretResultViewModel)
                .ToString());

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClientSecret(int internalClientId, int secretId)
        {
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            var deleteResult = await _identityServerRepository.DeleteClientSecretByIdAsync(secretId, authorisedOrgIds);

            if (deleteResult.IsError)
            {
                return RedirectToAction(nameof(ManageClientCredentials), new { id = internalClientId });
            }

            return RedirectToAction(nameof(ManageClientCredentials), new { id = internalClientId } );
        }

        private async Task<List<Organisation>> GetRoleUsersOrganisations(string roleUserEmail)
        {
            var authorisedOrgIds = HttpContext.User.GetOrganisationIds();
            return await _identityManagementRepository
                .GetOrganisationsByEmailAndRoleAsync(roleUserEmail, RoleType.Developer, authorisedOrgIds);
        }

        private Client CreateBasicClientEntity(string clientName, List<string> scopes)
        {
            return new Client
            {
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientName = clientName,
                AllowedScopes = scopes,
                AlwaysIncludeUserClaimsInIdToken = true,
                ClientId = $"Client-{Guid.NewGuid():N}",
                ClientSecrets = new List<Secret>(),
                EnableLocalLogin = false,
                UpdateAccessTokenClaimsOnRefresh = true
            };
        }
    }
}
