using System.Collections.Generic;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class ClientCredentialsViewModel
    {
        public string CurrentRoleUserEmail { get; set; }
        public bool AbleToAddClient { get; set; }
        public List<ClientViewModel> Clients { get; set; }
    }
}
