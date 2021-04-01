using System.Text.Json;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;

namespace Insolvency.Models
{
    public class RestClientFactory : IRestClientFactory
    {
        private readonly static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public IRestClient CreateClient(string baseUrl)
        {
            var client = new RestClient(baseUrl);
            client.UseSystemTextJson(JsonOptions);
            return client;
        }

        public IRestRequest CreateRequest(string url, Method method)
        {
            return new RestRequest(url, method, DataFormat.Json);
        }
    }
}
