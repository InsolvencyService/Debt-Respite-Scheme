using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum DebtorEligibilityReviewStatus
    {
        [Display(Name = "Review Requested")]
        ReviewRequested = 0,

        [Display(Name = "Eligible After Adviser Review")]
        EligibleAfterAdviserReview = 1,

        [Display(Name = "Not Eligible After Adviser Review")]
        NotEligibleAfterAdviserReview = 2,

        [Display(Name = "Not Eligible After Court Ruling")]
        NotEligibleAfterCourtRuling = 3,

        [Display(Name = "Cancelled")]
        Cancelled = 4
    }
}
