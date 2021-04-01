using System;
using Insolvency.Common;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class DebtorEligibilityReview
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedOn { get; set; }
        public string CreditorNotes { get; set; }
        public string MoneyAdviserNotes { get; set; }
        public string CourtDecisionNotes { get; set; }
        public string Reason { get; set; }
        public Guid ReasonId { get; set; }
        public string Subreason { get; set; }
        public Guid SubreasonId { get; set; }
        public string Status { get; set; }
        public Guid StatusId { get; set; }
        public string Creditor { get; set; }
        public Guid CreditorId { get; set; }

        public DebtorEligibilityReviewResponse ToDebtorEligibilityReview(DynamicsGatewayOptions options)
        {
            return new DebtorEligibilityReviewResponse
            {
                CreditorId = CreditorId,
                CreditorNotes = CreditorNotes,
                MoneyAdviserNotes = MoneyAdviserNotes,
                CreditorName = Creditor,
                Status = options.DebtorEligibilityReviewStatus[StatusId.ToString()],
                CreatedOn = CreatedOn.ToDateTimeOffset(),
                EndReason = options.GetEligibilityReviewParentReasonById(ReasonId),
                ModifiedOn = ModifiedOn.ToDateTimeOffset(),
                NoLongerEligibleReason = SubreasonId != Guid.Empty
                    ? options.GetEligibilityReviewChildReasonById(SubreasonId)
                    : null
            };
        }
    }
}