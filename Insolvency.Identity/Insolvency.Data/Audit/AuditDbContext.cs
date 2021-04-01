using Insolvency.Models.Audit;
using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.Audit
{
    public class AuditDbContextFactory : ContextFactory<AuditDbContext>
    {
        protected override string ConnectionStringConfigurationKey => "AuditDbConnection";

        public AuditDbContextFactory()
        {
        }

        public AuditDbContextFactory(string settingsFileName): base (settingsFileName)
        {
        }

        protected override AuditDbContext GetContext(DbContextOptions<AuditDbContext> options)
        {
            return new AuditDbContext(options);
        }
    }
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AuditMessage> AuditMessage { get; set; }
    }
}
