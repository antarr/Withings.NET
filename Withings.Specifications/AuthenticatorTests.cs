using System;
using System.Threading.Tasks;
using AsyncOAuth;
using FluentAssertions;
using Material.Infrastructure.Credentials;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class AuthenticatorTests
    {
        Authenticator _authenticator;
        OAuth1Credentials _credentials;
        private RequestToken _requestToken;
        [SetUp]
        public async Task Init()
        {
            _credentials = new OAuth1Credentials();
            _credentials.SetCallbackUrl("http://localhost:56617/api/oauth/callback");
            _credentials.SetConsumerProperties("fb97731ef7cc787067ff5912d13663520e9428038044d198ded8d3009c52", "36b51e76c54f49558de84756c1c613b9ec450011b6481e6424dfe905bcb3c6");
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

        private void InvalidExchangeRequestForAccessToken()
        {
            var unused = _authenticator.ExchangeRequestTokenForAccessToken(_requestToken, _requestToken.Secret).Result;
        }
    }
}
