using System;
using System.Globalization;

namespace Insolvency.Common
{
    public static class StringExtension
    {
        public static DateTimeOffset ToDateTimeOffset(this string value, string format = Constants.UkDateFormat)
        {
            _ = DateTimeOffset.TryParseExact(
                    value, 
                    format, 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out var result
                );
            return result;
        }
    }
}
