using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;

namespace Withings.Specifications
{
    [TestFixture]
    public class WithingsClientTests
    {
        private CapturingHandler _handler;
        private WithingsClient _client;

        private class CapturingHandler : HttpMessageHandler
        {
            public HttpRequestMessage LastRequest { get; private set; }
            private readonly string _responseJson;

            public CapturingHandler(object responseBody)
            {
                _responseJson = JsonSerializer.Serialize(responseBody);
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, System.Text.Encoding.UTF8, "application/json")
                });
            }
        }

        [TearDown]
        public void TearDown()
        {
            _handler?.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _handler = new CapturingHandler(new { status = 0, body = new { value = 42 } });
            var httpClient = new HttpClient(_handler);
            var credentials = new WithingsCredentials();
            credentials.SetConsumerProperties("id", "secret");
            _client = new WithingsClient(credentials, httpClient);
        }

        [Test]
        public async Task GetActivityMeasures_WithDateRange_BuildsCorrectUrl()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);

            await _client.GetActivityMeasures(start, end, "user123", "token123");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("measure");
            url.Should().Contain("action=getactivity");
            url.Should().Contain("userid=user123");
            url.Should().Contain("startdateymd=2024-01-01");
            url.Should().Contain("enddateymd=2024-01-31");
        }

        [Test]
        public async Task GetActivityMeasures_WithDateRange_SetsBearerToken()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);

            await _client.GetActivityMeasures(start, end, "user123", "my_token");

            _handler.LastRequest.Headers.Authorization.Scheme.Should().Be("Bearer");
            _handler.LastRequest.Headers.Authorization.Parameter.Should().Be("my_token");
        }

        [Test]
        public async Task GetActivityMeasures_WithSingleDate_BuildsCorrectUrl()
        {
            var date = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);

            await _client.GetActivityMeasures(date, "user456", "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("action=getactivity");
            url.Should().Contain("userid=user456");
            url.Should().Contain("date=2024-03-15");
        }

        [Test]
        public async Task GetSleepSummary_BuildsCorrectUrl()
        {
            await _client.GetSleepSummary("2024-01-01", "2024-01-31", "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("sleep");
            url.Should().Contain("action=getsummary");
            url.Should().Contain("startdateymd=2024-01-01");
            url.Should().Contain("enddateymd=2024-01-31");
        }

        [Test]
        public async Task GetSleepMeasures_BuildsCorrectUrlWithUnixTimestamps()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            await _client.GetSleepMeasures("user1", start, end, "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("sleep");
            url.Should().Contain("action=get");
            url.Should().Contain($"startdate={start.ToUnixTime()}");
            url.Should().Contain($"enddate={end.ToUnixTime()}");
        }

        [Test]
        public async Task GetWorkouts_BuildsCorrectUrl()
        {
            await _client.GetWorkouts("2024-02-01", "2024-02-28", "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("measure");
            url.Should().Contain("action=getworkouts");
            url.Should().Contain("startdateymd=2024-02-01");
            url.Should().Contain("enddateymd=2024-02-28");
        }

        [Test]
        public async Task GetIntraDayActivity_BuildsCorrectUrlWithUnixTimestamps()
        {
            var start = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 1, 1, 20, 0, 0, DateTimeKind.Utc);

            await _client.GetIntraDayActivity("user1", start, end, "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("action=getintradayactivity");
            url.Should().Contain("userid=user1");
            url.Should().Contain($"startdate={start.ToUnixTime()}");
            url.Should().Contain($"enddate={end.ToUnixTime()}");
        }

        [Test]
        public async Task GetBodyMeasures_WithDateRange_BuildsCorrectUrl()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);

            await _client.GetBodyMeasures("user1", start, end, "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("action=getmeas");
            url.Should().Contain("userid=user1");
            url.Should().Contain($"startdate={start.ToUnixTime()}");
            url.Should().Contain($"enddate={end.ToUnixTime()}");
        }

        [Test]
        public async Task GetBodyMeasures_WithLastUpdate_BuildsCorrectUrl()
        {
            var lastUpdate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);

            await _client.GetBodyMeasures("user1", lastUpdate, "token");

            var url = _handler.LastRequest.RequestUri.ToString();
            url.Should().Contain("action=getmeas");
            url.Should().Contain("userid=user1");
            url.Should().Contain($"lastupdate={lastUpdate.ToUnixTime()}");
        }

        [Test]
        public async Task GetActivityMeasures_ReturnsExpandoObject()
        {
            var start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);

            var result = await _client.GetActivityMeasures(start, end, "user1", "token");

            result.Should().NotBeNull();
            result.Should().BeOfType<ExpandoObject>();
        }
    }
}
