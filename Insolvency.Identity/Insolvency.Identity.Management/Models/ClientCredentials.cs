namespace Insolvency.Management.Models
{
    public class ClientCredentials
    {
        public ClientCredentials() { }
        public ClientCredentials(string clientId, string secret)
        {
            this.ClientId = clientId;
            this.Secret = secret;
        }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}
