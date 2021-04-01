using System;

namespace Insolvency.Common
{
    public static class DateTimeOffsetExtension
    {
        public static string ToOrdinalDateTimeFormat(this DateTimeOffset value, string monthFormat = "MMMM")
        {
            return $"{value.Day}{value.Day.ToOrdinal()} " +
                   $"{value.ToString(monthFormat)} " +
                   $"{value.Year}";
        }
    }
}
