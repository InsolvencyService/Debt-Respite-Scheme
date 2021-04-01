using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationClientDetailsCreate : ClientDetailsCreateRequest
    {
        [Required]
        public bool? IsMentalHealth { get; set; }
        [Required]
        public bool? AddressIsHidden { get; set; }

        public override Dictionary<string, object> ToDictionary()
        {
            var dictionary = base.ToDictionary();
            dictionary.Add("AddressIsHidden", AddressIsHidden.Value);
            dictionary.Add("IsMentalHealth", IsMentalHealth.Value);
            return dictionary;
        }
    }
}
