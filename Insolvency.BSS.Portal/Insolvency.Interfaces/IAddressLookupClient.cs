using System.Threading.Tasks;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Interfaces
{
    public interface IAddressLookupClient
    {
        Task<AddressSearchResponse> GetDataAsync(string postcode);
        Task<CustomAddress> FullAddress(string url);
    }
}
