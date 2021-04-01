using System;
using System.Threading.Tasks;
using Insolvency.Models.Audit;
using Microsoft.Extensions.Logging;

namespace Insolvency.Models
{
    public class AuditService : IAuditService
    {
        private IRestClientFactory _restFactory;
        private string _baseUrl;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IRestClientFactory restFactory, string baseUrl, ILogger<AuditService> logger)
        {
            _restFactory = restFactory;
            _baseUrl = baseUrl;
            _logger = logger;
        }

        public async Task PerformAuditing(AuditDetail auditDetail)
        {
            var client = _restFactory.CreateClient(_baseUrl);
            var request = _restFactory.CreateRequest("api/InssAuditMessaging", RestSharp.Method.POST);
            request = request.AddJsonBody(auditDetail);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                string errorMessage = $"Auditing Failed: {response.StatusCode}";

                _logger.LogWarning(errorMessage);
                
                throw new Exception(errorMessage);
            }
        }
    }
}
