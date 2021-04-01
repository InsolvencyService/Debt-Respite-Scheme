using RestSharp;

namespace Insolvency.Notifications.BSSRegisterStatusUpdate
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient CreateClient(string baseUrl)
        {
            return new RestSharp.RestClient(baseUrl);
        }

        public IRestRequest CreateRequest(string url, Method method)
        {
            return new RestRequest(url, method, DataFormat.Json);
        }
    }
}
