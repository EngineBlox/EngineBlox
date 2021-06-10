using System;

namespace EngineBlox.Timezones
{
    public static class UkDateTime
    {
        public static DateTime Now => ConvertToUkTime(DateTime.UtcNow);
        public static DateTime FromUtc(string utcDateTime) => ConvertToUkTime(DateTime.Parse(utcDateTime));
        public static DateTime ToUkTime(this DateTime toConvert) => ConvertToUkTime(toConvert);

        private static DateTime ConvertToUkTime(DateTime dateTimeUtc)
        {
            try
            {
                return TimeZoneInfo.ConvertTime(dateTimeUtc, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
            }
            catch // Linux platforms will trigger this. Fixed in .Net 6 with cross-platform support for Windows strings
            {
                return dateTimeUtc;
            }
        }
    }
}
