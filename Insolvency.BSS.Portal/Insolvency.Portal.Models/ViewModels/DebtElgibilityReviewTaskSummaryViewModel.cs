using Insolvency.Portal.Models.ViewModels.Creditor;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtElgibilityReviewTaskSummaryViewModel : CreditorDebtEligibilityReviewConfirmViewModel
    {
        public DebtDetailViewModel DebtDetailViewModel { get; set; }
    }
}
