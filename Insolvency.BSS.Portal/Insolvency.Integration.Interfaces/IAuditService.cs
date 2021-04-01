using System.Threading.Tasks;
using Insolvency.Integration.Interfaces.Models;

namespace Insolvency.Integration.Interfaces
{
    public interface IAuditService
    {
        Task PerformAuditing(AuditDetail auditDetail);
    }
}
