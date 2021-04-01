using Insolvency.Data.IdentityManagement;
using Insolvency.Data.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Identity.Extensions
{
    public static class DataLayerConfigurationExtensions
    {
        public static void UseDataLayer(this IApplicationBuilder app)
        {
            using var initScope = app.ApplicationServices.CreateScope();
            var serviceProvider = initScope.ServiceProvider;

            serviceProvider.InitialiseIdentityServerStores();
            serviceProvider.InitialiseIdentityManagementContext();           
        }
    }
}
