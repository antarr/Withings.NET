using Machine.Specifications;
using Withings.NET.Client;

namespace When
{
	[Tags("Integration")]
    public class GettingRequestToken
	{
		static WithingsClient Subject;
	    static string RequestUrl;

        Because GetRequstTokenHasNotBeenCalled = () => Subject = new WithingsClient();

		It ShouldHaveANullOauthToken = () => Subject.OauthToken.ShouldBeNull();

        Because AUserRequestUrlsHasBeRequest = () => Subject.ShouldEqual(Subject);

        It ShouldNotReturnANull = () => Subject.GetUserRequstUrl().ShouldNotBeNull();

	    //It ShouldNotReturnAEmptyRequestUrl = Subject.GetUserRequstUrl().ShouldNotBeEmpty;
	}
}
