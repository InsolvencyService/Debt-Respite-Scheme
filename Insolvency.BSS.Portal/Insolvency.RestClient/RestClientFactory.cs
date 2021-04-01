using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using System.Text.Json;

namespace Insolvency.RestClient
{
    public class RestClientFactory : IRestClientFactory
    {
        private readonly static JsonSerializerOptions JsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };

        public IRestClient CreateClient(string baseUrl)
        {
            var client = new RestSharp.RestClient(baseUrl);           
            client.UseSystemTextJson(JsonOptions);
            return client;
        }

        public IRestRequest CreateRequest(string url, Method method)
        {
            return new RestRequest(url, method, DataFormat.Json);
        }        
    }
}
