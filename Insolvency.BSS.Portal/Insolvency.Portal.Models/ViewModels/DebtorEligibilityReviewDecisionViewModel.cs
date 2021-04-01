using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Common;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorEligibilityReviewDecisionViewModel : DebtorEligibilityReviewViewModel
    {
        public DebtorEligibilityReviewDecisionViewModel() : base() { }
        public DebtorEligibilityReviewDecisionViewModel(DebtorEligibilityReviewResponse clientEligibilityReview) : base(clientEligibilityReview) { }
        public DebtorEligibilityReviewDecisionViewModel(DebtorEligibilityReviewViewModel clientEligibilityReview)
        {
            CreatedOn = clientEligibilityReview.CreatedOn;
            CreditorId = clientEligibilityReview.CreditorId;
            CreditorNotes = clientEligibilityReview.CreditorNotes;
            CreditorOrganisation = clientEligibilityReview.CreditorOrganisation;
            DebtorEligibilityReviewStatus = clientEligibilityReview.DebtorEligibilityReviewStatus;
            DebtorEligibilityReviewParentReason = clientEligibilityReview.DebtorEligibilityReviewParentReason;
            DebtorEligibilityReviewChildReason = clientEligibilityReview.DebtorEligibilityReviewChildReason;
        }

        [Required(ErrorMessage = "Please select Yes or No to end your client’s Breathing Space")]
        public bool? EndBreathingSpace { get; set; }

        [Required(ErrorMessage = "Please provide details to support the decision")]
        public string MoneyAdviserNotes { get; set; }

        public string CreditorName { get; set; }

        public string MoneyAdviserName { get; set; }

        public string MoneyAdviserOrganisation { get; set; }

        public string ClientEligibilityReviewSubtitle => $"{CreditorOrganisation} asked for a review";

        public string RequestedBy => $"{CreditorName}, {CreditorOrganisation}, {RequestedOn}";

        private string ReviewedOn => FormatDate(DateTimeOffset.UtcNow);

        public string ReviewedBy => $"{MoneyAdviserName}, {MoneyAdviserOrganisation}, {ReviewedOn}";

        public string ConfirmationTitle => EndBreathingSpace.IsTrue() ?
            "Confirm you are ending your client’s Breathing Space" :
            "Confirm you are not ending your client’s Breathing Space";

        public string ConfirmationButtonText => EndBreathingSpace.IsTrue() ?
            "Confirm and end Breathing Space" :
            "Confirm and continue";
    }
}
