using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Insolvency.Interfaces
{
    public interface IChangeTrackerAuditing
    {
        Task<int> SaveWithAuditTrackingAsync(ChangeTracker changeTracker, Func<CancellationToken, Task<int>> saveChangesAsync, CancellationToken cancellationToken);
    }
}