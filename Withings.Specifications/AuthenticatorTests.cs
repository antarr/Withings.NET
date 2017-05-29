using System;
using System.Threading.Tasks;
using AsyncOAuth;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;

namespace Withings.Specifications
{
    [TestFixture]
    public class AuthenticatorTests
    {
        Authenticator _authenticator;
        WithingsCredentials _credentials;
        RequestToken _requestToken;

        [SetUp]
        public async Task Init()
        {
            _credentials = new WithingsCredentials();
            _credentials.SetCallbackUrl("http://localhost:56617/api/oauth/callback");
            _credentials.SetConsumerProperties("", "");
            _authenticator = new Authenticator(_credentials);
            _requestToken = await _authenticator.GetRequestToken();
         }

        [Test]
        public void RequestTokenTest()
        {
            _requestToken.Key.Should().NotBeNullOrEmpty();
            _requestToken.Secret.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void AuthorizeUrlTest()
        {
            var url = _authenticator.UserRequestUrl(_requestToken);
            url.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void InvalidAuthorizeUrlTest()
        {
            Assert.Throws<ArgumentNullException>(() => _authenticator.UserRequestUrl(null));
        }

        [Test]
        public void ExchangeInvalidRequestTokenForAccessTokenTest()
        {
            Assert.Throws<AggregateException>(InvalidExchangeRequestForAccessToken);
        }

        void InvalidExchangeRequestForAccessToken()
        {
            var unused = _authenticator.ExchangeRequestTokenForAccessToken(_requestToken, _requestToken.Secret).Result;
        }
    }
}
