using System.Collections.Generic;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class AddressSearchResponse
    {
        public IEnumerable<RemoteAddress> Addresses { get; set; }
        public int AddressesFound { get; set; }
        public bool IsInvalidSearch { get; set; }
        public string Error { get; set; }
    }
}
