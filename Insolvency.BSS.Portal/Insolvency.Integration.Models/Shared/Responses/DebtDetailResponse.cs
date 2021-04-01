using System;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class DebtDetailResponse
    {
        public Guid Id { get; set; }
        public DebtStatus Status { get; set; }
        public Guid? CreditorId { get; set; }
        public string CreditorName { get; set; }
        public Guid? SoldToCreditorId { get; set; }
        public string SoldToCreditorName { get; set; }
        public bool PreviouslySold { get; set; }
        public Guid? DebtTypeId { get; set; }
        public string Reference { get; set; }
        public string NINO { get; set; }
        public decimal? Amount { get; set; }
        public string DebtTypeName { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
        public DateTimeOffset? StartsOn { get; set; }
        public DateTimeOffset? EndsOn { get; set; }
        public DateTimeOffset? RemovedOn { get; set; }
        public DebtEligibilityReviewResponse DebtEligibilityReview { get; set; }

        public DebtRemovalReason? DebtRemovalReason { get; set; }
    }
}
