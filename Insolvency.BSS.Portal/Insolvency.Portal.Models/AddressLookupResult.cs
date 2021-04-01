using System.Collections.Generic;

namespace Insolvency.Portal.Models
{
    public class AddressLookupResult
    {
        public IEnumerable<PartialAddress> Addresses { get; set; }
        public bool IsValid { get; set; }
        public int AddressesFound { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
