namespace Insolvency.Identity.Models.IdentityServer
{
    public class ClientCreationResult
    {
        public ClientCreationResult(string clientId)
        {
            ClientId = clientId;
        }
        public string ClientId { get; set; }

        public string Secret { get; set; }
        
        public int Id { get; set; }
    }
}
