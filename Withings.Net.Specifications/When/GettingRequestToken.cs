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

			Subject = new WithingsClient(credentials);
		};

	    Because As_a_user_i_called_user_request_url = async () => RequestUrl = await Subject.UserRequstUrl("machine.specifications");

        It Should_not_have_returned_a_null_url = () => RequestUrl.ShouldNotBeNull();

		It Should_not_have_returned_a_empty_url = () => RequestUrl.ShouldNotBeEmpty();

		static WithingsClient Subject;
		static string RequestUrl;
	}
}
