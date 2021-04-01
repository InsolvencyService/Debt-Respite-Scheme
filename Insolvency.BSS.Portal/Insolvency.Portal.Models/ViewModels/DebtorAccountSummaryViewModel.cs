using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAccountSummaryViewModel
    {
        public DebtorAccountSummaryViewModel()
        {
        }

        public DebtorAccountSummaryViewModel(BreathingSpaceResponse accountSummary)
        {
            if (accountSummary is null)
                throw new ArgumentNullException(nameof(accountSummary));

            DebtorDetail = new DebtorDetailViewModel(accountSummary);
            DebtDetails = accountSummary.DebtDetails?
                .Select(d => new DebtDetailViewModel(
                    d, accountSummary.DebtorEligibilityReviews
                        .FirstOrDefault(r => r.CreditorId == d.CreditorId)))
                .ToList();
            MoneyAdviceOrganisation = new MoneyAdviceOrganisationViewModel(accountSummary.MoneyAdviserOrganization);
            DebtorTransfer = accountSummary.DebtorTransfer is null ? null:
                new DebtorTransferViewModel(DebtorDetail.PersonalDetail, MoneyAdviceOrganisation, accountSummary.DebtorTransfer);
        }

        public DebtorDetailViewModel DebtorDetail { get; set; }
        public List<DebtDetailViewModel> DebtDetails { get; set; }
        public MoneyAdviceOrganisationViewModel MoneyAdviceOrganisation { get; set; }
        public DebtorTransferViewModel DebtorTransfer { get; set; }
        public Guid? AcceptedProposedDebtId { get; set; }
        public bool HasAcceptedProposedDebt => AcceptedProposedDebtId.HasValue;
        public bool IsOwningOrganization { get; set; }
        public bool HasDebtsDirectlyRemoved => DebtDetails?.Any(debt => debt.HasDebtBeenRemovedDirectly) ?? false;
        public bool HasDebtsRemovedAfterReview => DebtDetails?.Any(debt => debt.HasDebtBeenRemovedAfterReview) ?? false;
        public bool HasRemovedDebts => HasDebtsDirectlyRemoved || HasDebtsRemovedAfterReview;
        public bool HasTransferRequested => IsCurrentlyTransferring && DebtorTransfer.Status == TransferDebtorRequestStatusCodes.Requested;
        public bool IsCurrentlyTransferring => DebtorTransfer != null;
        public bool HasTransferCompleted => IsCurrentlyTransferring && DebtorTransfer.Status == TransferDebtorRequestStatusCodes.Completed;
        public bool HasTransferred => IsCurrentlyTransferring && DebtorTransfer.Status == TransferDebtorRequestStatusCodes.Transferred;
    }
}
