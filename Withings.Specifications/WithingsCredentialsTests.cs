using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Models;

namespace Withings.Specifications
{
    [TestFixture]
    public class WithingsCredentialsTests
    {
        [Test]
        public void ClientProperties_MapToConsumerProperties()
        {
            var credentials = new WithingsCredentials();

            credentials.SetClientProperties("client_id", "client_secret");

            credentials.ClientId.Should().Be("client_id");
            credentials.ClientSecret.Should().Be("client_secret");
            credentials.ConsumerKey.Should().Be("client_id");
            credentials.ConsumerSecret.Should().Be("client_secret");
        }

        [Test]
        public void ConsumerProperties_MapToClientProperties()
        {
            var credentials = new WithingsCredentials();

            credentials.SetConsumerProperties("consumer_key", "consumer_secret");

            credentials.ConsumerKey.Should().Be("consumer_key");
            credentials.ConsumerSecret.Should().Be("consumer_secret");
            credentials.ClientId.Should().Be("consumer_key");
            credentials.ClientSecret.Should().Be("consumer_secret");
        }

        [Test]
        public void SetCallbackUrl_SetsCallbackUrl()
        {
            var credentials = new WithingsCredentials();

            credentials.SetCallbackUrl("https://example.com/callback");

            credentials.CallbackUrl.Should().Be("https://example.com/callback");
        }
    }
}
