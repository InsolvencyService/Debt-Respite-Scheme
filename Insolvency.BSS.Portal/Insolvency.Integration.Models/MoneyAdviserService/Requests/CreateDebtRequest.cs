using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class CreateDebtRequest
    {
        private const string _ninErrorMessage = "Enter a National Insurance number in the correct format. It’s on your National Insurance card, benefit letter, payslip or P60. For example, ‘QQ 12 34 56 C’.";

        [JsonIgnore]
        public virtual Guid MoratoriumId { get; set; }

        [Required]
        public virtual Guid? CreditorId { get; set; }
        public virtual Guid? DebtTypeId { get; set; }
        public string DebtTypeName { get; set; }

        [MaxLength(100)]
        public string Reference { get; set; }

        [MaxLength(13, ErrorMessage = _ninErrorMessage)]
        [RegularExpression(
            Constants.UkNinRegex,
            ErrorMessage = _ninErrorMessage,
            MatchTimeoutInMilliseconds = 3000)]
        public string NINO { get; set; }

        public decimal? Amount { get; set; }
        public virtual Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                { "MoratoriumId", MoratoriumId },
                { "Amount", Amount },
                { "CreditorId", CreditorId },
                { "DebtTypeId", DebtTypeId },
                { "NIN", NINO },
                { "Reference", Reference },
                { "OtherDebtType", DebtTypeName },
                { "SetDebtStatus", false }
            };
            return dictionary;
        }
    }
}
