using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.IdentityServer
{
    public class IdentityServerOperationalStoreFactory : ContextFactory<PersistedGrantDbContext>
    {
        protected override string ConnectionStringConfigurationKey => "IdentityConnection";

        protected override PersistedGrantDbContext GetContext(DbContextOptions<PersistedGrantDbContext> options)
        {
            var storeOptions = new OperationalStoreOptions();
            return new PersistedGrantDbContext(options, storeOptions);
        }
    }
}
