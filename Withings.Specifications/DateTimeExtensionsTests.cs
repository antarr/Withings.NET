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
    }
}