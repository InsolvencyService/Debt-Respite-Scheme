using System;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtEligibilityReviewViewModel
    {
        public DebtEligibilityReviewViewModel()
        {
        }

        public DebtEligibilityReviewViewModel(DebtEligibilityReviewResponse debtEligibilityReview)
        {
            CreditorNote = debtEligibilityReview.CreditorNotes;
            ReviewReason = debtEligibilityReview.Reason.GetDisplayName();
            ReviewStatus = debtEligibilityReview.Status;
            CreatedOn = debtEligibilityReview.CreatedOn;
        }

        public string CreditorNote { get; set; }
        public string ReviewReason { get; set; }
        public DebtEligibilityReviewStatus ReviewStatus { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string FormattedCreatedOn => CreatedOn.ToString(Constants.PrettyDateFormat);
        public string FormattedCreatedOnTime => CreatedOn.ToString("t").ToLower();
        public string FormattedCreatedOnOrdinal => CreatedOn.ToOrdinalDateTimeFormat();
        public bool RequiredDebtReview => ReviewStatus == DebtEligibilityReviewStatus.ReviewRequested;
        public bool DebtReviewCompleted => !RequiredDebtReview;
        public bool HasReviewEligibleDebt => RequiredDebtReview || ReviewStatus == DebtEligibilityReviewStatus.EligibleAfterAdviserReview;
        public bool DebtRemovedAfterReview => ReviewStatus == DebtEligibilityReviewStatus.NotEligibleAfterAdviserReview;
    }
}
