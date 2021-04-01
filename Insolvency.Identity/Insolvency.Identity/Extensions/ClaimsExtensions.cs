using System.Collections.Generic;
using System.Security.Claims;

namespace Insolvency.Identity.Extensions
{
    public static class ClaimsExtensions
    {
        public static void AddIfValueNotNull(this ICollection<Claim> claims, string claimType, string claimValue)
        {
            if (string.IsNullOrEmpty(claimValue))
            {
                return;
            }

            claims.Add(new Claim(claimType, claimValue));
        }
    }
}
