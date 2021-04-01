using System.Collections.Generic;

using Insolvency.Notifications.Models;

namespace Insolvency.Notifications.BSS.Models
{
    public static class NotificationStatusExtensions
    {
        public static readonly IReadOnlyDictionary<NotificationStatus, int> DynamicsStatusMapping = new Dictionary<NotificationStatus, int>
        {
            { NotificationStatus.Completed, 961080002 },
            { NotificationStatus.Failed, 961080003 },
            { NotificationStatus.LetterFailed, 961080000 },
            { NotificationStatus.PermanentFailure, 961080005 },
            { NotificationStatus.TemporaryFailure, 961080006 },
            { NotificationStatus.TechnicalFailureEmail, 961080007 },
            { NotificationStatus.TechnicalFailureLetter, 961080004 },
        };

        private static int EmailMappingStatusToDynamicsId(this NotificationStatus status) => GetDynamicsMapIdByStatus(status);

        private static int LetterMappingStatusToDynamicsId(this NotificationStatus status) => GetDynamicsMapIdByStatus(status);

        private static int APIMappingStatusToDynamicsId(this NotificationStatus status) => GetDynamicsMapIdByStatus(status);

        private static int GetDynamicsMapIdByStatus(NotificationStatus status)
        {
            DynamicsStatusMapping.TryGetValue(status, out var mapId);
            return mapId;
        }

        public static int MapToDynamicsStatus(this StatusUpdateNotification update)
        {
            if (update.Type == NotificationType.Email)
            {
                return update.Status.EmailMappingStatusToDynamicsId();
            }
            else if (update.Type == NotificationType.Letter)
            {
                return update.Status.LetterMappingStatusToDynamicsId();
            }
            else if (update.Type == NotificationType.API)
            {
                return update.Status.APIMappingStatusToDynamicsId();
            }
            return 0;
        }
    }
}
