using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Insolvency.Data
{
    public class VersioningEndpointMiddleware
    {
        private readonly IConfiguration _configuration;

        public VersioningEndpointMiddleware(RequestDelegate next, IConfiguration configuration) => _configuration = configuration;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var buildNumber = _configuration.GetValue<string>("ApplicationBuildVersion") ?? "Version not configured";
            await httpContext.Response.WriteAsync(buildNumber);
        }
    }
}
