using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;

namespace Withings.Specifications
{
    [TestFixture]
    [Explicit("Requires Withings credentials and network access")]
    public class AuthenticatorTests
    {
        Authenticator _authenticator;
        WithingsCredentials _credentials;

        [SetUp]
        public void Init()
        {
            _credentials = new WithingsCredentials();
            _credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WithingsCallbackUrl") ?? "http://localhost:8080/api/oauth/callback");
            _credentials.SetConsumerProperties(Environment.GetEnvironmentVariable("WithingsConsumerKey") ?? "key", Environment.GetEnvironmentVariable("WithingsConsumerSecret") ?? "secret");
            _authenticator = new Authenticator(_credentials);
         }

        [Test]
        public void GetAuthCodeUrlTest()
        {
            var url = _authenticator.GetAuthCodeUrl("user.info,user.metrics", "state");
            url.Should().NotBeNullOrEmpty();
            url.Should().Contain("response_type=code");
            url.Should().Contain("client_id=");
            url.Should().Contain("scope=user.info%2Cuser.metrics");
        }

        [Test]
        public void InvalidExchangeRequestForAccessToken()
        {
            Assert.ThrowsAsync<Exception>(async () => await _authenticator.GetAccessToken("invalid_code"));
        }
    }
}
