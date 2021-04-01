using System.Collections.Generic;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models
{
    public abstract class AbstractDebtorNominatedContactAddress<T> : AbstractDebtorNominatedContact, IMultiConditionalRequiredValidation where T : CustomAddress
    {
        [MultiConditionalRequired("Address")]
        public T Address { get; set; }

        public override IDictionary<string, object> ToDictionary(DynamicsGatewayOptions options)
        {
            var dictionary = base.ToDictionary(options);
            var addressDictionary = Address?.ToDictionary();
            if (addressDictionary != null)
            {
                foreach (var item in addressDictionary)
                {
                    dictionary.Add(item);
                }
            }
            return dictionary;
        }
    }
}
