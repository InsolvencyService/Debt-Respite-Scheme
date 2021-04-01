using Microsoft.AspNetCore.Builder;

namespace Insolvency.Notifications.API.OrganisationSelector
{
    public static class OrganisationSelectorMiddlewareExtensions
    {
        public static IApplicationBuilder UseOrganisationSelector(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OrganisationSelectorMiddleware>();
        }
    }
}
