using System;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.MoneyAdviserService.Responses
{
    public class DebtEligibilityReviewResponse
    {
        public string CreditorNotes { get; set; }
        public string MoneyAdviserNotes { get; set; }
        public DebtEligibilityReviewReasons Reason { get; set; }
        public DebtEligibilityReviewStatus Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
    }
}
