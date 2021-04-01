using Microsoft.AspNetCore.Authorization;

namespace Insolvency.Notifications.API.Authorization
{
    public static class AuthorizationExtensions
    {
        public static void ConfigureAuthorizationPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(Constants.Auth.HasSelectedOrganisationPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new OrganisationRequirement());
            });
        }
    }
}
