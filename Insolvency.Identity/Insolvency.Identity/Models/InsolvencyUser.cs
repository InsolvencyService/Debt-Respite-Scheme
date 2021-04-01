using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using Insolvency.Identity.Extensions;
using Insolvency.Identity.Models.Claims;

namespace Insolvency.Identity.Models
{
    public class InsolvencyUser : IdentityServerUser
    {
        public InsolvencyUser(ClaimsPrincipal claimsPrincipal) : base(claimsPrincipal.FindFirstValue(JwtClaimTypes.Subject))
        {
            var name = claimsPrincipal.FindFirstValue(JwtClaimTypes.Name);
            var groupId = claimsPrincipal.FindFirstValue(ScpClaimTypes.GroupId);
            var email = claimsPrincipal.FindFirstValue(JwtClaimTypes.Email);
            var profile = claimsPrincipal.FindFirstValue(JwtClaimTypes.Profile);
            var groupProfile = claimsPrincipal.FindFirstValue(ScpClaimTypes.GroupProfile);

            Name = name;
            Email = email;
            ScpGroupId = groupId;
            ProfileUrl = profile;
            GroupProfileUrl = groupProfile;
            DisplayName = name;
            IdentityProvider = Constants.ScpAuthenticationSchemeAlias;

            AdditionalClaims.AddIfValueNotNull(ScpClaimTypes.GroupId, groupId);
            AdditionalClaims.AddIfValueNotNull(JwtClaimTypes.Email, email);
            AdditionalClaims.AddIfValueNotNull(JwtClaimTypes.Name, name);
            AdditionalClaims.AddIfValueNotNull(JwtClaimTypes.Profile, profile);
            AdditionalClaims.AddIfValueNotNull(ScpClaimTypes.GroupProfile, groupProfile);
        }

        public string Name { get; }

        public string Email { get; }

        public string ScpGroupId { get; }

        public string ProfileUrl { get; }

        public string GroupProfileUrl { get; }

        public IEnumerable<Claim> GetInsolvencyClaims()
        {
            var claims = new List<Claim>();
            claims.AddIfValueNotNull(InssClaimTypes.ScpGroupId, ScpGroupId);
            claims.AddIfValueNotNull(JwtClaimTypes.Email, Email);
            claims.AddIfValueNotNull(JwtClaimTypes.Name, Name);
            claims.AddIfValueNotNull(JwtClaimTypes.Profile, ProfileUrl);
            claims.AddIfValueNotNull(InssClaimTypes.GroupProfile, GroupProfileUrl);
            return claims;
        }
    }
}
