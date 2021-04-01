using System;
using System.Text.Json.Serialization;
using Insolvency.Integration.Models.Shared.Requests;

namespace Insolvency.Integration.Models.CreditorService.Requests
{
    public class CrDebtorEligibilityReviewRequest : DebtorEligibilityReviewRequest
    {
        [JsonIgnore]
        public override Guid? CreditorId { get; set; }
    }
}
