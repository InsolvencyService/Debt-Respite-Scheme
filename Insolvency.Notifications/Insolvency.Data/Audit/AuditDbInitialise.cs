using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.Audit
{
    public static class AuditDbInitialise
    {
        public static void Init(AuditDbContext context)
        {
            context.Database.Migrate();
        }
    }
}
