using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class DebtorContactPreferenceRequest : IConditionalRequiredValidation
    {
        private string _emailAddress;

        [JsonIgnore]
        public virtual Guid MoratoriumId { get; set; }
        [Required(ErrorMessage = "Please enter contact preference")]
        [EnumDataType(typeof(ContactPreferenceType))]
        public virtual ContactPreferenceType? Preference { get; set; }

        [ConditionalRequired(ErrorMessage = "Please enter email address")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [Display(Name = "Email address")]
        public string EmailAddress
        {
            get => _emailAddress?.Trim();
            set => _emailAddress = value;
        }

        public Dictionary<string, object> ToDictionary() =>
                new Dictionary<string, object>() {
                    { "Preference", (int)Preference.Value },
                    { "EmailAddress", EmailAddress }
                };

        [JsonIgnore]
        public virtual bool ConditionalFlag => Preference.HasValue && Preference.Value == ContactPreferenceType.Email;
    }
}
