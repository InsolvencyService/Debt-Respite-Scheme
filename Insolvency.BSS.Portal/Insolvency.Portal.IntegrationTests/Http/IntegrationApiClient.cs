using System;
using Microsoft.Extensions.Logging;
using Insolvency.RestClient;

namespace Insolvency.Portal.IntegrationTests.Http
{
    public class IntegrationApiClient : ApiClient
    {
        public IntegrationApiClient(
            IRestClientFactory clientFactory,
            string baseUrl,
            AuthorityDetails authDetails,
            ILogger<IntegrationApiClient> logger) 
        : base(clientFactory, baseUrl, authDetails, logger)
        {
        }

        protected override Guid? GetCurrentOrganisationId()
        {
            return Main.ManagingMoneyOrganisationId;
        }
    }
}
