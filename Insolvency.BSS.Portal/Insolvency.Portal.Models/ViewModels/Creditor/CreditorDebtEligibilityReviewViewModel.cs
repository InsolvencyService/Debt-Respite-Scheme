using System.ComponentModel.DataAnnotations;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorDebtEligibilityReviewViewModel
    {
        public CreditorDebtPartialViewModel Debt { get; set; }

        [Required(ErrorMessage = "Please select a reason for the debt review")]
        public DebtEligibilityReviewReasons? Reason { get; set; }

        [Required(ErrorMessage = "Please provide details to support the review")]
        public string CreditorNotes { get; set; }

    }
}
