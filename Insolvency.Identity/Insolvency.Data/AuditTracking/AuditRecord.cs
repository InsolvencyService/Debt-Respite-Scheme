using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Insolvency.Data.AuditTracking
{
    public class AuditRecord
    {
        public string Name { get; set; }
        public string RecordId { get; set; }
        public OperationType OperationType  { get; set; }
        public List<AuditProperty> Properties { get; set; }

        public AuditRecord()
        { }

        public AuditRecord(EntityEntry entityEntry, string id, bool filterModified, OperationType operationType)
        {

            Name = entityEntry.Entity.GetType().FullName;
            RecordId = id;
            Properties = entityEntry.Properties
                .Where(x => !filterModified || x.IsModified)
                .Select(x => new AuditProperty(x))
                .ToList();
            OperationType = operationType;
        }
    }
}
