using System.Linq;
using Insolvency.Integration.Models;
using Insolvency.RestClient.Experian;

namespace Insolvency.RestClient.Experian
{
    public partial class QAAddressType
    {
        public CustomAddress MapToCustomAddress()
        {
            var address = new CustomAddress();
            var addressElements = this.AddressLine;
            var addressLines = Enumerable.Where<AddressLineType>(this.AddressLine, x => string.IsNullOrEmpty(x.Label)).ToList();
            address.AddressLine1 = addressLines.Count > 0 ? addressLines[0].Line : null;
            address.AddressLine2 = addressLines.Count > 1 ? addressLines[1].Line : null;
            address.TownCity = Enumerable.FirstOrDefault<AddressLineType>(addressElements, x => x.Label == "Town")?.Line;
            address.County = Enumerable.FirstOrDefault<AddressLineType>(addressElements, x => x.Label == "County")?.Line;
            address.Postcode = Enumerable.FirstOrDefault<AddressLineType>(addressElements, x => x.Label == "Postcode")?.Line;
            return address;
        }
    }
}
