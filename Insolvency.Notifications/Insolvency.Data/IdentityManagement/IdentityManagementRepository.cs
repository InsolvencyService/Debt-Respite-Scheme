using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Insolvency.Identity.Models;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Models.OperationResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Insolvency.Data.IdentityManagement
{
    public class IdentityManagementRepository : IIdentityManagementRepository
    {
        private readonly IdentityManagementContext _organisationContext;
        private readonly ILogger<IdentityManagementRepository> _logger;

        public IdentityManagementRepository(IdentityManagementContext organisationContext,
            ILogger<IdentityManagementRepository> logger)
        {
            _organisationContext = organisationContext ?? throw new ArgumentNullException(nameof(organisationContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Organisation> GetOrganisationByIdAsync(Guid id)
        {
            return await _organisationContext.Organisations.Where(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Organisation>> GetOrganisationsByEmailAndRoleAsync(string email, RoleType role, List<string> authorisedOrgIds)
        {
            return await _organisationContext.Organisations
                        .Where(x => authorisedOrgIds.Any(y => y.Equals(x.ExternalId)) &&
                                    x.RoleUsers.Any(y => y.Role == role &&
                                                         y.NormalisedEmail.Equals(email)))
                        .OrderBy(x => x.Name)
                        .ToListAsync();
        }

        public async Task<List<Organisation>> GetRoleUsersOrganisationDetailsByEmailAsync(string email, List<string> authorisedOrgIds)
        {
            var organisations = await _organisationContext.Organisations
                        .Include(x => x.RoleUsers)
                        .ThenInclude(x => x.Scopes)
                        .Where(x => authorisedOrgIds.Any(y => y.Equals(x.ExternalId)) &&
                                    x.RoleUsers.Any(y => y.NormalisedEmail.Equals(email)))
                        .OrderBy(x => x.Name)
                        .AsNoTracking()
                        .ToListAsync();

            foreach(var org in organisations)
            {
                org.RoleUsers.RemoveAll(x => !x.NormalisedEmail.Equals(email));
            }

            return organisations;
        }

        public async Task<List<Organisation>> GetOrganisationByOnboarderEmailAsync(string email)
        {
            email = email.ToUpper().Trim();
            return await _organisationContext.Organisations.Where(o => o.NormalisedOnboarderEmail == email).ToListAsync();
        }

        public async Task<List<Organisation>> GetOrganisationByScpGroupIdAsync(string scpGroupId)
        {
            return await _organisationContext.Organisations
                         .Include(o => o.RoleUsers)
                         .Where(o => o.ScpGroupId == scpGroupId).ToListAsync();
        }

        public async Task<List<Organisation>> GetOrganisationsByClientIdAsync(string clientId)
        {
            return await _organisationContext.OrganisationRoleUserAssignedClients
                         .Where(a =>
                            a.RoleUserAssignedClient.Client.ClientId == clientId &&
                            a.RoleUserAssignedClient.Client.Enabled)
                         .Select(a => a.Organisation).ToListAsync();
        }

        public async Task CompleteOnboardingForPendingOrganisationsAsync(string email, string scpGroupId)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrEmpty(scpGroupId))
            {
                throw new ArgumentNullException(nameof(scpGroupId));
            }

            email = email.ToUpper().Trim();

            var organisationsToOnboard = await _organisationContext.Organisations
                                                .Where(o => o.NormalisedOnboarderEmail == email)
                                                .Where(o => o.ScpGroupId == null)
                                                .ToListAsync();

            if (organisationsToOnboard.Count == 0)
            {
                return;
            }

            organisationsToOnboard.ForEach(o => o.ScpGroupId = scpGroupId);

            await _organisationContext.SaveChangesAsync();
        }

        public async Task<OperationResult> PrepareOrganisationAsync(Organisation organisation)
        {

            var isAlreadyExisting = await _organisationContext.Organisations
                                    .Where(o => o.ExternalId == organisation.ExternalId)
                                    .AnyAsync();
            if (isAlreadyExisting)
            {
                _logger.LogError($"Unable to create new organisation. Organsation with Id: {organisation.ExternalId} already exists.");
                return OperationResult.Error(OperationErrors.DuplicateOrganisation);
            }

            organisation.OnboarderEmail = organisation.OnboarderEmail?.Trim();
            organisation.NormalisedOnboarderEmail = organisation.OnboarderEmail?.Trim().ToUpper();

            _organisationContext.Organisations.Add(organisation);
            await _organisationContext.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<List<RoleUser>> GetAdministratorRoleUserByEmailAsync(string email, List<string> authorisedOrgIds)
        {
            return await _organisationContext.RoleUsers
                .Where(x => authorisedOrgIds.Any(y => y.Equals(x.Organisation.ExternalId)) &&
                                          x.Role == RoleType.Administrator &&
                                          x.NormalisedEmail.Equals(email))
                .ToListAsync();
        }

        public async Task<bool> CheckHasDeveloperRoleUserByEmailAsync(string email, List<string> authorisedOrgIds)
        {
            return await _organisationContext.RoleUsers
                .AnyAsync(x => authorisedOrgIds.Any(y => y.Equals(x.Organisation.ExternalId)) &&
                               x.Role == RoleType.Developer &&
                               x.NormalisedEmail.Equals(email));
        }

        public async Task<Organisation> GetOrganisationAndRoleUsersAndScopesByOrganisationIdAsync(Guid organisationId, List<string> authorisedOrgIds)
        {
            return await _organisationContext.Organisations
                .Include(x => x.RoleUsers)
                .ThenInclude(y => y.Scopes)
                .Where(x => x.Id == organisationId && authorisedOrgIds.Any(y => y.Equals(x.ExternalId)))
                .FirstOrDefaultAsync();
        }

        public async Task<OperationResult> AddRoleUserAsync(RoleUser roleUser, List<string> authorisedOrgIds)
        {
            var isAlreadyExisting = await _organisationContext.RoleUsers
                                    .Where(x => x.NormalisedEmail.Equals(roleUser.NormalisedEmail) &&
                                                x.OrganisationId == roleUser.OrganisationId &&
                                                x.Role == roleUser.Role)
                                    .AnyAsync();

            if (isAlreadyExisting)
            {
                _logger.LogError($"Unable to add role user. Role user with email: {roleUser.Email}, role: {roleUser.Role} and organisation: {roleUser.OrganisationId} already exists.");
                return OperationResult.Error(OperationErrors.DuplicateEmailAndRole);
            }

            var isAuthorisedToOrg = await _organisationContext.Organisations
                .AnyAsync(x => x.Id == roleUser.OrganisationId &&
                               authorisedOrgIds.Contains(x.ExternalId));

            if (!isAuthorisedToOrg)
            {
                _logger.LogError($"Not authorised to add Role user with email: {roleUser.Email}, role: {roleUser.Role} to organisation: {roleUser.OrganisationId}.");
                return OperationResult.Error(OperationErrors.Unauthorised);
            }

            _organisationContext.RoleUsers.Add(roleUser);
            await _organisationContext.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<RoleUser> FindRoleUserByIdAsync(Guid id, List<string> authorisedOrgIds)
        {
            return await _organisationContext.RoleUsers
                .FirstOrDefaultAsync(x => authorisedOrgIds.Any(y => y.Equals(x.Organisation.ExternalId)) && x.Id == id);
        }

        public async Task<OperationResult> UpdateRoleUserAsync(RoleUser roleUser, List<string> authorisedOrgIds)
        {
            var roleUserEntity = await FindRoleUserByIdAsync(roleUser.Id, authorisedOrgIds);

            if (roleUserEntity == null)
            {
                _logger.LogError($"Not authorised to update Role user with email: {roleUser.Email}, role: {roleUser.Role} to organisation: {roleUser.OrganisationId}.");
                return OperationResult.Error(OperationErrors.Unauthorised);
            }

            roleUserEntity.Email = roleUser.Email;
            roleUserEntity.NormalisedEmail = roleUser.NormalisedEmail;
            await _organisationContext.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteRoleUserByIdAsync(Guid id, List<string> authorisedOrgIds)
        {
            var roleUserEntity = await FindRoleUserByIdAsync(id, authorisedOrgIds);

            if (roleUserEntity == null)
            {
                _logger.LogError($"Not authorised to delete Role user with Id: {id}.");
                return OperationResult.Error(OperationErrors.Unauthorised);
            }

            _organisationContext.RoleUsers.Remove(roleUserEntity);
            await _organisationContext.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<List<OrganisationScope>> GetOrganisationScopesByOrganisationIdAsync(Guid organisationId, List<string> authorisedOrgIds)
        {
            return await _organisationContext.OrganisationScopes
                .Where(x => authorisedOrgIds.Any(y => y.Equals(x.Organisation.ExternalId)) &&
                            x.OrganisationId == organisationId)
                .ToListAsync();
        }

        public async Task<OperationResult<bool>> IsEmailInUseAsync(RoleUser roleUser, List<string> authorisedOrgIds)
        {
            var isTaken = await _organisationContext.RoleUsers
                        .AnyAsync(x => x.Id != roleUser.Id &&
                                       x.NormalisedEmail.Equals(roleUser.NormalisedEmail) &&
                                       x.OrganisationId == roleUser.OrganisationId &&
                                       authorisedOrgIds.Any(y => y.Equals(x.Organisation.ExternalId)) &&
                                       x.Role == roleUser.Role);

            if (isTaken)
            {
                _logger.LogError($"Unable to update role user email. Another role user with email: {roleUser.Email} already exists.");
                return OperationResult<bool>.Error(true, OperationErrors.DuplicateEmailAndRole);
            }

            return OperationResult<bool>.Success(false);
        }

        public async Task<List<string>> GetAccessibleOrganisationScopesAsync(
            List<Guid> selectedOrganisationIds,
            string roleUserEmail,
            List<string> authorisedOrgIds)
        {
            var organisations = await _organisationContext.Organisations
                                      .Where(x => authorisedOrgIds.Contains(x.ExternalId) &&
                                                  selectedOrganisationIds.Any(y => y == x.Id) &&
                                                  x.RoleUsers.Any(y => y.NormalisedEmail.Equals(roleUserEmail) &&
                                                                       y.Role == RoleType.Developer))
                                      .Include(x => x.RoleUsers)
                                      .ThenInclude(x => x.Scopes)
                                      .Include(x => x.Scopes)
                                      .ToListAsync();



            var accessibleOrgScopes = organisations.Select(x => x.Scopes.Select(y => y.Name))
                                                .Aggregate((a, b) => a.Intersect(b));

            var thisRoleUser = organisations.SelectMany(x => x.RoleUsers)
                                .Where(x => selectedOrganisationIds.Any(y => y == x.OrganisationId) &&
                                            x.NormalisedEmail.Equals(roleUserEmail) &&
                                            x.Role == RoleType.Developer)
                                .Distinct();

            var roleUserCommonScopes = thisRoleUser.Select(x => x.Scopes.Select(x => x.OrganisationScopeName))
                                                .Aggregate((a, b) => a.Intersect(b));

            accessibleOrgScopes = accessibleOrgScopes.Intersect(roleUserCommonScopes);

            return accessibleOrgScopes.ToList();
        }

        public async Task<OperationResult> LinkRoleUserToClientAsync(
            int clientDbId,
            string roleUserEmail,
            List<Guid> selectedOrganisationIds,
            List<string> selectedOrganisationScopes,
            List<string> authorisedOrgIds)
        {
            var extendedClientEntity = new RoleUserAssignedClient
            {
                ClientRecordId = clientDbId,
                CreatedByEmail = roleUserEmail,
            };

            _organisationContext.RoleUserAssignedClients.Add(extendedClientEntity);

            var organisations = await _organisationContext.Organisations
                .Include(x => x.Scopes)
                .Where(x => authorisedOrgIds.Any(y => y.Equals(x.ExternalId)) &&
                            selectedOrganisationIds.Any(y => y == x.Id) &&
                            x.Scopes.Any(y => selectedOrganisationScopes
                                              .Any(z => z == y.Name)))
                .ToListAsync();

            if (!organisations.Any())
            {
                _logger.LogError($"Not authorised to create links between Role user with Email: {roleUserEmail} and Client with Id: {clientDbId}");
                return OperationResult.Error(OperationErrors.Unauthorised);
            }

            _organisationContext.OrganisationRoleUserAssignedClients.AddRange(organisations.Select(x =>
                    new OrganisationRoleUserAssignedClient()
                    {
                        RoleUserAssignedClient = extendedClientEntity,
                        Organisation = x
                    }));

            var organisationScopes = organisations.SelectMany(x => x.Scopes)
                                   .Where(scope => selectedOrganisationScopes.Any(selectedScope => selectedScope.Equals(scope.Name)))
                                   .ToList();
            _organisationContext.OrganisationScopeRoleUserAssignedClients
                .AddRange(organisationScopes.Select(x => new OrganisationScopeRoleUserAssignedClient
                {
                    OrganisationScope = x,
                    RoleUserAssignedClient = extendedClientEntity
                }));

            await _organisationContext.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<List<Client>> GetAuthorisedClientsByEmailAsync(string roleUserEmail, List<string> authorisedOrgIds)
        {
            var allAuthorisedClients = await GetAuthorisedRoleUserAssignedClientsByEmailAndOrgIdsAsync(roleUserEmail, authorisedOrgIds);

            return allAuthorisedClients.Select(x => x.Client).ToList();
        }

        public async Task<OperationResult> DeleteClientCredentialsByClientRecordIdAsync(
            int id,
            string roleUserEmail,
            List<string> authorisedOrgIds)
        {
            var allAuthorisedClients = await GetAuthorisedRoleUserAssignedClientsByEmailAndOrgIdsAsync(roleUserEmail, authorisedOrgIds);

            var recordToDelete = allAuthorisedClients.FirstOrDefault(x => x.ClientRecordId == id);

            if (recordToDelete == null)
            {
                _logger.LogError($"Not authorised to delete Client Credentials for Id: {id}");
                return OperationResult.Error(OperationErrors.Unauthorised);
            }

            var client = new Client { Id = recordToDelete.Client.Id };
            _organisationContext.Clients.Attach(client);
            _organisationContext.Clients.Remove(client);
            await _organisationContext.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<RoleUserAssignedClient> GetRoleUserAssignedClientByClientRecordIdAsync(
            int id,
            string roleUserEmail,
            List<string> authorisedOrgIds)
        {
            var allAuthorisedClients = await GetAuthorisedRoleUserAssignedClientsByEmailAndOrgIdsAsync(roleUserEmail, authorisedOrgIds);

            var recordToView = allAuthorisedClients.FirstOrDefault(x => x.ClientRecordId == id);

            return recordToView;
        }

        public async Task<bool> CheckRoleUserExistsByEmailAsync(
            string roleUserEmail,
            List<RoleType> roles,
            List<string> authorisedOrgIds)
        {
            return await _organisationContext.RoleUsers
                .AnyAsync(x => authorisedOrgIds.Any(y => y.Equals(x.Organisation.ExternalId)) &&
                                x.NormalisedEmail.Equals(roleUserEmail) &&
                                roles.Contains(x.Role));
        }

        private async Task<List<RoleUserAssignedClient>> GetAuthorisedRoleUserAssignedClientsByEmailAndOrgIdsAsync(
            string roleUserEmail,
            List<string> authorisedOrgIds)
        {
            var authorisedOrgRoleUserClients = await _organisationContext.OrganisationRoleUserAssignedClients
                            .Include(x => x.Organisation)
                            .ThenInclude(x => x.RoleUsers)
                            .ThenInclude(x => x.Scopes)
                            .Include(x => x.Organisation)
                            .ThenInclude(x => x.Scopes)
                            .Include(x => x.RoleUserAssignedClient)
                            .ThenInclude(x => x.Client)
                            .ThenInclude(x => x.AllowedScopes)
                            .Include(x => x.RoleUserAssignedClient)
                            .ThenInclude(x => x.Client)
                            .ThenInclude(x => x.ClientSecrets)
                            .Include(x => x.RoleUserAssignedClient)
                            .ThenInclude(x => x.OrganisationScopeRoleUserAssignedClients)
                            .ThenInclude(x => x.OrganisationScope)
                            .ThenInclude(x => x.Organisation)
                            .Where(y => authorisedOrgIds.Contains(y.Organisation.ExternalId))
                            .AsNoTracking()
                            .ToListAsync();

            foreach (var item in authorisedOrgRoleUserClients)
            {
                item.Organisation.RoleUsers.RemoveAll(x => !x.NormalisedEmail.Equals(roleUserEmail));
            }

            var clientsAdminOf = authorisedOrgRoleUserClients.Where(x => x.Organisation.RoleUsers.Any(x => x.Role == RoleType.Administrator))
                                               .Select(x => x.RoleUserAssignedClient)
                                               .GroupBy(x => x.Id)
                                               .Select(x => x.FirstOrDefault())
                                               .ToList();

            var clientsPossiblyDevOf = authorisedOrgRoleUserClients
                                   .Where(x => x.Organisation.RoleUsers.Any(x => x.Role == RoleType.Developer))
                                   .Select(x => x.RoleUserAssignedClient)
                                   .GroupBy(x => x.Id)
                                   .Select(x => x.FirstOrDefault())
                                   .ToList();

            foreach (var item in clientsPossiblyDevOf)
            {
                var roleUsers = item.OrganisationRoleUserAssignedClients
                .SelectMany(x => x.Organisation.RoleUsers)
                .ToList();

                var isClientVisible = item.OrganisationScopeRoleUserAssignedClients
                                      .All(x => roleUsers
                                                .All(y => y.Scopes
                                                          .Any(z => z.OrganisationScopeName.Equals(x.OrganisationScope.Name))));

                if (isClientVisible)
                {
                    clientsAdminOf.Add(item);
                }
            }

            return clientsAdminOf;
        }
    }
}