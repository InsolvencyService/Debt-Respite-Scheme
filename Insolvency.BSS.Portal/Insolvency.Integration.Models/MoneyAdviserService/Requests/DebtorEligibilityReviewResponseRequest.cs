using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class DebtorEligibilityReviewResponseRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }

        [Required]
        public Guid CreditorId { get; set; }

        [Required]
        public bool? IsNotEligibleAfterReview { get; set; }

        [Required]
        public string MoneyAdviserNotes { get; set; }

        public Dictionary<string, object> ToDictionary(DynamicsGatewayOptions options)
        {
            var status = IsNotEligibleAfterReview.Value ? DebtorEligibilityReviewStatus.NotEligibleAfterAdviserReview : DebtorEligibilityReviewStatus.EligibleAfterAdviserReview;
            return new Dictionary<string, object>()
            {
                    { "MoneyAdviserNotes", MoneyAdviserNotes },
                    { "CreditorId", CreditorId },
                    { "DebtorEligibilityStatusIdentifier", options.DebtorEligibilityReviewStatusCode[status]}
            };
        }
    }
}
