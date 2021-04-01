using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Models
{
    public class Creditor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsGovermentCreditor { get; set; }
        public IList<DebtType> DebtTypes { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string County { get; set; }
        public string TownCity { get; set; }
        public string PostCode { get; set; }
        public Guid AddressId { get; set; }
        public string Country { get; set; }
    }
}
