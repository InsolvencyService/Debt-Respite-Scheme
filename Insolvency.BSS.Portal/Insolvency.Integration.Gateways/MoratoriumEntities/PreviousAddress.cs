using Insolvency.Common;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class PreviousAddress : CurrentAddress
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public PreviousAddressResponse ToPreviousAddress()
        {
            return new PreviousAddressResponse
            {
                AddressId = Id,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                TownCity = AddressLine3,
                County = AddressLine4,
                Postcode = Postcode,
                Country = Country,
                DateFrom = DateFrom.ToDateTimeOffset().DateTime,
                DateTo = DateTo.ToDateTimeOffset().DateTime
            };
        }
    }
}