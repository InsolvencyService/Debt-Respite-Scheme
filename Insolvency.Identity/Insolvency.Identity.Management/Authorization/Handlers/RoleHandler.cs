using System;
using System.Threading.Tasks;
using Insolvency.Identity.Models.Claims;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Management.Authorization.Requirements;
using Insolvency.Management.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Insolvency.Management.Authorization.Handlers
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly IIdentityManagementRepository _identityManagementRepository;

        public RoleHandler(IIdentityManagementRepository identityManagementRepository)
        {
            _identityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == InssClaimTypes.Organisation))
            {
                return;
            }

            var userEmail = context.User.GetNormalisedEmail();
            var authorisedOrgIds = context.User.GetOrganisationIds();
            var doesRoleUserExist = await _identityManagementRepository.CheckRoleUserExistsByEmailAsync(
                                            userEmail,
                                            requirement.RoleTypes,
                                            authorisedOrgIds);

            if (doesRoleUserExist)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
