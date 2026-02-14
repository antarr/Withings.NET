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
    }
}
