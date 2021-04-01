using RestSharp;

namespace Insolvency.RestClient
{
    public interface IRestClientFactory
    {
        IRestClient CreateClient(string baseUrl);
        IRestRequest CreateRequest(string url, Method method);
    }
}
