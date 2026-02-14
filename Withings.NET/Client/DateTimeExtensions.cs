using System;

namespace Withings.NET.Client
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(this long unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static DateTime FromUnixTime(this double unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static DateTime FromUnixTime(this int unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
            {
                return new DateTimeOffset(date.Ticks, TimeSpan.Zero).ToUnixTimeSeconds();
            }

            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }
    }
}
