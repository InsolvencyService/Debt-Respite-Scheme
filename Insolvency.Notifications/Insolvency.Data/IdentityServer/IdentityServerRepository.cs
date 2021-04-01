using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Insolvency.Data.IdentityManagement;
using Insolvency.Identity.Models.IdentityServer;
using Insolvency.Interfaces.IdentityServer;
using Insolvency.Models.OperationResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Insolvency.Data.IdentityServer
{
    public class IdentityServerRepository : IIdentityServerRepository
    {
        private readonly IdentityManagementContext _organisationContext;
        private readonly ILogger<IdentityServerRepository> _logger;

        public IdentityServerRepository(IdentityManagementContext organisationContext, ILogger<IdentityServerRepository> logger)
        {
            _organisationContext = organisationContext ?? throw new ArgumentNullException(nameof(organisationContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<ClientCreationResult>> CreateIdentityClientAsync(Client client)
        {
            var result = new ClientCreationResult(client.ClientId);

            var isClientIdInUse = await _organisationContext.Clients
                                  .Where(c => c.ClientId == client.ClientId)
                                  .AnyAsync();
            if (isClientIdInUse)
            {
                return OperationResult<ClientCreationResult>.Error(result, OperationErrors.DuplicateClientId);
            }

            (var plainTextSecret, var secret) = GenerateSecret("Initial Secret");
            client.ClientSecrets.Add(secret);
            var entityClient = client.ToEntity();
            entityClient.ClientSecrets.ForEach(x => x.Created = DateTime.UtcNow);
            _organisationContext.Clients.Add(entityClient);
            await _organisationContext.SaveChangesAsync();

            result.Secret = plainTextSecret;
            result.Id = entityClient.Id;

            return OperationResult<ClientCreationResult>.Success(result);
        }

        public async Task<OperationResult> DeleteClientSecretByIdAsync(int id, List<string> authorisedOrgIds)
        {
            var isAuthorisedToOrg = await _organisationContext.Organisations
                .AnyAsync(x => x.OrganisationRoleUserAssignedClients.Any(y => y.RoleUserAssignedClient.Client.ClientSecrets.Any(z => z.Id == id)) && 
                               authorisedOrgIds.Any(y => y.Equals(x.ExternalId)));

            if (!isAuthorisedToOrg)
            {
                _logger.LogError($"Not authorised to delete Client Secret Id: {id}.");
                return OperationResult.Error(OperationErrors.Unauthorised);
            }

            var secret = new IdentityServer4.EntityFramework.Entities.ClientSecret { Id = id };
            var clientsDbSet = _organisationContext.Set<IdentityServer4.EntityFramework.Entities.ClientSecret>();
            clientsDbSet.Attach(secret);
            clientsDbSet.Remove(secret);
            await _organisationContext.SaveChangesAsync();

            return OperationResult.Success();
        }

        public async Task<OperationResult<AddSecretToClientResult>> AddSecretToClient(
            int clientRecordId,
            string description, 
            List<string> authorisedOrgIds)
        {

            var isAuthorisedToOrg = await _organisationContext.Organisations
                .AnyAsync(x => x.OrganisationRoleUserAssignedClients.Any(y => y.RoleUserAssignedClient.Client.Id == clientRecordId) &&
                               authorisedOrgIds.Any(y => y.Equals(x.ExternalId)));

            if (!isAuthorisedToOrg)
            {
                _logger.LogError($"Not authorised to add Secret to Client with Id: {clientRecordId}.");
                return OperationResult<AddSecretToClientResult>.Error(new AddSecretToClientResult(), OperationErrors.Unauthorised);
            }

            var client = await _organisationContext.Clients
                            .Include(x => x.ClientSecrets)
                            .FirstOrDefaultAsync(x => x.Id == clientRecordId);

            var result = new AddSecretToClientResult(
                            client.Id,
                            client.ClientId,
                            string.Empty,
                            description);

            if (client.ClientSecrets.Count() >= 2)
            {
                return OperationResult<AddSecretToClientResult>.Error(result, OperationErrors.ClientSecretCountLimitReached);
            }

            (var plainText, var secret) = GenerateSecret(description);
            var newSecret = new IdentityServer4.EntityFramework.Entities.ClientSecret
            {
                Value = secret.Value,
                Description = secret.Description,
                Type = secret.Type,
                Created = DateTime.UtcNow
            };

            client.ClientSecrets.Add(newSecret);
            await _organisationContext.SaveChangesAsync();
            result.Secret = plainText;

            return OperationResult<AddSecretToClientResult>.Success(result);
        }

        protected virtual (string, Secret) GenerateSecret(string description)
        {
            var plainText = $"{Guid.NewGuid():N}{Guid.NewGuid():N}";
            var newClientSecret = new Secret(plainText.Sha256());
            newClientSecret.Description = description;
            newClientSecret.Type = "SharedSecret";
            return (plainText, newClientSecret);
        }
    }
}
