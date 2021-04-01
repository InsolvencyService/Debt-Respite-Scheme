using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class CreditorSearchDetailedResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsGovermentCreditor { get; set; }

        public IEnumerable<DebtType> DebtTypes { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
