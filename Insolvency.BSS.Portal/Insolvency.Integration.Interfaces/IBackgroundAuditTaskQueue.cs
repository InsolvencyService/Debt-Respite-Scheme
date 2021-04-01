using System.Threading;
using System.Threading.Tasks;
using Insolvency.Integration.Interfaces.Models;

namespace Insolvency.Integration.Interfaces
{
    public interface IBackgroundAuditTaskQueue
    {
        void QueueBackgroundAudit(AuditDetail auditDetails);
        Task<AuditDetail> DequeueAsync(CancellationToken cancellationToken);
    }
}
