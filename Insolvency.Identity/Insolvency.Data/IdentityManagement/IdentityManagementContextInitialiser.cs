using System;
using Insolvency.Data.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Data.IdentityManagement
{
    public static class IdentityManagementContextInitialiser
    {
        public static void InitialiseIdentityManagementContext(this IServiceProvider services)
        {
            var identityManagementContext = services.GetService<IdentityManagementContext>();
            identityManagementContext.Database.Migrate();
            IdentityServerSeed.SeedConfigurationData(identityManagementContext);
        }
    }
}
