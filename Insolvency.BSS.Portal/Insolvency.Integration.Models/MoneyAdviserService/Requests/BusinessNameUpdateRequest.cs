using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class BusinessNameUpdateRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }
        [JsonIgnore]
        public Guid BusinessId { get; set; }

        [Required(ErrorMessage = "Enter Business name")]
        public string BusinessName { get; set; }

        public virtual Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                { "BusinessId", BusinessId.ToString() },
                { "BusinessName", BusinessName }
            };
            return dictionary;
        }
    }
}
