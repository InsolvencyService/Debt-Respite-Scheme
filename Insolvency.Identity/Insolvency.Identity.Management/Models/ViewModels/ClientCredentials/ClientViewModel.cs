using System.Collections.Generic;
using System.Linq;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class ClientViewModel
    {
        public ClientViewModel()
        {
            this.Secrets = new List<ClientSecretViewModel>();
        }

        public ClientViewModel(int id, string clientName)
        {
            this.Id = id;
            this.ClientName = clientName;
            this.Secrets = new List<ClientSecretViewModel>();
        }

        public ClientViewModel(RoleUserAssignedClient extendedClient, string currentRoleUserEmail)
        {
            this.Id = extendedClient.Client.Id;
            this.ClientName = extendedClient.Client.ClientName;
            this.ClientId = extendedClient.Client.ClientId;
            this.Secrets = extendedClient.Client.ClientSecrets
                            .Select(x => new ClientSecretViewModel(x))
                            .ToList();
            this.AbleToAddSecret = Secrets.Count() < 2 &&
                                   extendedClient.OrganisationRoleUserAssignedClients.Any(x => x.Organisation.RoleUsers
                                                                                               .Any(y => y.Role == RoleType.Developer));
        }

        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public List<ClientSecretViewModel> Secrets { get; set; }
        public bool AbleToAddSecret { get; set; }
    }
}
