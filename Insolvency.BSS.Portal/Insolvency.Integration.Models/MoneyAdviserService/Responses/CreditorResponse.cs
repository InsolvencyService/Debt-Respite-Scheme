using System;
using System.Collections.Generic;
using System.Text;

namespace Insolvency.Integration.Models.MoneyAdviserService.Responses
{
    public class CreditorResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsGovermentCreditor { get; set; }
        public IList<DebtType> DebtTypes { get; set; }
        public Guid AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string County { get; set; }
        public string TownCity { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        public bool HasCreditorName => !string.IsNullOrEmpty(Name);

        public string ToSingleLineString()
        {
            var builder = new StringBuilder();
            builder.Append(!string.IsNullOrEmpty(AddressLine1) ? $"{AddressLine1}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(AddressLine2) ? $"{AddressLine2}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(TownCity) ? $"{TownCity}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(County) ? $"{County}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(PostCode) ? $"{PostCode}" : string.Empty);
            var result = builder.ToString().Trim().Trim(',');
            return result;
        }
    }
}
