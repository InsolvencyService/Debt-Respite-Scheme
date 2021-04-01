using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.IdentityManagement
{
    public class IdentityManagementContextFactory : ContextFactory<IdentityManagementContext>
    {
        protected override string ConnectionStringConfigurationKey => "IdentityConnection";

        protected override IdentityManagementContext GetContext(DbContextOptions<IdentityManagementContext> options)
        {
            var storeOptions = new ConfigurationStoreOptions();
            return new IdentityManagementContext(options, storeOptions, new NonPersistingChangeTrackerAuditing());
        }
    }
}