namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class AddClientSecretResultViewModel
    {
        public int InternalClientId { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}
