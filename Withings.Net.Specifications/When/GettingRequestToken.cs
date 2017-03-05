using System.Configuration;
using Machine.Specifications;
using Withings.NET.Client;

namespace When
{
	[Tags("Integration")]
    public class GettingRequestToken
	{
		Establish context = () =>
		{
			var credentials = new WithingsCredentials(ConfigurationManager.AppSettings["WithingsConsumerKey"],
													  ConfigurationManager.AppSettings["WithingsConsumerSecret"],
													  ConfigurationManager.AppSettings["WithingsCallbackUrl"]);

			Subject = new Authenticator(credentials);
		};

	    Because AsAUserICalledUserRequestUrl = async () => RequestUrl = await Subject.UserRequstUrl("machine.specifications");

        It ShouldNotHaveReturnedANullUrl = () => RequestUrl.ShouldNotBeNull();

		It ShouldNotHaveReturnedAEmptyUrl = () => RequestUrl.ShouldNotBeEmpty();

		static Authenticator Subject;
		static string RequestUrl;
	}
}
