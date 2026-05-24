using System.Text.Json;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class UserIdConverterTests
    {
        [Test]
        public void Deserialize_UserIdNumber_ReadsStringValue()
        {
            var token = JsonSerializer.Deserialize<OAuthToken>("{\"userid\":123456789}");

            token.UserId.Should().Be("123456789");
        }

        [Test]
        public void Deserialize_UserIdString_ReadsStringValue()
        {
            var token = JsonSerializer.Deserialize<OAuthToken>("{\"userid\":\"user_123\"}");

            token.UserId.Should().Be("user_123");
        }

        [Test]
        public void Serialize_UserId_WritesStringValue()
        {
            var json = JsonSerializer.Serialize(new OAuthToken { UserId = "123456789" });

            json.Should().Contain("\"userid\":\"123456789\"");
        }
    }
}
