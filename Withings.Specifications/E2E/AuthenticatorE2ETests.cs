using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;

namespace Withings.Specifications.E2E
{
    [TestFixture]
    [Category("E2E")]
    public class AuthenticatorE2ETests
    {
        private Authenticator _authenticator;

        [OneTimeSetUp]
        public void Setup()
        {
            if (E2EFixture.Credentials == null)
                Assert.Ignore("E2E fixture not initialized");

            _authenticator = new Authenticator(E2EFixture.Credentials);
        }

        [Test]
        public void GetAuthCodeUrl_ReturnsValidUrl()
        {
            var url = _authenticator.GetAuthCodeUrl("user.info,user.metrics,user.activity", "test_state");

            url.Should().StartWith("https://account.withings.com/oauth2_user/authorize2?");
            url.Should().Contain("response_type=code");
            url.Should().Contain("client_id=");
            url.Should().Contain("scope=");
        }

        [Test]
        public async Task GetAccessToken_WithInvalidCode_ThrowsException()
        {
            Func<Task> act = async () => await _authenticator.GetAccessToken("invalid_code");

            await act.Should().ThrowAsync<WithingsApiException>();
        }
    }
}
