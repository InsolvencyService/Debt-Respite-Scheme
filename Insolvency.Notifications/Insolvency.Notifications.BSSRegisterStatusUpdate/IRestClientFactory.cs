using RestSharp;

namespace Insolvency.Notifications.BSSRegisterStatusUpdate
{
    public interface IRestClientFactory
    {
        IRestClient CreateClient(string baseUrl);
        IRestRequest CreateRequest(string url, Method method);
    }
}