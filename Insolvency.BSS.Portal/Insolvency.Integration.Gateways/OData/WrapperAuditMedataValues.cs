using System;
using System.Collections.Generic;
using Insolvency.Integration.Gateways.Audit;
using Insolvency.Integration.Gateways.OData.AbstractWrappers;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways.OData
{
    public class WrapperAuditMedataValues : WrapperMedata
    {
        public WrapperAuditMedataValues(IMetadata metadata, Func<AuditContext> auditContextProvider)
            : base(metadata) => AuditContextProvider = auditContextProvider;

        public Func<AuditContext> AuditContextProvider { get; }

        public override EntryDetails ParseEntryDetails(string collectionName, IDictionary<string, object> entryData, string contentId = null)
        {

            var audit = AuditContextProvider();

            entryData.SetDynamicsObjectAuditProperties(audit);

            return BaseMetadata.ParseEntryDetails(collectionName, entryData, contentId);
        }
    }
}
