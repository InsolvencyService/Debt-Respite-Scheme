using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorClientEligibilityReviewViewModel : IConditionalRequiredValidation
    {
        private const string ErrorMessage = "Please provide details to support the review";

        [Required(ErrorMessage = ErrorMessage)]
        public BreathingSpaceClientEndReasonType? EndResaon { get; set; }

        [ConditionalRequired(ErrorMessage = ErrorMessage)]
        public BreathingSpaceEndReasonNoLongerEligibleReasonType? NoLongerEligibleReason { get; set; }

        [Required(ErrorMessage = ErrorMessage)]
        public string CreditorNotes { get; set; }

        public bool ConditionalFlag => EndResaon == BreathingSpaceClientEndReasonType.NoLongerEligible;

        public Guid CreditorId { get; set; }
    }
}
