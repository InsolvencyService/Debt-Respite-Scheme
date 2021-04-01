using Newtonsoft.Json.Converters;

namespace Insolvency.Notifications.Models.Json
{
    public class DateTimeJsonConverter : IsoDateTimeConverter
    {
        public DateTimeJsonConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        }
    }
}
