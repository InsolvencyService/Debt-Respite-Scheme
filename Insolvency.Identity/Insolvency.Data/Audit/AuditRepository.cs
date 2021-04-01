using System.Threading.Tasks;
using Insolvency.Interfaces.Audit;
using Insolvency.Models.Audit;

namespace Insolvency.Data.Audit
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditDbContext _context;

        public AuditRepository(AuditDbContext context)
        {
            _context = context;
        }

        public async Task StoreAsync(AuditMessage message)
        {
            _context.AuditMessage.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
