using System;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class Business
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public CurrentAddress TradingAddress { get; set; }

        public BusinessAddressResponse ToBusinessAddress()
        {
            return new BusinessAddressResponse
            {
                Id = Id,
                BusinessName = Name,
                Address = TradingAddress.ToAddressResponse()
            };
        }
    }
}