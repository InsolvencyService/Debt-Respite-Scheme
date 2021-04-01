namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class LandingPageStats
    {
        public int ActiveMoratoriums { get; set; }

        public int DebtEligibilityReviews { get; set; }
        public int DebtorEligibilityReviews { get; set; }
        public int DebtProposedDebts { get; set; }
        public int SoldOnDebts { get; set; }
        public int ClientTransfers { get; set; }
        public int ClientTransferRequests { get; set; }
        public int SentToMoneyAdviser { get; set; }
    }
}
