using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Insolvency.Identity.Models.IdentityServer;
using Insolvency.Models.OperationResults;

namespace Insolvency.Interfaces.IdentityServer
{
    public interface IIdentityServerRepository
    {
        Task<OperationResult<ClientCreationResult>> CreateIdentityClientAsync(Client client);
        Task<OperationResult> DeleteClientSecretByIdAsync(int id, List<string> authorisedOrgIds);
        Task<OperationResult<AddSecretToClientResult>> AddSecretToClient(
            int clientRecordId,
            string description,
            List<string> authorisedOrgIds);
    }
}
