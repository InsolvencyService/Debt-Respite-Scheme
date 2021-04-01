using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways.OData
{
    public static class ODataCustomExtensions
    {
        public static IBoundClient<TOwner> WhereInFilter<TOwner, TProperty>(
            this IBoundClient<TOwner> client,
            Expression<Func<TOwner, TProperty>> getter,
            params TProperty[] propertyValues)
            where TOwner : class
                => client.Filter(CreateWhereInFilter(getter, propertyValues));

        public static IBoundClient<TOwner> WhereInFilter<TOwner, TProperty>(
            this IBoundClient<TOwner> client,
            Expression<Func<TOwner, TProperty>> getter,
            IEnumerable<TProperty> propertyValues)
            where TOwner : class
                => client.Filter(CreateWhereInFilter(getter, propertyValues));

        private static Expression<Func<TOwner, bool>> CreateWhereInFilter<TOwner, TProperty>(
            Expression<Func<TOwner, TProperty>> getter,
            IEnumerable<TProperty> propertyValues)
        {
            var arg = getter.Parameters.First();
            var property = getter.Body;

            Expression comparisons = null;
            foreach (var value in propertyValues)
            {
                var comparison = Expression.Equal(property, Expression.Constant(value));
                if (comparisons == null)
                    comparisons = comparison;
                else
                    comparisons = Expression.Or(comparisons, comparison);
            }

            return Expression.Lambda<Func<TOwner, bool>>(comparisons, arg);
        }
    }
}
