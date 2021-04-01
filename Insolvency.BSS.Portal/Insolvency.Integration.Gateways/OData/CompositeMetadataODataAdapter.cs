using System;
using Insolvency.Integration.Gateways.OData.AbstractWrappers;
using Simple.OData.Client;
using Simple.OData.Client.V4.Adapter;

namespace Insolvency.Integration.Gateways.OData
{
    public class CompositeMetadataODataAdapter : ODataAdapter
    {
        public IODataAdapter BaseDataAdapter { get; }

        public Func<IMetadata, WrapperMedata>[] CompositeMetadataFuctions { get; }

        public CompositeMetadataODataAdapter(ISession session, IODataModelAdapter modelAdapter,
            params Func<IMetadata, WrapperMedata>[] compositeMetadataFuctions)
            : base(session, modelAdapter) => CompositeMetadataFuctions = compositeMetadataFuctions;

        public override IMetadata GetMetadata()
        {
            var result = base.GetMetadata();
            foreach (var metadataFunction in CompositeMetadataFuctions)
            {
                result = metadataFunction(result);
            }
            return result;
        }
    }
}
