using System;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways.OData
{
    public class CustomCreteAdapterLoaderFactory : ODataAdapterFactory
    {
        public Func<ISession, IODataModelAdapter, IODataAdapter> CreateAdapterFunc { get; }

        public CustomCreteAdapterLoaderFactory(Func<ISession, IODataModelAdapter, IODataAdapter> createAdapterFunc)
            : base() => CreateAdapterFunc = createAdapterFunc;

        public override Func<ISession, IODataAdapter> CreateAdapterLoader(string metadataString, ITypeCache typeCache)
        {
            var modelAdapter = CreateModelAdapter(metadataString, typeCache);

            return x =>
            {
                return CreateAdapterFunc(x, modelAdapter);
            };
        }
    }
}
