using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.Shared.Requests
{
    public class DebtSoldOnToAdHocCreditorRequest : CustomAddress
    {
        [JsonIgnore]
        public Guid DebtId { get; set; }
        [JsonIgnore]
        public Guid CreditorId { get; set; }
        [Required]
        public string CreditorName { get; set; }
        [JsonIgnore]
        public override Guid OwnerId { get; set; }
        [JsonIgnore]
        public override DateTime? DateFrom { get; set; }
        [JsonIgnore]
        public override DateTime? DateTo { get; set; }

        public override IDictionary<string, object> ToDictionary()
        {
            var dictionary = ToNonIdAndDateDictionary();
            dictionary.Add("CreditorName", CreditorName);
            dictionary.Add("CreditorIdIn", CreditorId.ToString());
            return dictionary;
        }
    }
}
