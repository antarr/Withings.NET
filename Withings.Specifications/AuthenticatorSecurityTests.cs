using System;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;
using FluentAssertions;

namespace Withings.Specifications
{
    [TestFixture]
    public class AuthenticatorSecurityTests
    {
        private Authenticator _authenticator;
        private HttpTest _httpTest;

        [SetUp]
        public void Setup()
        {
            _httpTest = new HttpTest();
            var credentials = new WithingsCredentials();
            credentials.SetConsumerProperties("client_id", "client_secret");
            credentials.SetCallbackUrl("http://localhost");
            _authenticator = new Authenticator(credentials);
        }

        [TearDown]
        public void Teardown()
        {
            _httpTest.Dispose();
        }

        [Test]
        public async Task GetAccessToken_Should_Not_Expose_Error_Detail_In_Exception()
        {
            // Arrange
            _httpTest.RespondWithJson(new
            {
                status = 401,
                error = "This is a sensitive error message that should not be exposed"
            });

            // Act
            Func<Task> act = async () => await _authenticator.GetAccessToken("code");

            // Assert
            var exception = await act.Should().ThrowAsync<Exception>();
            exception.WithMessage("*401*");
            exception.WithMessage("*Withings API Error*");
            exception.Which.Message.Should().NotContain("This is a sensitive error message");
        }

        [Test]
        public async Task RefreshAccessToken_Should_Not_Expose_Error_Detail_In_Exception()
        {
            // Arrange
            _httpTest.RespondWithJson(new
            {
                status = 401,
                error = "This is another sensitive error message that should not be exposed"
            });

            // Act
            Func<Task> act = async () => await _authenticator.RefreshAccessToken("refresh_token");

            // Assert
            var exception = await act.Should().ThrowAsync<Exception>();
            exception.WithMessage("*401*");
            exception.WithMessage("*Withings API Error*");
            exception.Which.Message.Should().NotContain("This is another sensitive error message");
        }
    }
}
