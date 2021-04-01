using System;
using System.Linq;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class DebtEligibility
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedOn { get; set; }
        public string CreditorNotes { get; set; }
        public string Reason { get; set; }
        public Guid ReasonId { get; set; }
        public string Status { get; set; }
        public Guid StatusId { get; set; }
        public string MoneyAdviserNotes { get; set; }

        public DebtEligibilityReviewResponse ToDebtEligibilityReview(DynamicsGatewayOptions options)
        {
            return new DebtEligibilityReviewResponse
            {
                CreditorNotes = CreditorNotes,
                Reason = (DebtEligibilityReviewReasons)int.Parse(options.DebtEligibilityReviewReasons
                    .First(r => r.Value == ReasonId.ToString()).Key),
                Status = options.DebtEligibilityReviewStatus[StatusId.ToString()],
                MoneyAdviserNotes = MoneyAdviserNotes,
                CreatedOn = CreatedOn.ToDateTimeOffset(),
                ModifiedOn = ModifiedOn.ToDateTimeOffset(),
            };
        }
    }
}