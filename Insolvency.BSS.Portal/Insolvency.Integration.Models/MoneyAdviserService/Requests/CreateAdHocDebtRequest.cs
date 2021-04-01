using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class CreateAdHocDebtRequest : CustomAddress
    {
        private const string _ninErrorMessage = "Enter a National Insurance number in the correct format. It’s on your National Insurance card, benefit letter, payslip or P60. For example, ‘QQ 12 34 56 C’.";
       
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }

        [Required]
        public string CreditorName { get; set; }

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

        [JsonIgnore]
        [Required(ErrorMessage = "Enter MoratoriumId Id")]
        public override Guid OwnerId { get; set; }
        [JsonIgnore]
        public override DateTime? DateFrom { get; set; }
        [JsonIgnore]
        public override DateTime? DateTo { get; set; }

        public override IDictionary<string, object> ToDictionary()
        {
            var dictionary = ToNonIdAndDateDictionary();        
            dictionary.Add("MoratoriumId", MoratoriumId);
            dictionary.Add("CreditorName", CreditorName);
            dictionary.Add("Amount", Amount);
            dictionary.Add("NIN", NINO);
            dictionary.Add("Reference", Reference);
            dictionary.Add("OtherDebtType", DebtTypeName);

            return dictionary;
        }
    }
}
