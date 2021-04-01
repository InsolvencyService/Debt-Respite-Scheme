using Microsoft.AspNetCore.Builder;

namespace Insolvency.IntegrationAPI.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseOrganisationSelector(this IApplicationBuilder builder)
            => builder.UseMiddleware<OrganisationSelectorMiddleware>();
    }
}
