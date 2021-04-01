using System;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class DebtorEligibilityReviewResponse
    {
        public Guid CreditorId { get; set; }

        public string CreditorNotes { get; set; }
        public string MoneyAdviserNotes { get; set; }

        public string CreditorName { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public DebtorEligibilityReviewStatus? Status { get; set; }

        public BreathingSpaceClientEndReasonType? EndReason { get; set; }

        public BreathingSpaceEndReasonNoLongerEligibleReasonType? NoLongerEligibleReason { get; set; }
    }
}
