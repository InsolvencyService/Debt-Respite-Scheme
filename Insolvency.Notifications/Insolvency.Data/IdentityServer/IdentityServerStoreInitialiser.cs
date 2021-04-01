using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Data.IdentityServer
{
    public static class IdentityServerStoreInitialiser
    {
        public static void InitialiseIdentityServerStores(this IServiceProvider services)
        {
            PersistedGrantDbContext persistedGrantDbContext = services.GetService<PersistedGrantDbContext>();

            persistedGrantDbContext.Database.Migrate();                
        }
    }
}
