using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.Shared.Requests
{
    public abstract class DebtorEligibilityReviewRequest : IConditionalRequiredValidation
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }
        public virtual Guid? CreditorId { get; set; }

        [Required(ErrorMessage = "endReason is required")]
        [EnumDataType(typeof(BreathingSpaceClientEndReasonType))]
        public BreathingSpaceClientEndReasonType? EndReason { get; set; }

        [ConditionalRequired(ErrorMessage = "noLongerEligibleReason is required if endReason is 0(NoLongerEligible)")]
        [EnumDataType(typeof(BreathingSpaceEndReasonNoLongerEligibleReasonType))]
        public BreathingSpaceEndReasonNoLongerEligibleReasonType? NoLongerEligibleReason { get; set; }

        [Required(ErrorMessage = "creditorNotes is required")]
        public string CreditorNotes { get; set; }

        [JsonIgnore]
        public bool ConditionalFlag => EndReason == BreathingSpaceClientEndReasonType.NoLongerEligible;

        public Dictionary<string, object> ToDictionary() =>
            new Dictionary<string, object>
            {
                { nameof(CreditorId),CreditorId.Value.ToString() },
                { nameof(EndReason),((int)EndReason.Value).ToString() },
                { nameof(NoLongerEligibleReason),
                    NoLongerEligibleReason.HasValue ? ((int)NoLongerEligibleReason.Value).ToString(): null },
                { nameof(CreditorNotes),CreditorNotes }
            };
    }
}
