using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models;

namespace Insolvency.Portal.Gateways
{

    public class MockAddressLookupGateway : IAddressLookupGateway
    {
        public const string AddressId1 = "FB39CA45-BDE5-4B25-85F5-624E1DD3F682";
        public const string AddressId2 = "2CD4FC41-FD41-4091-AF16-03AD8F34B61F";
        public Task<AddressLookupResult> GetAddressesForPostcode(string postcode)
        {
            var result = new AddressLookupResult();

            if (postcode == "test-invalid")
            {
                result.IsValid = false;
                result.Errors = new string[] { "Invalid Postcode" };
            }
            else if (postcode == "test-empty")
            {
                result.IsValid = true;
                result.Addresses = new List<PartialAddress>();
            }
            else
            {
                result.IsValid = true;
                result.AddressesFound = 2;
                result.Addresses = new List<PartialAddress> {
                    new PartialAddress { Address = "Apt 91, 41 Essex Street, West Midlands, Birmingham, B5 4TT", Id = AddressId1 },
                    new PartialAddress { Address = "25A Westbourne Grove, City of London, London, W2 4UA", Id = AddressId2 },
                };
            }

            return Task.FromResult(result);
        }

        public async Task<Address> GetFullAddressAsync(string id)
        {
            if (id == AddressId1)
            {
                return await Task.FromResult(new Address
                {
                    AddressLine1 = "Apt 91",
                    AddressLine2 = "41 Essex Street",
                    County = "West Midlands",
                    Postcode = "B5 4TT",
                    TownCity = "Birmingham"
                });
            }
            return await Task.FromResult(new Address
            {
                AddressLine1 = "25A Westbourne Grove",
                AddressLine2 = "",
                County = "City of London",
                Postcode = "W2 4UA",
                TownCity = "London"
            });
        }
    }
}