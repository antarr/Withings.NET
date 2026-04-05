using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

        private class FakeHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public FakeHandler(object responseBody)
            {
                _responseJson = JsonSerializer.Serialize(responseBody);
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, System.Text.Encoding.UTF8, "application/json")
                });
            }
        }

        private Authenticator CreateAuthenticator(object responseBody)
        {
            var handler = new FakeHandler(responseBody);
            var httpClient = new HttpClient(handler);
            var credentials = new WithingsCredentials();
            credentials.SetConsumerProperties("client_id", "client_secret");
            credentials.SetCallbackUrl("http://localhost");
            return new Authenticator(credentials, httpClient);
        }

        [Test]
        public async Task GetAccessToken_Should_Not_Expose_Error_Detail_In_Exception()
        {
            // Arrange
            _authenticator = CreateAuthenticator(new
            {
                status = 401,
                error = "This is a sensitive error message that should not be exposed"
            });

            // Act
            Func<Task> act = async () => await _authenticator.GetAccessToken("code");

            // Assert
            var exception = await act.Should().ThrowAsync<WithingsApiException>();
            exception.WithMessage("*401*");
            exception.WithMessage("*Withings API Error*");
            exception.Which.Message.Should().NotContain("This is a sensitive error message");
        }

        [Test]
        public async Task RefreshAccessToken_Should_Not_Expose_Error_Detail_In_Exception()
        {
            // Arrange
            _authenticator = CreateAuthenticator(new
            {
                status = 401,
                error = "This is another sensitive error message that should not be exposed"
            });

            // Act
            Func<Task> act = async () => await _authenticator.RefreshAccessToken("refresh_token");

            // Assert
            var exception = await act.Should().ThrowAsync<WithingsApiException>();
            exception.WithMessage("*401*");
            exception.WithMessage("*Withings API Error*");
            exception.Which.Message.Should().NotContain("This is another sensitive error message");
        }
    }
}
