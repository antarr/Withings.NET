using System;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void DoubleFromUnixTimeTest()
        {
            ((double)1491934309).FromUnixTime().Date.Should().Be(DateTime.Parse("04/11/2017"));
        }

        [Test]
        public void LongFromUnixTimeTest()
        {
            ((long)1491934309).FromUnixTime().Date.Should().Be(DateTime.Parse("04/11/2017"));
        }

        [Test]
        public void IntFromUnixTimeTest()
        {
            1491934309.FromUnixTime().Date.Should().Be(DateTime.Parse("04/11/2017"));
        }

        [Test]
        public void DateTimeToUnixTimeTest()
        {
            DateTime.Parse("04/11/2017").ToUnixTime().Should().Be(1491868800);
        }

        [Test]
        public void DateTimeToUnixTimeTest_Unspecified()
        {
            // Unspecified dates are treated as UTC for legacy compatibility
            var date = new DateTime(2017, 4, 11, 0, 0, 0, DateTimeKind.Unspecified);
            date.ToUnixTime().Should().Be(1491868800);
        }

        [Test]
        public void DateTimeToUnixTimeTest_Utc()
        {
            var date = new DateTime(2017, 4, 11, 0, 0, 0, DateTimeKind.Utc);
            date.ToUnixTime().Should().Be(1491868800);
        }

        [Test]
        public void DateTimeToUnixTimeTest_Local()
        {
            // Local dates should be correctly converted to UTC before Unix timestamp calculation
            // This behavior differs from legacy implementation (which treated Local as UTC),
            // but is correct according to DateTimeOffset.ToUnixTimeSeconds() standard.
            var date = new DateTime(2017, 4, 11, 0, 0, 0, DateTimeKind.Local);
            var expected = new DateTimeOffset(date).ToUnixTimeSeconds();
            date.ToUnixTime().Should().Be(expected);
        }

        [Test]
        public void DateTimeToUnixTimeTest_InvalidLocalTime_ThrowsArgumentException()
        {
            // Find a DST gap time for the local time zone
            var rules = TimeZoneInfo.Local.GetAdjustmentRules();
            DateTime? invalidTime = null;

            foreach (var rule in rules)
            {
                if (rule.DaylightTransitionStart.IsFixedDateRule)
                    continue;

                var year = rule.DateEnd.Year > 2100 ? 2024 : rule.DateStart.Year;
                var transition = rule.DaylightTransitionStart;
                var month = transition.Month;
                var day = transition.Day;

                // Find the first matching day-of-week in the given week
                var firstDayOfMonth = new DateTime(year, month, 1);
                var daysUntilTarget = ((int)transition.DayOfWeek - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
                var candidateDay = 1 + daysUntilTarget + (transition.Week - 1) * 7;

                if (candidateDay > DateTime.DaysInMonth(year, month))
                    continue;

                var candidate = new DateTime(year, month, candidateDay,
                    transition.TimeOfDay.Hour, transition.TimeOfDay.Minute, 0, DateTimeKind.Local);

                if (TimeZoneInfo.Local.IsInvalidTime(candidate))
                {
                    invalidTime = candidate;
                    break;
                }
            }

            if (invalidTime == null)
            {
                Assert.Ignore("No DST gap found in local time zone");
                return;
            }

            Action act = () => invalidTime.Value.ToUnixTime();
            act.Should().Throw<ArgumentException>();
        }
    }
}
