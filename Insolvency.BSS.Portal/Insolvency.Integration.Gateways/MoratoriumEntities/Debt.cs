using System;
using System.Linq;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class Debt
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedOn { get; set; }
        public string RemovalDate { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
        public Guid StatusId { get; set; }
        public string Type { get; set; }
        public Guid TypeId { get; set; }
        public string CommencementDate { get; set; }
        public string ExpiryDate { get; set; }
        public bool PreviouslySold { get; set; }
        public Creditor Creditor { get; set; }
        public string CreditorExternalReference { get; set; }
        public string NationalInsuranceNumber { get; set; }
        public ProposedCreditor ProposedCreditor { get; set; }
        public DebtEligibility DebtEligibilityReview { get; set; }

        public Guid? DebtRemovalReasonId { get; set; }
        public string DebtRemovalReason { get; set; }
        public string DebtRemovalAdditionalInformation { get; set; }


        public DebtDetailResponse ToDebtDetail(DynamicsGatewayOptions options)
        {
            return new DebtDetailResponse
            {
                Id = Id,
                Amount = decimal.Parse(Amount),
                Reference = CreditorExternalReference,
                DebtTypeName = Type,
                NINO = NationalInsuranceNumber,
                CreatedOn = CreatedOn.ToDateTimeOffset(),
                ModifiedOn = ModifiedOn.ToDateTimeOffset(),
                Status = options.DebtStatus[StatusId.ToString()],
                SoldToCreditorName = ProposedCreditor?.Name,
                SoldToCreditorId = ProposedCreditor?.Id,
                PreviouslySold = PreviouslySold,
                CreditorName = Creditor.Name,
                CreditorId = Creditor.Id,
                StartsOn = CommencementDate?.ToDateTimeOffset(),
                EndsOn = ExpiryDate?.ToDateTimeOffset(),
                RemovedOn = RemovalDate?.ToDateTimeOffset(),
                DebtTypeId = TypeId,
                DebtEligibilityReview = DebtEligibilityReview != null
                    ? DebtEligibilityReview.ToDebtEligibilityReview(options)
                    : null,
                DebtRemovalReason = DebtRemovalReasonId.HasValue ?
                    options.DebtRemovalReason.First(r => r.Value == DebtRemovalReasonId.ToString()).Key : 
                    (DebtRemovalReason?)null
            };
        }
    }
}