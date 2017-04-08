using System.Configuration;
using AsyncOAuth;
using Machine.Specifications;
using Material.Infrastructure.Credentials;
using Withings.NET.Client;

namespace When
{
	[Tags("Integration")]
    public class GettingRequestToken
	{
	    private Establish context = () =>
	    {
	        var credentials = new OAuth1Credentials();
	        credentials.SetConsumerProperties
	        (
	            ConfigurationManager.AppSettings["WithingsConsumerKey"],
	            ConfigurationManager.AppSettings["WithingsConsumerSecret"]
	        );
	        credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);

			subject = new Authenticator(credentials);
	        requestToken = subject.GetRequestToken().Result;
        };

	    It ShouldHaveAKey = () => requestToken.Key.ShouldNotBeEmpty();

	    It ShouldHaveASecret = () => requestToken.Secret.ShouldNotBeEmpty();

	    Because AsAUserICalledUserRequestUrl = () => requestUrl = subject.UserRequestUrl(requestToken);

        It ShouldNotHaveReturnedANullUrl = () => requestUrl.ShouldNotBeNull();

		It ShouldNotHaveReturnedAEmptyUrl = () => requestUrl.ShouldNotBeEmpty();

		static Authenticator subject;
		static string requestUrl;
	    static RequestToken requestToken;
	}
}
