using System;
using Withings.NET;
using Machine.Specifications;
namespace When
{
	[Subject(typeof(WithingsClient))]
	public class GettingRequestToken
	{
		static WithingsClient Subject;

		Because GetRequstTokenHasNotBeenCalled = () => Subject = new WithingsClient();

		It ShouldHaveANullOauthToken = () => Subject.oauthToken.ShouldBeNull();
	}
}
