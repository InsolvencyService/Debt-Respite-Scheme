using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using Insolvency.Common;
using Microsoft.AspNetCore.Authorization;

namespace Insolvency.IntegrationAPI.Authorization
{
    public static class AuthorizationExtensions
    {
        public static void ConfigureAuthorizationPolicies(this AuthorizationOptions options)
        {
            options.AddBSSPolicy(Constants.Auth.CreditorOrginisationType);
            options.AddBSSPolicy(Constants.Auth.MoneyAdviserOrginisationType);
            options.AddCommonBSSPolicy();
        }

        public static void AddBSSPolicy(this AuthorizationOptions options, string organisationType)
        {
            var policyName = $"{organisationType}{Constants.Auth.PolicySuffix}";
            var scope = $"{organisationType}{Constants.Auth.ScopeSuffix}";
            var organizationTypeNames = new List<string> { organisationType };
            options.AddPolicy(policyName, builder =>
            {
                builder.RequireAuthenticatedUser();
                builder.RequireClaim(JwtClaimTypes.Scope, scope);
                builder.Requirements.Add(new OrganisationAuthorizationRequirement() { OrganisationTypeNames = organizationTypeNames });
            });
        }

        public static void AddCommonBSSPolicy(this AuthorizationOptions options)
        {
            var policyName = Constants.Auth.CommonPolicy;

            var organisationTypeNames = new List<string>
            {
                Constants.Auth.MoneyAdviserOrginisationType,
                Constants.Auth.CreditorOrginisationType
            };

            var scopes = organisationTypeNames.Select(organisationType => $"{organisationType}{Constants.Auth.ScopeSuffix}");

            options.AddPolicy(policyName, builder =>
            {
                builder.RequireAuthenticatedUser();
                builder.RequireClaim(JwtClaimTypes.Scope, scopes);
                builder.Requirements.Add(new OrganisationAuthorizationRequirement() { OrganisationTypeNames = organisationTypeNames });
            });
        }
    }
}
