using System.Collections.Generic;

namespace Insolvency.Notifications.Models
{
    public static class NotificationTypeExtensions
    {
        public static KeyValuePair<string, object> ToMessageType(this NotificationType notificationType)
        {
            var msgType = new KeyValuePair<string, object>();

            switch (notificationType)
            {
                case NotificationType.Email:
                    msgType = new KeyValuePair<string, object>("MessageType", "Email");
                    break;
                case NotificationType.Letter:
                    msgType = new KeyValuePair<string, object>("MessageType", "Letter");
                    break;
                case NotificationType.API:
                    msgType = new KeyValuePair<string, object>("MessageType", "Api");
                    break;
                default:
                    break;
            }

            return msgType;
        }
    }
}
