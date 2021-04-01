using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class NominatedContactUpdateRequest : AbstractDebtorNominatedContactAddress<NominatedContactAddress>
    {
        [JsonIgnore]
        public Guid ContactId { get; set; }

        [JsonIgnore]
        public Guid MoratoriumId { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                { "PointOfContactId", ContactId.ToString()},
                { "FullName", FullName },
                { "CommunicationsChannel", (int)ContactConfirmationMethod.Value},
                { "TelephoneNumber", TelephoneNumber},
                { "EmailAddress", EmailAddress }
            };

            var addressDictionary = Address?.ToNonIdAndDateDictionary();
            if (addressDictionary != null)
            {
                foreach (var item in addressDictionary)
                {
                    dict.Add(item.Key, item.Value);
                }
            }

            return dict;
        }

        [JsonIgnore]
        public override Dictionary<string, Func<bool>> Actions
        {
            get
            {
                var result = base.Actions;
                result.Add("Never", () => false);
                return result;
            }
        }

        [JsonIgnore]
        [MultiConditionalRequired("Never", ErrorMessage = "Please enter confirm email address")]
        [Compare(nameof(ConfirmEmailAddress), ErrorMessage = "The email addresses you entered do not match")]
        public override string ConfirmEmailAddress { get => null; set { return; } }
    }
}