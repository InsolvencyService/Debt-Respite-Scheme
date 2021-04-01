using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.Shared.Requests
{
    public class DebtEligibilityReviewRequest
    {
        [JsonIgnore]
        public Guid DebtId { get; set; }
        [JsonIgnore]
        public virtual Guid? CreditorId { get; set; }
        [JsonIgnore]
        public virtual Guid? MoneyAdviserId { get; set; }

        [Required(ErrorMessage = "breathingSpaceEligibilityReviewType is required")]
        [EnumDataType(typeof(DebtEligibilityReviewReasons))]
        public DebtEligibilityReviewReasons? ReviewType { get; set; }

        [Required(ErrorMessage = "creditorNotes is required")]
        public string CreditorNotes { get; set; }

        public Dictionary<string, object> ToDictionary(DynamicsGatewayOptions options) =>
            new Dictionary<string, object>
            {
                { "EligibilityReviewReasonId",
                   options
                        .GetDebtEligibilityReviewReasonId(ReviewType.Value)
                        .ToString() },
                { nameof(CreditorNotes),CreditorNotes },
                { nameof(CreditorId), CreditorId?.ToString()},
                { nameof(MoneyAdviserId), MoneyAdviserId?.ToString()}
            };
    }
}
