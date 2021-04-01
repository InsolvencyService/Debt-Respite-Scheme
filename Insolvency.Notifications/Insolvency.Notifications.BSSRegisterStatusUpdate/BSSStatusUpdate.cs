using System.Linq;
using System.Threading.Tasks;
using Insolvency.Notifications.BSS.Models;
using Insolvency.Notifications.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Insolvency.Notifications.BSSRegisterStatusUpdate
{
    public class BSSStatusUpdate
    {
        public IRestClient Client { get; }
        public AuthorityDetails AuthDetails { get; }
        public IRestClientFactory RestFactory { get; }

        public BSSStatusUpdate(IRestClient client, IRestClientFactory restClientFactory, AuthorityDetails authDetails)
        {
            this.Client = client;
            this.AuthDetails = authDetails;
            this.RestFactory = restClientFactory;
        }

        [FunctionName("BSSStatusUpdate")]
        public async Task Run([ServiceBusTrigger("%Topic%", "%Subscription%", Connection = "SubscriptionConnection")] string mySbMsg, ILogger log)
        {
            var bssMessage = this.GetBSSMessage(mySbMsg);
            var token = this.GetToken();
            var request = RestFactory.CreateRequest("ntt_BSSUpdateNotificationStatus", Method.POST);
            request.AddJsonBody(bssMessage);
            request.AddHeader("Authorization", $"{token.token_type} {token.access_token}");
            request.AddHeader("OData-Version", "4.0");
            request.AddHeader("OData-MaxVersion", "4.0");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("If-None-Match", null);
            request.AddHeader("Content-Type", "application/json");
            var response = await Client.ExecuteAsync(request);
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {response.Content}");
        }

        protected virtual TokenResponse GetToken()
        {
            var client = RestFactory.CreateClient(AuthDetails.ClientUrl);
            var request = RestFactory.CreateRequest("token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("resource", AuthDetails.ResourceUrl);
            client.Authenticator = new HttpBasicAuthenticator(AuthDetails.ClientId, AuthDetails.ClientSecret);
            var response = client.Execute<TokenResponse>(request);
            return response?.Data;
        }

        protected virtual BSSStatusUpdateMessage GetBSSMessage(string mySbMsg)
        {
            var message = JsonConvert.DeserializeObject<StatusUpdateMessage>(mySbMsg);

            var updates = message.Updates
                .Select(x => new NotificationUpdate
                {
                    notificationId = x.Id,
                    notificationType = x.Type,
                    notifyStatus = x.MapToDynamicsStatus()
                });
            var bssMessage = new BSSStatusUpdateMessage {
                Notifications = JsonConvert.SerializeObject( new BSSStatusUpdateNotification { notificationUpdates = updates } )};
            return bssMessage;
        }
    }
}