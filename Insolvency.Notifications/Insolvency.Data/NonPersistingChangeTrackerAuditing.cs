using System;
using System.Threading;
using System.Threading.Tasks;
using Insolvency.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Insolvency.Data
{
    public class NonPersistingChangeTrackerAuditing : IChangeTrackerAuditing
    {
        public async Task<int> SaveWithAuditTrackingAsync(ChangeTracker changeTracker, Func<CancellationToken, Task<int>> saveChangesAsync, CancellationToken cancellationToken)
        {
            return await saveChangesAsync(cancellationToken);
        }
    }
}
