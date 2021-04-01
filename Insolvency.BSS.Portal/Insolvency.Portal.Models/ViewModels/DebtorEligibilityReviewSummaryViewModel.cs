using System;
using Insolvency.Portal.Models.ViewModels.Creditor;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorEligibilityReviewSummaryViewModel : CreditorClientEligibilityReviewConfirmViewModel
    {
        public DebtDetailViewModel DebtDetailViewModel { get; set; }
        public Guid MoratoriumId { get; set; }
    }
}
