using System.Linq;
using System.Threading.Tasks;
using Insolvency.Interfaces;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models;

namespace Insolvency.Portal.Gateways
{
    public class AddressLookupGateway : IAddressLookupGateway
    {
        public IAddressLookupClient LookupClient { get; }

        public AddressLookupGateway(IAddressLookupClient lookupClient)
        {
            this.LookupClient = lookupClient;
        }

        public async Task<AddressLookupResult> GetAddressesForPostcode(string postcode)
        {
            var lookupResult = await this.LookupClient.GetDataAsync(postcode);
            var result = new AddressLookupResult
            {
                IsValid = !lookupResult.IsInvalidSearch,
                Errors = new[] { lookupResult.Error },
                AddressesFound = lookupResult.AddressesFound,
                Addresses = lookupResult.Addresses?.Select(x =>
                    new PartialAddress
                    {
                        Address = x.Address,
                        Id = x.AddressUrl
                    })
                .ToList()
            };
            return result;
        }

        public async Task<Address> GetFullAddressAsync(string id)
        {
            var address = await this.LookupClient.FullAddress(id);
            var result = new Address
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                County = address.County,
                Postcode = address.Postcode,
                TownCity = address.TownCity
            };
            return result;
        }
    }
}