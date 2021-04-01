using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class ReviewProposedDebtRequest : IConditionalRequiredValidation
    {
        [JsonIgnore]
        public Guid DebtId { get; set; }

        [Required]
        public bool? AcceptProposedDebt { get; set; }

        [ConditionalRequired]
        public string RemovalReason { get; set; }

        public bool ConditionalFlag => AcceptProposedDebt.HasValue && !AcceptProposedDebt.Value;

        public IDictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>();

            if (ConditionalFlag)
            {
                dictionary.Add("AdditionalInformation", RemovalReason);
            }

            return dictionary;
        }
    }
}
