using System;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class BusinessAddressResponse
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; }
        public virtual AddressResponse Address { get; set; }
    }
}
