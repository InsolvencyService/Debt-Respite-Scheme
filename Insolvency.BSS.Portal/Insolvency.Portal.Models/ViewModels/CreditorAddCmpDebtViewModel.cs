using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Insolvency.Common;
using Insolvency.Integration.Models;

namespace Insolvency.Portal.Models.ViewModels
{
    public class CreditorAddCmpDebtViewModel : DebtViewModel
    {
        private const string _ninErrorMessage = "Enter a National Insurance number in the correct format.\n It’s on your National Insurance card, benefit letter, payslip or P60. For example, ‘QQ 12 34 56 C’.";

        [Display(Name = "Debt Amount (optional)")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Please enter a valid amount, up to 2 decimal places", MatchTimeoutInMilliseconds = 3000)]
        [MaxLength(100)]
        public override string DebtAmount { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reference (optional)")]
        public override string Reference { get; set; }

        [Display(Name = "National Insurance number (optional)")]
        [MaxLength(13, ErrorMessage = _ninErrorMessage)]
        [RegularExpression(
            Constants.UkNinRegex,
            ErrorMessage = _ninErrorMessage,
            MatchTimeoutInMilliseconds = 3000)]
        public override string NINO { get; set; }

        public override Guid? SelectedDebtTypeId { get; set; }
        public override string SelectedDebtTypeName { get; set; }

        public override Guid CreditorId { get; set; }
        public override string CreditorName { get; set; }

        public bool IsSingleDebtType => !(DebtTypes is null) && DebtTypes.Count < 2;
        public IList<DebtType> DebtTypes { get; set; }

        public bool IsGovernmentCreditor { get; set; }

        public override Guid MoratoriumId { get; set; }

        public void SetSelectedDebtTypeName() => SelectedDebtTypeName = DebtTypes.SingleOrDefault(x => x.Id == SelectedDebtTypeId)?.Name;
    }
}
