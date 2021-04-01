using System.Linq;
using System.Threading.Tasks;

using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Interfaces;
using Insolvency.RestClient.Experian;

namespace Insolvency.RestClient.Experian
{
    public class AddressLookupClient : IAddressLookupClient
    {
        public IQAPortTypeClient Client { get; }

        public AddressLookupClient(IQAPortTypeClient client) => Client = client;

        public async Task<AddressSearchResponse> GetDataAsync(string postcode)
        {
            var request = GetRequest(postcode);

            var response = await Client.DoSearchAsync(request);
            if (response.QAInformation.CreditsUsed == 0)
            {
                return new AddressSearchResponse { IsInvalidSearch = true, Error = response.QAInformation.StateTransition };
            }

            var addresses = response.QASearchResult.QAPicklist.PicklistEntry
                .Select(x => new RemoteAddress
                {
                    Address = x.PartialAddress,
                    AddressUrl = x.Moniker
                })
                .ToList();
            return new AddressSearchResponse { Addresses = addresses, AddressesFound = addresses.Count };
        }

        public async Task<CustomAddress> FullAddress(string url)
        {
            var request = new DoGetAddressRequest
            {
                QAGetAddress = new QAGetAddress { Moniker = url, Layout = "QADefault" }
            };
            var response = await Client.DoGetAddressAsync(request);

            var address = response.Address.QAAddress.MapToCustomAddress();
            return address;
        }

        protected virtual DoSearchRequest GetRequest(string postcode)
        {
            return new DoSearchRequest
            {
                QASearch = new QASearch
                {
                    Country = "GBR",
                    Search = postcode,
                    Layout = "QADefault",
                    Engine = new EngineType { Value = EngineEnumType.Singleline }
                }
            };
        }
    }
}
