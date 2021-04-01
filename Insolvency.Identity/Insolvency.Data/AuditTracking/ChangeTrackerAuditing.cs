using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Insolvency.Interfaces;
using Insolvency.Models;
using Insolvency.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Insolvency.Data.AuditTracking
{
    public class ChangeTrackerAuditing : IChangeTrackerAuditing
    {
        public IAuditService AuditService { get; }
        public Func<AuditDetail> AuditDetailProvider { get; }
        public ILogger<IAuditService> Logger { get; }

        public ChangeTrackerAuditing(IAuditService auditService, ILogger<IAuditService> logger, Func<AuditDetail> auditDetailProvider)
        {
            this.AuditService = auditService;
            this.AuditDetailProvider = auditDetailProvider;
            this.Logger = logger;
        }

        public async Task<int> SaveWithAuditTrackingAsync(ChangeTracker changeTracker, Func<CancellationToken, Task<int>> saveChangesAsync, CancellationToken cancellationToken)
        {
            int result;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entityContents = new List<AuditRecord>();
                    var addedEntries = GetAddedEntries(changeTracker);
                    entityContents.AddRange(GetDeletedAuditRecords(changeTracker));
                    entityContents.AddRange(GetModifiedAuditRecords(changeTracker));
                    result = await saveChangesAsync(cancellationToken);
                    entityContents.AddRange(GetAddedAuditRecords(addedEntries));
                    var auditDetail = AuditDetailProvider();

                    if (entityContents.Count > 0)
                    {
                        auditDetail.Content = new Dictionary<string, object> { { "efSaveChanges", entityContents } };
                        await AuditService.PerformAuditing(auditDetail);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"An error occurred while saving Identity DB Changes: {ex.Message}");
                    throw;
                }
            }
            return result;
        }

        private IEnumerable<EntityEntry> GetAddedEntries(ChangeTracker changeTracker)
        {
            return changeTracker.Entries()
                .Where(x => x.State == EntityState.Added)
                .ToList();
        }

        private IEnumerable<AuditRecord> GetAddedAuditRecords(IEnumerable<EntityEntry> entries)
        {
            var records = GetAuditRecords(entries, OperationType.Added);
            return records;
        }

        private IEnumerable<AuditRecord> GetDeletedAuditRecords(ChangeTracker changeTracker)
        {
            var deletedEntries = changeTracker.Entries()
                .Where(x => x.State == EntityState.Deleted)
                .ToList();

            var records = GetAuditRecords(deletedEntries, OperationType.Deleted);

            return records;
        }

        private IEnumerable<AuditRecord> GetModifiedAuditRecords(ChangeTracker changeTracker)
        {
            var updatedEntries = changeTracker.Entries()
                .Where(x => x.State == EntityState.Modified)
                .ToList();

            var records = GetAuditRecords(updatedEntries, OperationType.Updated, true);

            return records;
        }

        private IEnumerable<AuditRecord> GetAuditRecords(IEnumerable<EntityEntry> entries, OperationType operationType, bool filterModified = false)
        {
            var updatedProperties = entries
                .Select(x => new AuditRecord(x, GetId(x), filterModified, operationType))
                .ToList();
            return updatedProperties;
        }

        private string GetId(EntityEntry entry)
        {
            var pk = entry.Metadata.FindPrimaryKey();
            var values = new List<string>();
            foreach (var property in pk.Properties)
            {
                var getter = property.GetGetter();
                var value = getter.GetClrValue(entry.Entity);
                values.Add($"{property.Name}-{value}");
            }
            return values.Aggregate((x, y) => $"{x}||{y}");
        }
    }
}
