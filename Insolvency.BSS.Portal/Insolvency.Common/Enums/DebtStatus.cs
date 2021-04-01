namespace Insolvency.Common.Enums
{
    public enum DebtStatus
    {
        Active_RecoveryActionStopped = 0,
        Active_SoldOnDebt = 1,
        Active_CreditorNotificationPending = 2,
        Active_CreditorNotificationSubmitted = 3,
        Active_CreditorNotificationFailed = 4,
        Active_NewDebt = 5,
        Active_ReviewRequested = 6,
        Active_AfterReview = 7,
        Draft_PendingSubmission = 8,
        Draft_Submitted = 9,
        Draft_CreditorProposedNewDebt = 10,
        RemovedAfterCourtRuling = 11,
        RemovedAndAcknowledged = 12,
        RemovedAfterAdviserReview = 13,
        Cancelled = 14,
        Expired = 15,
        RemovedNeverActive = 16
    }
}
