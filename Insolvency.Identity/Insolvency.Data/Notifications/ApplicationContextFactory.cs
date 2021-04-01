using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.Notifications
{
    public class OrganisationContextFactory : ContextFactory<ApplicationContext>
    {
        protected override string ConnectionStringConfigurationKey => "DevConnection";

        protected override ApplicationContext GetContext(DbContextOptions<ApplicationContext> options)
        {
            return new ApplicationContext(options);
        }
    }
}
