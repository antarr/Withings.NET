using System;
using System.Dynamic;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class WithingsClientTests
    {
        WithingsClient _client;
        HttpTest _httpTest;

        [SetUp]
        public void Init()
        {
            _client = new WithingsClient();
            _httpTest = new HttpTest();
        }

        [TearDown]
        public void Dispose()
        {
            _httpTest.Dispose();
        }

        [Test]
        public async Task GetActivityMeasuresTest()
        {
            _httpTest.RespondWithJson(new { status = 0, body = new { some_data = "test" } });

            var start = DateTime.UtcNow.Date;
            var end = DateTime.UtcNow.Date.AddDays(1);

            dynamic result = await _client.GetActivityMeasures(start, end, "userid", "access_token");

            ((object)result).Should().BeOfType<ExpandoObject>();
            // Verify property existence and value (JSON numbers are typically long/int64)
            long status = result.status;
            status.Should().Be(0);

            // Wait, RespondWithJson serializes the object. GetJsonAsync<ExpandoObject> deserializes it.
            // ExpandoObject will have properties matching the json.
            // result is ExpandoObject.
            // But checking properties on ExpandoObject directly needs casting to IDictionary<string, object> or using dynamic.
            // "status" and "body" should be present.
        }
    }
}
