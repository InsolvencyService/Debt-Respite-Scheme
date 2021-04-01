using System.Threading.Tasks;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Interfaces.Models;

namespace Insolvency.IntegrationAPI
{
    public class AuditService : IAuditService
    {
        public IBackgroundAuditTaskQueue BackgroundTaskQueue { get; }

        public AuditService(IBackgroundAuditTaskQueue backgroundTaskQueue)
        {
            this.BackgroundTaskQueue = backgroundTaskQueue;
        }

        public Task PerformAuditing(AuditDetail auditDetail)
        {
            BackgroundTaskQueue.QueueBackgroundAudit(auditDetail);
            return Task.CompletedTask;
        }
    }
}
