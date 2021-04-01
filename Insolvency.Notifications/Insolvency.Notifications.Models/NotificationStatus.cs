namespace Insolvency.Notifications.Models
{
    public enum NotificationStatus
    {
        Transit,
        Sent,
        Completed,
        Failed,
        LetterFailed,
        PermanentFailure,
        TemporaryFailure,
        TechnicalFailureEmail,
        TechnicalFailureLetter
    }
}
