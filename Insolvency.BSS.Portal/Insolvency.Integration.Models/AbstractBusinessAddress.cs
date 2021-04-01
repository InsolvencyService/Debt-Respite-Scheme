using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Integration.Models
{
    public abstract class AbstractBusinessAddress<T> where T : CustomAddress
    {
        [Required]
        public string BusinessName { get; set; }
        public virtual T Address { get; set; }

        public virtual IDictionary<string, object> ToDictionary(Guid debtorId)
        {
            var addressDictionary = ToDictionary();
            addressDictionary.Add(new KeyValuePair<string, object>("OwnerID", debtorId));
            return addressDictionary;
        }

        public virtual IDictionary<string, object> ToDictionary()
        {
            var addressDictionary = Address.ToDictionary();
            addressDictionary.Add(new KeyValuePair<string, object>("BusinessName", BusinessName));
            return addressDictionary;
        }
    }
}
