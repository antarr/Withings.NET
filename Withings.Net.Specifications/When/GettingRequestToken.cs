using System.Configuration;
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

			_subject = new Authenticator(credentials);
		};

	    Because AsAUserICalledUserRequestUrl = async () => _requestUrl = await _subject.UserRequstUrl("machine.specifications").ConfigureAwait(true);

        It ShouldNotHaveReturnedANullUrl = () => _requestUrl.ShouldNotBeNull();

		It ShouldNotHaveReturnedAEmptyUrl = () => _requestUrl.ShouldNotBeEmpty();

		static Authenticator _subject;
		static string _requestUrl;
	}
}
