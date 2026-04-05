using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;
using Withings.Specifications.Helpers;

namespace Withings.Specifications
{
    [TestFixture]
    [Category("E2E")]
    public class AuthenticatorTests
    {
        Authenticator _authenticator;
        WithingsCredentials _credentials;

        [SetUp]
        public void Init()
        {
            EnvLoader.Load();

            _credentials = new WithingsCredentials();
            _credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WITHINGS_CALLBACK_URL") ?? "http://localhost:8080/api/oauth/callback");
            _credentials.SetConsumerProperties(
                Environment.GetEnvironmentVariable("WITHINGS_CLIENT_ID") ?? "key",
                Environment.GetEnvironmentVariable("WITHINGS_CLIENT_SECRET") ?? "secret");
            _authenticator = new Authenticator(_credentials);
         }

        [Test]
        public void GetAuthCodeUrlTest()
        {
            var url = _authenticator.GetAuthCodeUrl("user.info,user.metrics", "state");
            url.Should().NotBeNullOrEmpty();
            url.Should().Contain("response_type=code");
            url.Should().Contain("client_id=");
        }

        [Test]
        public void InvalidExchangeRequestForAccessToken()
        {
            Assert.ThrowsAsync<WithingsApiException>(async () => await _authenticator.GetAccessToken("invalid_code"));
        }
    }
}
