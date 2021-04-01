using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class MoneyAdviserLandingPageViewModel
    {
        public MoneyAdviserLandingPageViewModel(MoneyAdviserLandingPageStatsResponse landingPageStats)
        {
            ActiveBreathingSpacesCount = landingPageStats.ActiveBreathingSpacesCount;
            
            DebtReviewsCount = landingPageStats.DebtEligibilityReviews;
            ClientReviewsCount = landingPageStats.DebtorEligibilityReviews;
            NewlyProposedDebtsCount = landingPageStats.DebtProposedDebts;
            SoldDebtsTransfersCount = landingPageStats.SoldOnDebts;
            ClientTransferRequestsCount = landingPageStats.ClientTransferRequests;
            ClientTransfersMoneyAdviserCount = landingPageStats.ClientTransfers;
            SentToMoneyAdviserCount = landingPageStats.SentToMoneyAdviser;
        }

        public int ActiveBreathingSpacesCount { get; set; }

        public int TasksToDoCount =>
            DebtReviewsCount +
            ClientReviewsCount +
            NewlyProposedDebtsCount +
            SoldDebtsTransfersCount +
            ClientTransferRequestsCount +
            ClientTransfersMoneyAdviserCount;

        public int DebtReviewsCount { get; set; }
        public int ClientReviewsCount { get; set; }
        public int NewlyProposedDebtsCount { get; set; }
        public int SoldDebtsTransfersCount { get; set; }
        public int ClientTransferRequestsCount { get; set; }
        public int ClientTransfersMoneyAdviserCount { get; set; }
        public int SentToMoneyAdviserCount { get; set; }
    }
}
