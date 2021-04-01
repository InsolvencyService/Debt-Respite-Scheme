using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Interfaces.Models;

namespace Insolvency.IntegrationAPI.HostedService
{
    public class BackgroundTaskQueue : IBackgroundAuditTaskQueue
    {
        private ConcurrentQueue<string> WorkItems { get; set; } = new ConcurrentQueue<string>();
        private SemaphoreSlim signal = new SemaphoreSlim(0);

        public void QueueBackgroundAudit(AuditDetail auditDetails)
        {
            if (auditDetails == null)
            {
                throw new ArgumentNullException(nameof(auditDetails));
            }

            WorkItems.Enqueue(JsonSerializer.Serialize(auditDetails));
            signal.Release();
        }

        public async Task<AuditDetail> DequeueAsync(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            WorkItems.TryDequeue(out var workItem);

            return JsonSerializer.Deserialize<AuditDetail>(workItem);
        }
    }
}
