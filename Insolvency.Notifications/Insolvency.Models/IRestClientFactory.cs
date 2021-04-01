using RestSharp;

namespace Insolvency.Models
{
    public interface IRestClientFactory
    {
        IRestClient CreateClient(string baseUrl);
        IRestRequest CreateRequest(string url, Method method);
    }
}
