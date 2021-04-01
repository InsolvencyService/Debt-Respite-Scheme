namespace Insolvency.Common.Identity.Claims
{
    public static class InssClaimTypes
    {
        private const string InsolvencyClaimsPrefix = "inss:";

        public static string ScpGroupId => $"{InsolvencyClaimsPrefix}scp:groupid";

        public static string Organisation => $"{InsolvencyClaimsPrefix}organisation";
    }
}
