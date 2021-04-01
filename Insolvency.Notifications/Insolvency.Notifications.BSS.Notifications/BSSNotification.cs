using System;
using System.Net.Http;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.BSS.Models;
using Insolvency.Notifications.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Insolvency.Notifications.BSS.Notifications
{

    public class BSSNotification
    {
        public IMessagingClient Client { get; }
        public BSSNotification(IMessagingClient client)
        {
            this.Client = client;
        }

        [FunctionName("BSSNotification")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage request,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var notification = await request.Content.ReadAsAsync<Notification>();

            var notificationMessage = new NotificationMessage
            {
                CreatedOn = DateTime.UtcNow,
                Id = Guid.Empty,
                SenderSystem = new Sender
                {
                    Id = notification.BusinessUnit
                },
                ExternalSenderSystemId = notification.OriginatingNotification.ToString(),
                Status = NotificationStatus.Transit,
                Type = notification.Channel,
                PayLoad = JsonConvert.SerializeObject(notification.Content)
            };

            await this.Client.SendNotificationMessageAsync(notificationMessage);

            return new AcceptedResult();
        }
    }
}
