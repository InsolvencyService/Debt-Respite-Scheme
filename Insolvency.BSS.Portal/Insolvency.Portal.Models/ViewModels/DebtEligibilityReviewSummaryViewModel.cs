using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtEligibilityReviewSummaryViewModel : DebtorRadioYesNoViewModel
    {
        public DebtDetailViewModel DebtDetailViewModel { get; set; }

        [Required(ErrorMessage = "Please select Yes or No to remove this debt")]
        public override string SubmitOption { get; set; }

        [Required(ErrorMessage = "Please provide details to support your decision")]
        public string ReviewSupportingDetail { get; set; }

        public bool HasReviewSubmitted { get; set; }
        public bool HasDebtRemoved => SubmitNow;
    }
}
