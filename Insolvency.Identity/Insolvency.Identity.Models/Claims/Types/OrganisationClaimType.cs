using System;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Insolvency.Identity.Models.Claims.Types
{
    public class OrganisationClaimType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("orgid")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string OrganisationTypeName { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<int> CurrentUserRoles { get; set; }

        public Claim ToClaim()
        {
            var value = JsonConvert.SerializeObject(this);
            return new Claim(InssClaimTypes.Organisation, value);
        }

        public static OrganisationClaimType FromClaim(Claim organisationClaim)
        {
            if (organisationClaim.Type != InssClaimTypes.Organisation)
            {
                throw new ArgumentException($"Cannot create organisation type claim. The supplied source claim is not of type {InssClaimTypes.Organisation}", nameof(organisationClaim));
            }
            return JsonConvert.DeserializeObject<OrganisationClaimType>(organisationClaim.Value);
        }
    }
}
