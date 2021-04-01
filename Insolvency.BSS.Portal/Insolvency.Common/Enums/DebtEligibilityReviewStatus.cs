namespace Insolvency.Common.Enums
{
    public enum DebtEligibilityReviewStatus
    {
        ReviewRequested = 0,
        EligibleAfterAdviserReview = 1,
        NotEligibleAfterAdviserReview = 2,
        NotEligibleAfterCourtRuling = 3,
        Cancelled = 4
    }
}
