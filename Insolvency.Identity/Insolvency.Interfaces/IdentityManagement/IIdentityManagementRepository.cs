using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Insolvency.Identity.Models;
using Insolvency.Models.OperationResults;

namespace Insolvency.Interfaces.IdentityManagement
{
    public interface IIdentityManagementRepository
    {
        Task<Organisation> GetOrganisationByIdAsync(Guid id);

        Task<Organisation> GetOrganisationAndRoleUsersAndScopesByOrganisationIdAsync(Guid organisationId, List<string> authorisedOrgIds);

        Task<List<Organisation>> GetOrganisationByOnboarderEmailAsync(string email);

        Task<List<Organisation>> GetOrganisationByScpGroupIdAsync(string scpGroupId);

        Task<List<Organisation>> GetOrganisationsByEmailAndRoleAsync(string email, RoleType role, List<string> authorisedOrgIds);

        Task<List<Organisation>> GetRoleUsersOrganisationDetailsByEmailAsync(string email, List<string> authorisedOrgIds);

        Task<List<Organisation>> GetOrganisationsByClientIdAsync(string clientId);

        Task<List<OrganisationScope>> GetOrganisationScopesByOrganisationIdAsync(Guid organisationId, List<string> authorisedOrgIds);

        Task<RoleUser> FindRoleUserByIdAsync(Guid id, List<string> authorisedOrgIds);

        Task<List<RoleUser>> GetAdministratorRoleUserByEmailAsync(string email, List<string> authorisedOrgIds);

        Task<RoleUserAssignedClient> GetRoleUserAssignedClientByClientRecordIdAsync(int id, string roleUserEmail, List<string> authorisedOrgIds);

        Task<List<Client>> GetAuthorisedClientsByEmailAsync(string roleUserEmail, List<string> authorisedOrgIds);

        Task<OperationResult> PrepareOrganisationAsync(Organisation organisation);

        Task<OperationResult> UpdateRoleUserAsync(RoleUser roleUser, List<string> authorisedOrgIds);

        Task<OperationResult> DeleteRoleUserByIdAsync(Guid id, List<string> authorisedOrgIds);

        Task<OperationResult<bool>> IsEmailInUseAsync(RoleUser roleUser, List<string> authorisedOrgIds);

        Task<OperationResult> AddRoleUserAsync(RoleUser roleUser, List<string> authorisedOrgIdsr);

        Task<OperationResult> LinkRoleUserToClientAsync(
            int clientDbId,
            string roleUserEmail,
            List<Guid> selectedOrganisationIds,
            List<string> selectedOrganisationScopes,
            List<string> authorisedOrgIds);

        Task<OperationResult> DeleteClientCredentialsByClientRecordIdAsync(int id, string roleUserEmail, List<string> authorisedOrgIds);

        Task<List<string>> GetAccessibleOrganisationScopesAsync(List<Guid> organisationIds, string roleUserEmail, List<string> authorisedOrgIds);

        Task<bool> CheckRoleUserExistsByEmailAsync(string roleUserEmail, List<RoleType> roles, List<string> authorisedOrgIds);

        Task<bool> CheckHasDeveloperRoleUserByEmailAsync(string email, List<string> authorisedOrgIds);

        Task CompleteOnboardingForPendingOrganisationsAsync(string email, string scpGroupId);
    }
}
