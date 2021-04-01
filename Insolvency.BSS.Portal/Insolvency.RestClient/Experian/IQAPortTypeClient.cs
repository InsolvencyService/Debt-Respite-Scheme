using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Insolvency.RestClient.Experian
{
    public interface IQAPortTypeClient
    {
        Task<DoSearchResponse> DoSearchAsync(DoSearchRequest request);
        Task<DoGetAddressResponse> DoGetAddressAsync(DoGetAddressRequest request);
    }
}
