using System.Threading.Tasks;
using AsyncOAuth;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications
{
    [TestFixture]
    public class AuthenticatorTests
    {
        Authenticator _authenticator;
        WithingsCredentials credentials = new WithingsCredentials();
        private RequestToken _requestToken;
        [SetUp]
        public async Task Init()
        {
            credentials = new WithingsCredentials();
            credentials.SetCallbackUrl("http://localhost:56617/api/oauth/callback");
            credentials.SetConsumerProperties("fb97731ef7cc787067ff5912d13663520e9428038044d198ded8d3009c52", "36b51e76c54f49558de84756c1c613b9ec450011b6481e6424dfe905bcb3c6");
            _authenticator = new Authenticator(credentials);
            _requestToken = await _authenticator.GetRequestToken(credentials.CallbackUrl);
         }

        [Test]
        public void RequestTokenTest()
        {
            Assert.IsNotEmpty(_requestToken.Key);
            Assert.IsNotNull(_requestToken.Key);
            Assert.IsNotEmpty(_requestToken.Secret);
            Assert.IsNotNull(_requestToken.Secret);
        }

        [Test]
        public void AuthorizeUrlTest()
        {
            var url = _authenticator.UserRequestUrl(_requestToken);
            Assert.IsNotNull(url);
            Assert.IsNotEmpty(url);
        }
    }
}
