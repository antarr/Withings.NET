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

            if (date.Kind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(date))
            {
                throw new ArgumentException(
                    "The specified local DateTime represents an invalid time in the local time zone (for example, during a DST transition gap) and cannot be converted to Unix time.",
                    nameof(date));
            }
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }
    }
}
