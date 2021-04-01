using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Insolvency.Interfaces.Notifications;
using Insolvency.Models.Audit;

namespace Insolvency.Audit.MessageInsert.Function
{
    public class InssAuditMessaging
    {
        private readonly IMessagingGateway _messagingGateway;

        public InssAuditMessaging(IMessagingGateway messagingGateway)
        {
            _messagingGateway = messagingGateway;
        }

        [FunctionName("InssAuditMessaging")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var auditDetail = JsonConvert.DeserializeObject<AuditDetail>(reqBody);

            var auditMessage = new AuditMessage
            {
                CreatedOn = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Email = auditDetail.Email,
                ActionName = auditDetail.ActionName,
                ControllerName = auditDetail.ControllerName,
                SenderName = auditDetail.SenderName,
                Name = auditDetail.Name,
                ClientId = auditDetail.ClientId,
                OrganisationId = auditDetail.OrganisationId,
                PayLoad = JsonConvert.SerializeObject(auditDetail.Content)
            };

            string messageBody = JsonConvert.SerializeObject(auditMessage);
            
            await _messagingGateway.SendMessageAsync(Encoding.UTF8.GetBytes(messageBody));

            log.LogInformation("C# HTTP trigger InssAuditMessaging function processed a request.");

            return new OkResult();
        }
    }
}
