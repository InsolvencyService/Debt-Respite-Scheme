using System;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class BusinessAddressViewModel
    {
        public BusinessAddressViewModel()
        {
        }

        public BusinessAddressViewModel(
            string businessName,
            AddressResponse businessAddress,
            AddressResponse residentAddress,
            bool hideResidentAddress,
            Guid businessId
        )
        {
            BusinessId = businessId;
            BusinessName = businessName;
            BusinessAddress = new Address(businessAddress);
            ResidentAddress = new Address(residentAddress);
            HideResidentAddress = hideResidentAddress;
            HideBusinessAddress = HideResidentAddress ? BusinessAddress.Equals(ResidentAddress) : false;
        }

        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; }
        public Address BusinessAddress { get; set; }
        public bool HideResidentAddress { get; set; }
        public Address ResidentAddress { get; set; }
        public bool HideBusinessAddress { get; private set; }
        public bool ShouldDisplayBusinessName => !string.IsNullOrWhiteSpace(BusinessName);
        public bool ShouldDisplayBusinessAddress => BusinessAddress != null && !HideBusinessAddress;
    }
}
