using System.Collections.Generic;
using System.Linq;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class ManageClientCredentialsViewModel
    {
        public List<IGrouping<string, OrganisationScope>> Scopes { get; set; }
        public ClientViewModel Client { get; set; }
        public RoleUserAssignedClient ExtendedClient { get; set; }
    }
}
