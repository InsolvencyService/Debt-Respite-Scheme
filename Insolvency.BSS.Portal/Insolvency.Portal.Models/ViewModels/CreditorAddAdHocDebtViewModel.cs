using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class CreditorAddAdHocDebtViewModel : DebtViewModel
    {
        [Display(Name = "Debt Amount (optional)")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Please enter a valid amount, up to 2 decimal places", MatchTimeoutInMilliseconds = 3000)]
        [MaxLength(100)]
        public override string DebtAmount { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reference (optional)")]
        public override string Reference { get; set; }

        [MaxLength(100)]
        [Display(Name = "Type of debt (optional)")]
        public override string SelectedDebtTypeName { get; set; }

        public override Guid MoratoriumId { get; set; }
    }
}
