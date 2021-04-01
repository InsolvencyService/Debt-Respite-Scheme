using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Insolvency.Identity.Models.Claims;
using Insolvency.Identity.Models.Claims.Types;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Interfaces.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Tokens
{
    public class OrganisationTokenService : DefaultTokenService
    {
        private readonly IIdentityManagementRepository _iIdentityManagementRepository;
        private readonly IIdentityServerRepository _iIdentityServerRepository;

        public OrganisationTokenService(
            IClaimsService claimsProvider,
            IIdentityServerRepository iIdentityServerRepository,
            IReferenceTokenStore referenceTokenStore,
            ITokenCreationService creationService,
            IHttpContextAccessor contextAccessor,
            ISystemClock clock,
            IKeyMaterialService keyMaterialService,
            IdentityServerOptions options,
            ILogger<DefaultTokenService> logger,
            IIdentityManagementRepository identityManagementRepository)
        : base(claimsProvider, referenceTokenStore, creationService, contextAccessor, clock, keyMaterialService, options, logger)
        {
            _iIdentityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
            _iIdentityServerRepository = iIdentityServerRepository ?? throw new ArgumentNullException(nameof(iIdentityServerRepository));
        }

        public override async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            var token = await base.CreateAccessTokenAsync(request);

            if (ShouldAddOrganisationClaims(request))
            {
                await AddOrganisationClaimsAsync(token);
            }

            return token;
        }

        public virtual bool ShouldAddOrganisationClaims(TokenCreationRequest request)
        {
            var requestedClaimTypes = new HashSet<string>();
            requestedClaimTypes.UnionWith(request.ValidatedResources.Resources.ApiResources.SelectMany(r => r.UserClaims));
            requestedClaimTypes.UnionWith(request.ValidatedResources.Resources.ApiScopes.SelectMany(s => s.UserClaims));

            var isOrganisaionClaimRequired = requestedClaimTypes.Contains(InssClaimTypes.Organisation);
            var isClientCredentials = (request.ValidatedRequest as ValidatedTokenRequest)?.GrantType == GrantTypes.ClientCredentials.First();

            return isOrganisaionClaimRequired && isClientCredentials;
        }

        public virtual async Task AddOrganisationClaimsAsync(Token token)
        {
            var orgs = await _iIdentityManagementRepository.GetOrganisationsByClientIdAsync(token.ClientId);
            var orgClaims = orgs
            .Select(org =>
                new OrganisationClaimType
                {
                    Id = org.ExternalId,
                    Name = org.Name,
                    OrganisationTypeName = org.Type,
                }.ToClaim())
            .ToList();
            orgClaims.ForEach(orgClaim => token.Claims.Add(orgClaim));
        }
    }
}
