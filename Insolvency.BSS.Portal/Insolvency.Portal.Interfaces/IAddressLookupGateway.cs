using System.Threading.Tasks;
using Insolvency.Portal.Models;

namespace Insolvency.Portal.Interfaces
{
    public interface IAddressLookupGateway
    {
        Task<AddressLookupResult> GetAddressesForPostcode(string postcode);
        Task<Address> GetFullAddressAsync(string id);
    }
}
