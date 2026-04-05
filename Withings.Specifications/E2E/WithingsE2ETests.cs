using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications.E2E
{
    [TestFixture]
    [Category("E2E")]
    public class WithingsE2ETests
    {
        private WithingsClient _client;
        private string _accessToken;
        private string _userId;

        [OneTimeSetUp]
        public void Setup()
        {
            _accessToken = E2EFixture.AccessToken;
            _userId = E2EFixture.UserId;
            _client = new WithingsClient(E2EFixture.Credentials);

            if (string.IsNullOrEmpty(_accessToken))
                Assert.Ignore("No access token available");
        }

        [Test]
        public async Task GetActivityMeasures_WithDateRange_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-30);
            var end = DateTime.UtcNow;

            var result = await _client.GetActivityMeasures(start, end, _userId, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetActivityMeasures_WithSingleDate_ReturnsResponse()
        {
            var date = DateTime.UtcNow.AddDays(-1);

            var result = await _client.GetActivityMeasures(date, _userId, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetSleepSummary_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
            var end = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var result = await _client.GetSleepSummary(start, end, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetSleepMeasures_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-1);
            var end = DateTime.UtcNow;

            var result = await _client.GetSleepMeasures(_userId, start, end, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetWorkouts_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            var end = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var result = await _client.GetWorkouts(start, end, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetIntraDayActivity_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-1);
            var end = DateTime.UtcNow;

            var result = await _client.GetIntraDayActivity(_userId, start, end, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetBodyMeasures_WithDateRange_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-30);
            var end = DateTime.UtcNow;

            var result = await _client.GetBodyMeasures(_userId, start, end, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetBodyMeasures_WithLastUpdate_ReturnsResponse()
        {
            var lastUpdate = DateTime.UtcNow.AddDays(-30);

            var result = await _client.GetBodyMeasures(_userId, lastUpdate, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetHeartList_ReturnsResponse()
        {
            var start = DateTime.UtcNow.AddDays(-30);
            var end = DateTime.UtcNow;

            var result = await _client.GetHeartList(start, end, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetDevices_ReturnsResponse()
        {
            var result = await _client.GetDevices(_accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task GetGoals_ReturnsResponse()
        {
            var result = await _client.GetGoals(_accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }

        [Test]
        public async Task ListSubscriptions_ReturnsResponse()
        {
            var result = await _client.ListSubscriptions(1, _accessToken);

            result.Should().NotBeNull();
            var dict = (IDictionary<string, object>)result;
            dict.Should().ContainKey("status");
        }
    }
}
