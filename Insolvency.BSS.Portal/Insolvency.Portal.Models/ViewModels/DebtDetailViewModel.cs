using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtDetailViewModel
    {
        public DebtDetailViewModel()
        {
        }

        public DebtDetailViewModel(DebtDetailResponse debtDetail, DebtorEligibilityReviewResponse debtorEligibilityView)
        {
            Debt = new DebtViewModel
            {
                Id = debtDetail.Id,
                DebtAmount = debtDetail.Amount?.ToString(Constants.TwoDecimalPointFormat),
                Reference = debtDetail.Reference,
                NINO = debtDetail.NINO,
                SelectedDebtTypeName = debtDetail.DebtTypeName,
                CreatedOn = debtDetail.CreatedOn.Value,
                CommencementDate = debtDetail.StartsOn,
                RemovedOn = debtDetail.RemovedOn,
                Status = debtDetail.Status,
                PreviouslySold = debtDetail.PreviouslySold,
                SoldToCreditorId = debtDetail.SoldToCreditorId,
                SoldToCreditorName = debtDetail.SoldToCreditorName,
                RemovalReason = debtDetail.DebtRemovalReason
            };
            Creditor = new CreditorResponse
            {
                Name = debtDetail.CreditorName,
                Id = debtDetail.CreditorId.Value
            };
            DebtEligibilityReview = debtDetail.DebtEligibilityReview != null
                ? new DebtEligibilityReviewViewModel(debtDetail.DebtEligibilityReview)
                : null;
            DebtorClientEligibilityReview = debtorEligibilityView != null ?
                new DebtorEligibilityReviewViewModel(debtorEligibilityView)
                : null;
        }

        public CreditorResponse Creditor { get; set; }
        public DebtViewModel Debt { get; set; }
        public bool FromCreateBreathingSpace { get; set; }
        public bool IsNonCmpDebt { get; set; }
        public DebtEligibilityReviewViewModel DebtEligibilityReview { get; set; }
        public bool HasDebtReview => DebtEligibilityReview != null;
        public bool ClientEligibilityReviewRequested => ClientHasEligibilityReview &&
            DebtorClientEligibilityReview.DebtorEligibilityReviewStatus == DebtorEligibilityReviewStatus.ReviewRequested;
        public bool ClientEligibleAfterAdviserReview => ClientHasEligibilityReview &&
            DebtorClientEligibilityReview.DebtorEligibilityReviewStatus == DebtorEligibilityReviewStatus.EligibleAfterAdviserReview;
        public bool ClientHasEligibilityReview => DebtorClientEligibilityReview != null;
        public DebtorEligibilityReviewViewModel DebtorClientEligibilityReview { get; set; }

        public bool DisplayDebt
        {
            get
            {
                if (Debt == null || Creditor == null)
                {
                    return false;
                }

                if (HasDebtReview && DebtEligibilityReview.DebtRemovedAfterReview)
                {
                    return false;
                }

                if (HasDebtBeenRemovedDirectly)
                {
                    return false;
                }

                if (IsRejectedProposedDebt)
                {
                    return false;
                }

                if (IsProposedDebt)
                {
                    return false;
                }

                return true;
            }
        }

        public bool DisplayCreateLinks => !HasDebtReview && !ClientHasEligibilityReview && !IsDebtSold;
        public bool IsDebtSold => Debt.Status == DebtStatus.Active_SoldOnDebt;
        public bool IsDebtTransferred => Debt.PreviouslySold;
        public bool HasDebtBeenRemovedDirectly => Debt.RemovalReason.HasValue;
        public bool HasDebtBeenRemovedAfterReview => DebtEligibilityReview?.DebtRemovedAfterReview ?? false;
        public bool HasDebtBeenRemoved => HasDebtBeenRemovedDirectly || HasDebtBeenRemovedAfterReview;
        public bool IsProposedDebt => Debt.Status == DebtStatus.Draft_CreditorProposedNewDebt;
        public bool IsRejectedProposedDebt => Debt.Status == DebtStatus.RemovedNeverActive;
    }
}
