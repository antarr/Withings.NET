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
            ((double)1491868800).FromUnixTime().Date.Should().Be(DateTime.Parse("04/11/2017"));
        }

        [Test]
        public void LongFromUnixTimeTest()
        {
            ((long)1491868800).FromUnixTime().Date.Should().Be(DateTime.Parse("04/11/2017"));
        }

        [Test]
        public void IntFromUnixTimeTest()
        {
            1491868800.FromUnixTime().Date.Should().Be(DateTime.Parse("04/11/2017"));
        }

        [Test]
        public void DateTimeToUnixTimeTest()
        {
            // Note: 04/11/2017 in UTC might depend on local time if not specified,
            // but DateTime.Parse uses local time, and ToUnixTime in code uses UTC epoch.
            // The code in DateTimeExtensions.cs:
            // return Convert.ToInt64((date - epoch).TotalSeconds);
            // epoch is UTC.
            // If date is Unspecified (from Parse), subtraction treats it as same kind (or assumes local?).

            // Let's use a fixed UTC date to be sure.
            var date = new DateTime(2017, 4, 11, 0, 0, 0, DateTimeKind.Utc);
            // 1491868800
            date.ToUnixTime().Should().Be(1491868800);
        }
    }
}
