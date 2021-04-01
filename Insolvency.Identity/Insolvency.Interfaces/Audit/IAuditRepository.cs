using System.Threading.Tasks;
using Insolvency.Models.Audit;

namespace Insolvency.Interfaces.Audit
{
    public interface IAuditRepository
    {
        Task StoreAsync(AuditMessage message);
    }
}
