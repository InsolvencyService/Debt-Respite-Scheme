using System;
using System.Runtime.InteropServices;

namespace Insolvency.Common
{
    public static class DateTimeExtension
    {

        public static DateTime FromUtcToSpecifiedTimeZone(
            this DateTime value, 
            string windowsTimeZoneId, 
            string linuxTimeZoneId)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(value, windowsTimeZone);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var linxTimeZone = TimeZoneInfo.FindSystemTimeZoneById(linuxTimeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(value, linxTimeZone);
            }

            return value;
        }
    }
}
