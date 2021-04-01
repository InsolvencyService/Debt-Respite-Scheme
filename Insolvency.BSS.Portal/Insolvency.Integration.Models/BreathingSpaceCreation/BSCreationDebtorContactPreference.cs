using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationDebtorContactPreference : DebtorContactPreferenceRequest
    {
        [JsonIgnore]
        public override Guid MoratoriumId { get; set; }

        [JsonIgnore]
        public override bool ConditionalFlag => base.ConditionalFlag;

        [Required]
        [EnumDataType(typeof(ContactPreferenceType))]
        public override ContactPreferenceType? Preference { get; set; }
    }
}
