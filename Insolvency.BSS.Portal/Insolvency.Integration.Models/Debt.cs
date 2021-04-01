using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Insolvency.Common;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models
{
    public class Debt
    {
        private const string _ninErrorMessage = "Enter a National Insurance number in the correct format. It’s on your National Insurance card, benefit letter, payslip or P60. For example, ‘QQ 12 34 56 C’.";

        public virtual Guid Id { get; set; }
        public virtual Guid? DebtTypeId { get; set; }

        [MaxLength(100)]
        public string Reference { get; set; }

        [MaxLength(13, ErrorMessage = _ninErrorMessage)]
        [RegularExpression(
            Constants.UkNinRegex,
            ErrorMessage = _ninErrorMessage,
            MatchTimeoutInMilliseconds = 3000)]
        public string NINO { get; set; }
        public virtual Guid CreditorId { get; set; }
        public virtual Guid MoratoriumId { get; set; }
        public decimal? Amount { get; set; }
        public string DebtTypeName { get; set; }
        public virtual DateTimeOffset CreatedOn { get; set; }
        public virtual DateTimeOffset RemovedOn { get; set; }
        public virtual DebtStatus Status { get; set; }
        public virtual Guid? SoldToCreditorId { get; set; }
        public virtual string SoldToCreditorName { get; set; }
        public virtual bool PreviouslySold { get; set; }
        public virtual DebtRemovalReason? DebtRemovalReason { get; set; }

        public virtual Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                { "Amount", Amount },
                { "CreditorId", CreditorId },
                { "DebtTypeId", DebtTypeId },
                { "NIN", NINO },
                { "Reference", Reference },
                { "OtherDebtType", DebtTypeName }
            };
            return dictionary;
        }
    }
}
