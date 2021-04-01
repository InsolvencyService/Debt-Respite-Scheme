using Microsoft.Extensions.Logging;
using RestSharp;
using System.Threading.Tasks;
using System.Net;
using System;

namespace Insolvency.RestClient
{
    public static class RestSharpExtention
    {
        public static async Task<IRestResponse<T>> ExecuteWithLogsAsync<T>(this IRestClient client, IRestRequest request, ILogger logger)
        {
            var response = await client.ExecuteAsync<T>(request);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogError($"REST call failed for => {response.ResponseUri.AbsoluteUri} {response.Content}");
            }
            return response;
        }
    }
}
