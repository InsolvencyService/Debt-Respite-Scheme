namespace Insolvency.RestClient
{
    public class AuthorityDetails
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientUrl { get; set; }
        public object ResourceUrl { get; set; }

        public string Scope { get; set; }
    }
}
