using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;

namespace Withings.Specifications
{
    [TestFixture]
    public class AuthenticatorSuccessTests
    {
        private class FakeHandler : HttpMessageHandler
        {
            private readonly string _responseJson;
            public HttpRequestMessage LastRequest { get; private set; }

            public FakeHandler(object responseBody)
            {
                _responseJson = JsonSerializer.Serialize(responseBody);
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, System.Text.Encoding.UTF8, "application/json")
                });
            }
        }

        [Test]
        public void GetAuthCodeUrl_ContainsAllRequiredParameters()
        {
            var credentials = new WithingsCredentials();
            credentials.SetConsumerProperties("my_client_id", "my_secret");
            credentials.SetCallbackUrl("http://localhost/callback");
            var authenticator = new Authenticator(credentials);

            var url = authenticator.GetAuthCodeUrl("user.info,user.metrics", "my_state");

            url.Should().StartWith("https://account.withings.com/oauth2_user/authorize2?");
            url.Should().Contain("response_type=code");
            url.Should().Contain("client_id=my_client_id");
            url.Should().Contain("state=my_state");
            url.Should().Contain("scope=user.info");
            url.Should().Contain("redirect_uri=");
        }

        [Test]
        public async Task GetAccessToken_SuccessResponse_ReturnsOAuthToken()
        {
            var handler = new FakeHandler(new
            {
                status = 0,
                body = new
                {
                    access_token = "access_123",
                    refresh_token = "refresh_456",
                    expires_in = 3600,
                    scope = "user.info",
                    token_type = "Bearer",
                    userid = "user_789"
                }
            });
            var authenticator = CreateAuthenticator(handler);

            var token = await authenticator.GetAccessToken("valid_code");

            token.Should().NotBeNull();
            token.AccessToken.Should().Be("access_123");
            token.RefreshToken.Should().Be("refresh_456");
            token.ExpiresIn.Should().Be(3600);
            token.Scope.Should().Be("user.info");
            token.TokenType.Should().Be("Bearer");
            token.UserId.Should().Be("user_789");
        }

        [Test]
        public async Task GetAccessToken_SendsCorrectFormData()
        {
            var handler = new FakeHandler(new { status = 0, body = new { access_token = "t" } });
            var authenticator = CreateAuthenticator(handler);

            await authenticator.GetAccessToken("auth_code_123");

            handler.LastRequest.Method.Should().Be(HttpMethod.Post);
            var content = await handler.LastRequest.Content.ReadAsStringAsync();
            content.Should().Contain("action=requesttoken");
            content.Should().Contain("grant_type=authorization_code");
            content.Should().Contain("client_id=test_id");
            content.Should().Contain("client_secret=test_secret");
            content.Should().Contain("code=auth_code_123");
            content.Should().Contain("redirect_uri=");
        }

        [Test]
        public async Task RefreshAccessToken_SuccessResponse_ReturnsOAuthToken()
        {
            var handler = new FakeHandler(new
            {
                status = 0,
                body = new
                {
                    access_token = "new_access",
                    refresh_token = "new_refresh",
                    expires_in = 7200,
                    scope = "user.metrics",
                    token_type = "Bearer",
                    userid = "user_789"
                }
            });
            var authenticator = CreateAuthenticator(handler);

            var token = await authenticator.RefreshAccessToken("old_refresh_token");

            token.Should().NotBeNull();
            token.AccessToken.Should().Be("new_access");
            token.RefreshToken.Should().Be("new_refresh");
            token.ExpiresIn.Should().Be(7200);
        }

        [Test]
        public async Task RefreshAccessToken_SendsCorrectFormData()
        {
            var handler = new FakeHandler(new { status = 0, body = new { access_token = "t" } });
            var authenticator = CreateAuthenticator(handler);

            await authenticator.RefreshAccessToken("refresh_token_abc");

            var content = await handler.LastRequest.Content.ReadAsStringAsync();
            content.Should().Contain("action=requesttoken");
            content.Should().Contain("grant_type=refresh_token");
            content.Should().Contain("refresh_token=refresh_token_abc");
        }

        private static Authenticator CreateAuthenticator(FakeHandler handler)
        {
            var httpClient = new HttpClient(handler);
            var credentials = new WithingsCredentials();
            credentials.SetConsumerProperties("test_id", "test_secret");
            credentials.SetCallbackUrl("http://localhost/callback");
            return new Authenticator(credentials, httpClient);
        }
    }
}
