
namespace Insolvency.Identity
{
    public static class Constants
    {
        public const string ScpAuthenticationSchemeAlias = "govscp";
        public const string ResponseType = "code";
        public const string ResponseMode = "form_post";
        public const string CallbackPath = "/auth-callback";
        public const int AuthenticationLifeTimeFromHours = 1;
    }
}
