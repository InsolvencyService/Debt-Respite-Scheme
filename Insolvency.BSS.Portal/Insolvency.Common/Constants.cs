namespace Insolvency.Common
{
    public static class Constants
    {
        public const string IntegrationAPICacheKey = "BSS-IntegrationAPI-";
        public const string CMPCacheListKey = "CMPCreditors";
        public const string UkCountryValue = "United Kingdom";
        public const string UkPostcodeRegex = @"([a-pr-uwyzA-PR-UWYZ]([\d]{1,2}|([a-hk-yA-HK-Y][\d]([\dabehmnprv-yABEHMNPRV-Y])?)|[\d][a-hjkps-uwA-HJKPS-UW]) ?[\d][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}|GIR ?0AA)";
        public const string UkNinRegex = @"^(?!BG|bg)(?!GB|gb)(?!NK|nk)(?!KN|kn)(?!TN|tn)(?!NT|nt)(?!ZZ|zz)(?:[a-ceghj-pr-tw-zA-CEGHJ-PR-TW-Z][a-ceghj-pr-tw-zA-CEGHJ-NPR-TW-Z])(?:\s*\d\s*){6}([a-dA-D]|\s)$";
        public const string AbbreviatedMonthFormat = "dd-MMM-yyyy";
        public const string PrettyDateFormat = "d MMMM yyyy";
        public const string UkDateFormat = "dd/MM/yyyy";
        public const string UkDateFullMonthFormat = "dd MMMM yyyy";
        public const string UkDateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        public const string UkDateHoursAndMinutesFormat = "dd/MM/yyyy HH:mm";
        public const string WindowsUKSystemTimeZone = "GMT Standard Time";
        public const string LinuxUKSystemTimeZone = "Europe/London";
        // starting with UK country code followed by digits
        public const string BssTelephoneRegex = @"^(?:\+44)[\d]*\d$";
        public const string OrganisationItemsKey = "OrganisationItem";
        public const string BssReferenceNumberRegex = @"^(?:BSS-)\d{10}$";
        public const string TwoDecimalPointFormat = "0.00";

        public const string MaxPageSizeODataKey = "MaxPageSizeODataKey";
        public const string RequestedPageKey = "RequestedPageKey";
        public const int PageSize = 10;

        public static class Auth
        {
            public const string PolicySuffix = ".policy";
            public const string ScopeSuffix = ".api";

            public const string CreditorOrginisationType = "bss.creditor";
            public const string CreditorPolicy = CreditorOrginisationType + PolicySuffix;

            public const string MoneyAdviserOrginisationType = "bss.moneyadviser";
            public const string MoneyAdviserPolicy = MoneyAdviserOrginisationType + PolicySuffix;

            public const string CommonType = "bss.common";
            public const string CommonPolicy = CommonType + PolicySuffix;

            public const string CookiesAuthenticationScheme = "Cookies";
            public const string OpenIdAuthenticationScheme = "oidc";
        }
    }
}
