using System;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorEligibilityReviewViewModel
    {
        public DebtorEligibilityReviewViewModel()
        {
        }

        public DebtorEligibilityReviewViewModel(DebtorEligibilityReviewResponse clientEligibilityReview)
        {
            CreatedOn = clientEligibilityReview.CreatedOn;
            CreditorId = clientEligibilityReview.CreditorId;
            CreditorNotes = clientEligibilityReview.CreditorNotes;
            CreditorOrganisation = clientEligibilityReview.CreditorName;
            DebtorEligibilityReviewStatus = clientEligibilityReview.Status;
            DebtorEligibilityReviewParentReason = clientEligibilityReview.EndReason;
            DebtorEligibilityReviewChildReason = clientEligibilityReview.NoLongerEligibleReason;
        }

        public Guid CreditorId { get; set; }

        public string CreditorNotes { get; set; }

        public string CreditorOrganisation { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public DebtorEligibilityReviewStatus? DebtorEligibilityReviewStatus { get; set; }

        public BreathingSpaceClientEndReasonType? DebtorEligibilityReviewParentReason { get; set; }

        public BreathingSpaceEndReasonNoLongerEligibleReasonType? DebtorEligibilityReviewChildReason { get; set; }

        public string Reason =>
             this switch
             {
                 {
                     DebtorEligibilityReviewParentReason: BreathingSpaceClientEndReasonType.NoLongerEligible,
                 } => DebtorEligibilityReviewChildReason.HasValue ?
                    $"{DebtorEligibilityReviewParentReason.GetDisplayName()}{DebtorEligibilityReviewChildReason.Value.GetDisplayName()}" :
                    null,
                 {
                     DebtorEligibilityReviewParentReason: BreathingSpaceClientEndReasonType.AbleToPayDebts,
                 } => "The client has sufficient funds to discharge their debt",
                 _ => null
             };

        protected string FormatDate(DateTimeOffset dateTimeOffset) =>
            $"{dateTimeOffset.ToString(Constants.PrettyDateFormat)} at {dateTimeOffset.ToString("HH.mm")}{dateTimeOffset.ToString("tt").ToLower()}";

        protected string RequestedOn => CreatedOn.HasValue ? FormatDate(CreatedOn.Value) : null;
    }
}
