using System.Threading.Tasks;
using Insolvency.Models.Audit;

namespace Insolvency.Models
{
    public interface IAuditService
    {
        Task PerformAuditing(AuditDetail auditDetail);
    }
}