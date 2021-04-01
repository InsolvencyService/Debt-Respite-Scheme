using System;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class PreviousAddressResponse : AddressResponse
    {
        public virtual DateTime? DateFrom { get; set; }
        public virtual DateTime? DateTo { get; set; }
    }
}
