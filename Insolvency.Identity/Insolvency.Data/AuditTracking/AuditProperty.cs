using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Insolvency.Data.AuditTracking
{
    public class AuditProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public AuditProperty()
        { }

        public AuditProperty(PropertyEntry propertyEntry)
        {
            this.Name = propertyEntry.Metadata.Name;
            this.Value = propertyEntry.CurrentValue?.ToString();
        }
    }
}
