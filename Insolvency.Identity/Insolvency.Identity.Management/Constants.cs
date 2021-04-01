namespace Insolvency.Management
{
    public static class Constants
    {
        public const string AdministratorPolicyName = "IsAdministrator";
        public const string DeveloperPolicyName = "IsDeveloper";
        public const string AnyRolePolicyName = "IsAnyRole";

        public static class AuthSchemes
        {
            public const string CookieAuthSckeme = "Cookies";
            public const string OidcAuthScheme = "oidc";
        }
    }
}
