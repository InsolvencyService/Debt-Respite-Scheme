namespace Insolvency.Identity.Models.IdentityServer
{
    public class AddSecretToClientResult
    {
        public AddSecretToClientResult() { }
        public AddSecretToClientResult(
            int internalClientId, 
            string clientId, 
            string secret,
            string description)
        {
            this.InternalClientId = internalClientId;
            this.ClientId = clientId;
            this.Secret = secret;
            this.Description = description;
        }

        public int InternalClientId { get; set; }

        public string ClientId { get; set; }

        public string Secret { get; set; }

        public string Description { get; set; }
    }
}
