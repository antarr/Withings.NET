using System.Configuration;
using AsyncOAuth;
using FluentAssertions;
using Material.Infrastructure.Credentials;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Net.Specifications
{
    [TestFixture]
    public class AuthenticatorSpec
	  {
        static Authenticator _subject;
	      ////static string requestUrl;
	      static RequestToken _requestToken;
        readonly OAuth1Credentials _credentials = new OAuth1Credentials();

        [OneTimeSetUp]
        void before_all()
	      {
            _credentials.SetConsumerProperties
            (
                ConfigurationManager.AppSettings["WithingsConsumerKey"],
                ConfigurationManager.AppSettings["WithingsConsumerSecret"]
	          );
            _credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);
            _subject = new Authenticator(_credentials);
            _requestToken = _subject.GetRequestToken(_credentials.CallbackUrl).Result;
        }

    [Test]
    void describe_getting_request_token()
	      {
	          before = async () => _requestToken = await _subject.GetRequestToken(_credentials.CallbackUrl);

            it["should have a key"] = () =>
            {
                _requestToken.Key.Should().NotBeNullOrWhiteSpace();
            };

            it["should have a secret"] = () =>
            {
               _requestToken.Secret.Should().NotBeNullOrWhiteSpace();
            };
        }

	   //// Because AsAUserICalledUserRequestUrl = () => requestUrl = subject.UserRequestUrl(requestToken);

    ////  It ShouldNotHaveReturnedANullUrl = () => requestUrl.ShouldNotBeNull();

		  ////It ShouldNotHaveReturnedAEmptyUrl = () => requestUrl.ShouldNotBeEmpty();

		  
	}
}
