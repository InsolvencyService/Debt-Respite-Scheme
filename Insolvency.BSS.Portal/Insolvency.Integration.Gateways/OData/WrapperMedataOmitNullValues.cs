using System.Collections.Generic;
using System.Linq;
using Insolvency.Integration.Gateways.OData.AbstractWrappers;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways.OData
{
    public class WrapperMedataOmitNullValues : WrapperMedata
    {
        public WrapperMedataOmitNullValues(IMetadata metadata)
            : base(metadata)
        { }

        public override EntryDetails ParseEntryDetails(string collectionName, IDictionary<string, object> entryData, string contentId = null)
        {
            var filteredEntryData = entryData.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
            return BaseMetadata.ParseEntryDetails(collectionName, filteredEntryData, contentId);
        }
    }
}
