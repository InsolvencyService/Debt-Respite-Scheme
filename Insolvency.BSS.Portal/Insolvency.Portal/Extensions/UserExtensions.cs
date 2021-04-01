using Insolvency.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Insolvency.Common.Identity.Claims;
using Insolvency.Common.Identity.Claims.Types;

namespace Insolvency.Portal.Extensions
{
    public static class UserExtensions
    {
        public static List<OrganisationModel> GetAvailableOrganisations(this ClaimsPrincipal user)
        {
            var organisationClaims = user.Claims.Where(c => c.Type == InssClaimTypes.Organisation).ToList();
            return organisationClaims
                   .Select(claim => OrganisationClaimType.FromClaim(claim))
                   .Select(claimType => OrganisationModel.FromClaimType(claimType))
                   .ToList();
        }
    }
}
