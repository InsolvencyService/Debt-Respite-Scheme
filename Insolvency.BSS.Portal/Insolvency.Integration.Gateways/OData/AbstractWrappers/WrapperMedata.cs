using System.Collections.Generic;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways.OData.AbstractWrappers
{
    public abstract class WrapperMedata : IMetadata
    {
        public IMetadata BaseMetadata { get; }
        public WrapperMedata(IMetadata metadata) => BaseMetadata = metadata;

        public virtual bool EntityCollectionRequiresOptimisticConcurrencyCheck(string collectionName) => BaseMetadata.EntityCollectionRequiresOptimisticConcurrencyCheck(collectionName);
        public virtual string GetActionFullName(string actionName) => BaseMetadata.GetActionFullName(actionName);
        public virtual EntityCollection GetActionReturnCollection(string functionName) => BaseMetadata.GetActionReturnCollection(functionName);
        public virtual IEnumerable<IEnumerable<string>> GetAlternateKeyPropertyNames(string collectionName) => BaseMetadata.GetAlternateKeyPropertyNames(collectionName);
        public virtual IEnumerable<string> GetDeclaredKeyPropertyNames(string collectionName) => BaseMetadata.GetDeclaredKeyPropertyNames(collectionName);
        public virtual EntityCollection GetDerivedEntityCollection(EntityCollection baseCollection, string entityTypeName) => BaseMetadata.GetDerivedEntityCollection(baseCollection, entityTypeName);
        public virtual EntityCollection GetEntityCollection(string collectionPath) => BaseMetadata.GetEntityCollection(collectionPath);
        public virtual string GetEntityCollectionExactName(string collectionName) => BaseMetadata.GetEntityCollectionExactName(collectionName);
        public virtual string GetEntityTypeExactName(string collectionName) => BaseMetadata.GetEntityTypeExactName(collectionName);
        public virtual string GetFunctionFullName(string functionName) => BaseMetadata.GetFunctionFullName(functionName);
        public virtual EntityCollection GetFunctionReturnCollection(string functionName) => BaseMetadata.GetFunctionReturnCollection(functionName);
        public virtual string GetFunctionVerb(string functionName) => BaseMetadata.GetFunctionVerb(functionName);
        public virtual string GetLinkedCollectionName(string instanceTypeName, string typeName, out bool isSingleton) => BaseMetadata.GetLinkedCollectionName(instanceTypeName, typeName, out isSingleton);
        public virtual string GetNavigationPropertyExactName(string collectionName, string propertyName) => BaseMetadata.GetNavigationPropertyExactName(collectionName, propertyName);
        public virtual IEnumerable<string> GetNavigationPropertyNames(string collectionName) => BaseMetadata.GetNavigationPropertyNames(collectionName);
        public virtual string GetNavigationPropertyPartnerTypeName(string collectionName, string propertyName) => BaseMetadata.GetNavigationPropertyPartnerTypeName(collectionName, propertyName);
        public virtual string GetQualifiedTypeName(string typeOrCollectionName) => BaseMetadata.GetQualifiedTypeName(typeOrCollectionName);
        public virtual string GetStructuralPropertyExactName(string collectionName, string propertyName) => BaseMetadata.GetStructuralPropertyExactName(collectionName, propertyName);
        public virtual IEnumerable<string> GetStructuralPropertyNames(string collectionName) => BaseMetadata.GetStructuralPropertyNames(collectionName);
        public virtual string GetStructuralPropertyPath(string collectionName, params string[] propertyNames) => BaseMetadata.GetStructuralPropertyPath(collectionName, propertyNames);
        public virtual bool HasNavigationProperty(string collectionName, string propertyName) => BaseMetadata.HasNavigationProperty(collectionName, propertyName);
        public virtual bool HasStructuralProperty(string collectionName, string propertyName) => BaseMetadata.HasStructuralProperty(collectionName, propertyName);
        public virtual bool IsNavigationPropertyCollection(string collectionName, string propertyName) => BaseMetadata.IsNavigationPropertyCollection(collectionName, propertyName);
        public virtual bool IsOpenType(string collectionName) => BaseMetadata.IsOpenType(collectionName);
        public virtual bool IsTypeWithId(string typeName) => BaseMetadata.IsTypeWithId(typeName);
        public virtual EntityCollection NavigateToCollection(string path) => BaseMetadata.NavigateToCollection(path);
        public virtual EntityCollection NavigateToCollection(EntityCollection rootCollection, string path) => BaseMetadata.NavigateToCollection(rootCollection, path);
        public virtual EntryDetails ParseEntryDetails(string collectionName, IDictionary<string, object> entryData, string contentId = null) => BaseMetadata.ParseEntryDetails(collectionName, entryData, contentId);
    }
}
