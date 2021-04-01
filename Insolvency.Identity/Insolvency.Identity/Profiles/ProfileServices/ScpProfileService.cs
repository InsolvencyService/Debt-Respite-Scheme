using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Insolvency.Identity.Models;
using Insolvency.Identity.Models.Claims.Types;
using Insolvency.Interfaces.IdentityManagement;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Profiles.ProfileServices
{
    public class ScpProfileService : IProfileService
    {
        private readonly IIdentityManagementRepository _iIdentityManagementRepository;
        private readonly ILogger<ScpProfileService> _logger;

        public ScpProfileService(IIdentityManagementRepository identityManagementRepository, ILogger<ScpProfileService> logger)
        {
            _iIdentityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = new InsolvencyUser(context.Subject);
            context.AddRequestedClaims(user.GetInsolvencyClaims());
            await AddOrganisationClaims(context, user);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }

        protected virtual async Task AddOrganisationClaims(ProfileDataRequestContext context, InsolvencyUser user)
        {
            if (string.IsNullOrEmpty(user.ScpGroupId))
            {
                return;
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                return;
            }

            var organisations = await _iIdentityManagementRepository.GetOrganisationByScpGroupIdAsync(user.ScpGroupId);
            if (organisations.Count == 0)
            {
                _logger.LogWarning($"No organisations for user with ScpGroupId: {user.ScpGroupId} could be found! Unable to add requested claims.");
                return;
            }

            context.AddRequestedClaims(CreateOrganisationClaims(organisations, user.Email));
        }

        protected virtual List<Claim> CreateOrganisationClaims(List<Organisation> organisations, string email)
        {
            var claims = organisations.Select(org =>
                new OrganisationClaimType
                {
                    Id = org.ExternalId,
                    Name = org.Name,
                    OrganisationTypeName = org.Type,
                    CurrentUserRoles = org.RoleUsers
                                       .Where(ru => ru.Email == email)                                      
                                       .Select(ru => (int)ru.Role)
                                       .Distinct()
                }.ToClaim()).ToList();
            return claims;
        }
    }
}
