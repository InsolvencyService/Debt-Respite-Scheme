using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class DebtorAccountEndRequest : IMultiConditionalRequiredValidation
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }

        [Required]
        [EnumDataType(typeof(BreathingSpaceEndReasonType))]
        public BreathingSpaceEndReasonType? Reason { get; set; }

        [Required]
        public bool? IsPartOfThirtyDayReview { get; set; }

        [MultiConditionalRequired(nameof(NoLongerEligibleReason), ErrorMessage = "noLongerEligibleReason is required if reason is 2(NoLongerEligible)")]
        [EnumDataType(typeof(BreathingSpaceEndReasonNoLongerEligibleReasonType))]
        public BreathingSpaceEndReasonNoLongerEligibleReasonType? NoLongerEligibleReason { get; set; }

        [MultiConditionalRequired(nameof(DateOfDeath), ErrorMessage = "dateOfDeath is required if reason is 6(Deceased)")]
        public DateTime? DateOfDeath { get; set; }

        [MultiConditionalRequired(nameof(EndTreatmentDate), ErrorMessage = "endTreatmentDate is required if reason is 0(StoppedTreatment)")]
        public DateTime? EndTreatmentDate { get; set; }

        [JsonIgnore]
        public bool ConditionalFlag => Reason == BreathingSpaceEndReasonType.NoLongerEligible;

        [JsonIgnore]
        public Dictionary<string, Func<bool>> Actions => new Dictionary<string, Func<bool>>
        {
            { nameof(NoLongerEligibleReason), () => ConditionalFlag  },
            { nameof(DateOfDeath), () => Reason != null && Reason.Value == BreathingSpaceEndReasonType.Deceased },
            { nameof(EndTreatmentDate), () => Reason != null && Reason.Value == BreathingSpaceEndReasonType.StoppedTreatment }
        };

        public Dictionary<string, object> ToDictionary(DynamicsGatewayOptions options, bool isMentalHealthMoratorium)
        {
            var isPartOfThirtyDayReviewAnswerRequired = Reason == BreathingSpaceEndReasonType.StoppedTreatment ||
                                                        Reason == BreathingSpaceEndReasonType.UnableToReachPointOfContact ||
                                                        (Reason == BreathingSpaceEndReasonType.DebtManagementSolution && !isMentalHealthMoratorium) ||
                                                        Reason == BreathingSpaceEndReasonType.NotCompliedWithObligations ||
                                                        Reason == BreathingSpaceEndReasonType.UnableToContactClient;

            var reasonId = options.BreathingSpaceEndReason[((int)Reason).ToString()];

            var result = new Dictionary<string, object>()
            {
                    { "CancellationReasonParent", reasonId },
                    { "MidwayReviewRequired", isPartOfThirtyDayReviewAnswerRequired },
                    { "MHReviewRequired", false}
            };

            if (Reason == BreathingSpaceEndReasonType.NoLongerEligible && NoLongerEligibleReason.HasValue)
            {
                var noLongerEligibleReasonId = options.BreathingSpaceEndReasonNoLongerEligibleReason[((int)NoLongerEligibleReason).ToString()];
                result.Add("CancellationReasonChild", noLongerEligibleReasonId);
            }

            if (isPartOfThirtyDayReviewAnswerRequired && IsPartOfThirtyDayReview.HasValue)
            {
                result.Add("MidwayReviewAnswer", IsPartOfThirtyDayReview.Value);
            }

            if (Reason == BreathingSpaceEndReasonType.Deceased)
            {
                result.Add("DebtorDeceasedDate", DateOfDeath.Value.ToString(Constants.UkDateFormat));
            }

            if (Reason == BreathingSpaceEndReasonType.StoppedTreatment)
            {
                result.Add("MHTreatmentEndDate", EndTreatmentDate.Value.ToString(Constants.UkDateFormat));
            }

            return result;
        }
    }
}
