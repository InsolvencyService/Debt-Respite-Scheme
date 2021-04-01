using System;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class CurrentAddress
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }

        public AddressResponse ToAddressResponse()
        {
            return new AddressResponse
            {
                AddressId = Id,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                TownCity = AddressLine3,
                County = AddressLine4,
                Postcode = Postcode,
                Country = Country
            };
        }
    }
}