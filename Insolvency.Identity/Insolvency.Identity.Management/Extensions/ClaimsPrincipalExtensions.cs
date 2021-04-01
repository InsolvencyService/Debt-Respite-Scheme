using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Insolvency.Identity.Models.Claims;
using Insolvency.Identity.Models.Claims.Types;
using Newtonsoft.Json;

namespace Insolvency.Management.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(JwtClaimTypes.Name);
        }

        public static string GetNormalisedEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Email).Trim().ToUpper();
        }

        public static List<string> GetOrganisationIds(this ClaimsPrincipal claimsPrincipal)
        {
            var claims = claimsPrincipal.FindAll(x => x.Type == InssClaimTypes.Organisation).ToList();

            return claims.Select(x => JsonConvert.DeserializeObject<OrganisationClaimType>(x.Value).Id)
                .ToList();
        }

        public static string GetManageGroupProfileUrl(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(InssClaimTypes.GroupProfile);
        }

        public static string GetManageProfileUrl(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(JwtClaimTypes.Profile);
        }
    }
}
