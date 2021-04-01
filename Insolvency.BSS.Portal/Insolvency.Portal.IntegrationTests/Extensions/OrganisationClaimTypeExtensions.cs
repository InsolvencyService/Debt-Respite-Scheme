using System.Security.Claims;
using System.Text.Json;
using Insolvency.Common.Identity.Claims;
using Insolvency.Common.Identity.Claims.Types;

namespace Insolvency.Portal.IntegrationTests.Extensions
{
    public static class OrganisationClaimTypeExtensions
    {
        public static Claim ToClaim(this OrganisationClaimType organisation)
        {
            var value = JsonSerializer.Serialize(organisation);
            return new Claim(InssClaimTypes.Organisation, value);
        }
    }
}
