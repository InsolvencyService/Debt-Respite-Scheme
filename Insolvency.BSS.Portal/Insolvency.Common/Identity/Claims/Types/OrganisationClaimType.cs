using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Insolvency.Common.Identity.Claims.Types
{
    public class OrganisationClaimType
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("orgid")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string OrganisationTypeName { get; set; }       

        public static OrganisationClaimType FromClaim(Claim organisationClaim)
        {
            if (organisationClaim.Type != InssClaimTypes.Organisation)
            {
                throw new ArgumentException($"Cannot create organisation type claim. The supplied source claim is not of type {InssClaimTypes.Organisation}", nameof(organisationClaim));
            }
            return JsonSerializer.Deserialize<OrganisationClaimType>(organisationClaim.Value);
        }
    }
}
