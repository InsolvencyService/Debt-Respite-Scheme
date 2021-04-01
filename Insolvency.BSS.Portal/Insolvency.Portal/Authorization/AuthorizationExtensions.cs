using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Common;

namespace Insolvency.Portal.Authorization
{
    public static class AuthorizationExtensions
    {
        public static void ConfigureAuthorizationPolicies(this AuthorizationOptions options)
        {
            options.AddBSSPolicy(Constants.Auth.CreditorOrginisationType);
            options.AddBSSPolicy(Constants.Auth.MoneyAdviserOrginisationType);
        }

        public static void AddBSSPolicy(this AuthorizationOptions options, string organisationType)
        {
            var policyName = $"{organisationType}{Constants.Auth.PolicySuffix}";
            var scope = $"{organisationType}{Constants.Auth.ScopeSuffix}";
            options.AddPolicy(policyName, builder =>
            {
                builder.RequireAuthenticatedUser();
                builder.Requirements.Add(new OrganisationAuthorizationRequirement() { OrganisationTypeName = organisationType });
            });
        }
    }
}
