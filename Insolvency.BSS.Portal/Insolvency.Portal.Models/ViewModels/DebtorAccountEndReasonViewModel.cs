using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAccountEndReasonViewModel : IMultiConditionalRequiredValidation
    {
        private BreathingSpaceEndReasonNoLongerEligibleReasonType? _noLongerEligibleReason;

        public bool IsInMentalHealthMoratorium { get; set; }

        [Required(ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        public BreathingSpaceEndReasonType? SubmitOption { get; set; }

        [MultiConditionalRequired(nameof(IsPartOfThirtyDayReviewStoppedTreament), ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        [Display(Name = "Has this decision been made as part of a 30 day review?")]
        public bool? IsPartOfThirtyDayReviewStoppedTreament { get; set; }

        [MultiConditionalRequired(nameof(IsPartOfThirtyDayReviewUnableToReachPointOfContact), ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        [Display(Name = "Has this decision been made as part of a 30 day review?")]
        public bool? IsPartOfThirtyDayReviewUnableToReachPointOfContact { get; set; }

        [MultiConditionalRequired(nameof(IsPartOfThirtyDayReviewNotCompliedWithObligation), ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        [Display(Name = "Has this decision been made as part of a 30 day review?")]
        public bool? IsPartOfThirtyDayReviewNotCompliedWithObligation { get; set; }

        [MultiConditionalRequired(nameof(IsPartOfThirtyDayReviewUnableToContactClient), ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        [Display(Name = "Has this decision been made as part of a 30 day review?")]
        public bool? IsPartOfThirtyDayReviewUnableToContactClient { get; set; }

        [MultiConditionalRequired(nameof(IsPartOfThirtyDayReviewUsingDebtManagement), ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        [Display(Name = "Has this decision been made as part of a 30 day review?")]
        public bool? IsPartOfThirtyDayReviewUsingDebtManagement { get; set; }

        [MultiConditionalRequired(nameof(NoLongerEligibleReason), ErrorMessage = "Please select a reason for ending your client's Breathing Space")]
        [Display(Name = "Why is the client not eligible?")]
        public BreathingSpaceEndReasonNoLongerEligibleReasonType? NoLongerEligibleReason
        {
            get => ConditionalFlag ? _noLongerEligibleReason : null;
            set => _noLongerEligibleReason = value;
        }

        public bool ConditionalFlag => SubmitOption.HasValue && SubmitOption.Value == BreathingSpaceEndReasonType.NoLongerEligible;

        [MultiConditionalRequired(nameof(DeathDay), ErrorMessage= "Client's date of death must include a day")]
        [Range(1, 31, ErrorMessage= "Day must be between 1 and 31")]
        [Display(Name = "Day")]
        public int? DeathDay { get; set; }

        [MultiConditionalRequired(nameof(DeathMonth), ErrorMessage= "Client's date of death must include a month")]
        [Range(1, 12, ErrorMessage= "Month must be between 1 and 12")]
        [Display(Name = "Month")]
        public int? DeathMonth { get; set; }

        [MultiConditionalRequired(nameof(DeathYear), ErrorMessage = "Client's date of death must include a year")]
        [Range(2020, 3000, ErrorMessage= "Year must be after 2020")]
        [Display(Name = "Year")]
        public int? DeathYear { get; set; }

        [Display(Name = "What was the date of death?")]
        public bool IsValidDateOfDeath => ValidateDeathDate();

        private bool ValidateDeathDate()
        {
            try
            {
                return DateOfDeath != null;
            }
            catch
            {
                return false;
            }
        }
        
        [MultiConditionalRequired(nameof(DateOfDeath), ErrorMessage = "Invalid date")]
        public DateTime? DateOfDeath { get => GetDate(DeathDay, DeathMonth, DeathYear); }

        public DateTime? GetDate(int? day, int? month, int? year)
        {
            try
            {
                if (!day.HasValue || !month.HasValue || !year.HasValue)
                {
                    return DateTime.Now;
                }
                return new DateTime(year.Value, month.Value, day.Value);
            }
            catch
            {
                return null;
            }
        }

        [MultiConditionalRequired(nameof(TreatmentEndDay), ErrorMessage = "Mental health treatment end date must include a day")]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
        [Display(Name = "Day")]
        public int? TreatmentEndDay { get; set; }

        [MultiConditionalRequired(nameof(TreatmentEndMonth), ErrorMessage = "Mental health treatment end date must include a month")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        [Display(Name = "Month")]
        public int? TreatmentEndMonth { get; set; }

        [MultiConditionalRequired(nameof(TreatmentEndYear), ErrorMessage = "Mental health treatment end date must include a year")]
        [Range(2020, 3000, ErrorMessage = "Year must be after 2020")]
        [Display(Name = "Year")]
        public int? TreatmentEndYear { get; set; }

        [Display(Name = "What was the date of death?")]
        public bool IsValidTreatmentEndDate => ValidateEndTreatmentDate();

        private bool ValidateEndTreatmentDate()
        {
            try
            {
                return TreatmentEndDate != null;
            }
            catch
            {
                return false;
            }
        }

        [MultiConditionalRequired(nameof(TreatmentEndDate), ErrorMessage = "Invalid date")]
        public DateTime? TreatmentEndDate { get => GetDate(TreatmentEndDay, TreatmentEndMonth, TreatmentEndYear); }

        [JsonIgnore]
        public Dictionary<string, Func<bool>> Actions => new Dictionary<string, Func<bool>> 
        {
            { nameof(NoLongerEligibleReason), () => ConditionalFlag  },
            { nameof(IsPartOfThirtyDayReviewStoppedTreament), () => SubmitOption != null && 
                                                                   SubmitOption.Value == BreathingSpaceEndReasonType.StoppedTreatment && 
                                                                   !IsPartOfThirtyDayReviewStoppedTreament.HasValue },
            { nameof(IsPartOfThirtyDayReviewUnableToReachPointOfContact), () => SubmitOption != null &&
                                                                               SubmitOption.Value == BreathingSpaceEndReasonType.UnableToReachPointOfContact &&
                                                                               !IsPartOfThirtyDayReviewUnableToReachPointOfContact.HasValue },
            { nameof(IsPartOfThirtyDayReviewNotCompliedWithObligation), () => SubmitOption != null &&
                                                                               SubmitOption.Value == BreathingSpaceEndReasonType.NotCompliedWithObligations &&
                                                                               !IsPartOfThirtyDayReviewNotCompliedWithObligation.HasValue },
            { nameof(IsPartOfThirtyDayReviewUnableToContactClient), () => SubmitOption != null &&
                                                                               SubmitOption.Value == BreathingSpaceEndReasonType.UnableToContactClient &&
                                                                               !IsPartOfThirtyDayReviewUnableToContactClient.HasValue },
            { nameof(IsPartOfThirtyDayReviewUsingDebtManagement), () => SubmitOption != null &&
                                                                               SubmitOption.Value == BreathingSpaceEndReasonType.DebtManagementSolution &&
                                                                               !IsInMentalHealthMoratorium &&
                                                                               !IsPartOfThirtyDayReviewUsingDebtManagement.HasValue },
            {   nameof(DateOfDeath), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.Deceased },
            {   nameof(DeathDay), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.Deceased },
            {   nameof(DeathMonth), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.Deceased },
            {   nameof(DeathYear), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.Deceased },
            {   nameof(TreatmentEndDate), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.StoppedTreatment },
            {   nameof(TreatmentEndDay), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.StoppedTreatment },
            {   nameof(TreatmentEndMonth), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.StoppedTreatment },
            {   nameof(TreatmentEndYear), () => SubmitOption != null && SubmitOption.Value == BreathingSpaceEndReasonType.StoppedTreatment }
        };

        public string GetBreathingSpaceEndReasonConfirmationMessage()
        {
            var reason = SubmitOption.Value.GetDisplayName();

            if ((IsPartOfThirtyDayReviewUsingDebtManagement.IsTrue() && !IsInMentalHealthMoratorium) ||
                IsPartOfThirtyDayReviewStoppedTreament.IsTrue() ||
                IsPartOfThirtyDayReviewUnableToReachPointOfContact.IsTrue() ||
                IsPartOfThirtyDayReviewNotCompliedWithObligation.IsTrue() ||
                IsPartOfThirtyDayReviewUnableToContactClient.IsTrue())
            {
                if (IsInMentalHealthMoratorium)
                {
                    reason += ". This decision was made as part of a 30 day review";
                }
                else
                {
                    reason += ". This decision was made as part of a midway review";
                }
            }
            else if (SubmitOption.Value == BreathingSpaceEndReasonType.NoLongerEligible)
            {
                reason += NoLongerEligibleReason.Value.GetDisplayName();
            }

            return reason;
        }

        public bool? IsPartOfThirtyDayReviewRequiredAndAnswered()
        {
            if (SubmitOption == null)
            {
                return null;
            }

            if (!IsInMentalHealthMoratorium &&
                SubmitOption.Value == BreathingSpaceEndReasonType.DebtManagementSolution)
            {
                return IsPartOfThirtyDayReviewUsingDebtManagement;
            }
            if (SubmitOption.Value == BreathingSpaceEndReasonType.StoppedTreatment)
            {
                return IsPartOfThirtyDayReviewStoppedTreament;
            }
            if (SubmitOption.Value == BreathingSpaceEndReasonType.UnableToReachPointOfContact)
            {
                return IsPartOfThirtyDayReviewUnableToReachPointOfContact;
            }
            if (SubmitOption.Value == BreathingSpaceEndReasonType.NotCompliedWithObligations)
            {
                return IsPartOfThirtyDayReviewNotCompliedWithObligation;
            }
            if (SubmitOption.Value == BreathingSpaceEndReasonType.UnableToContactClient)
            {
                return IsPartOfThirtyDayReviewUnableToContactClient;
            }

            return false;
        }
    }
}
