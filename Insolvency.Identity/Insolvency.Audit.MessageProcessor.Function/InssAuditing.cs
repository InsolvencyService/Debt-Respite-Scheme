using System.Threading.Tasks;
using Insolvency.Interfaces.Audit;
using Insolvency.Models.Audit;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Insolvency.Audit.MessageProcessor.Function
{
    public class InssAuditing
    {
        private readonly IAuditRepository _auditRepository;

        public InssAuditing(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        [FunctionName("InssAuditing")]
        public async Task Run([ServiceBusTrigger("%AuditSbQueueName%", Connection = "AuditSbConnection")] string sbMessage, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<AuditMessage>(sbMessage);
            await _auditRepository.StoreAsync(message);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {sbMessage}");
        }
    }
}
