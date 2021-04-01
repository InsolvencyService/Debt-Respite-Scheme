using System;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAccountEndReasonConfirmationViewModel
    {
        public bool IsInMentalHealthMoratorium { get; set; }
        public BreathingSpaceEndReasonType SubmitOption { get; set; }
        public bool? IsPartOfThirtyDayReview { get; set; }
        public BreathingSpaceEndReasonNoLongerEligibleReasonType? NoLongerEligibleReason { get; set; }
        public string ReasonMessage { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public DateTime? EndTreatmentDate { get; set; }
        public bool AlreadyConfirmed { get; set; }
    }
}
