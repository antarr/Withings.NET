using System;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;

namespace Withings.Specifications
{
    [TestFixture]
    public class AuthenticatorTests
    {
        Authenticator _authenticator;
        WithingsCredentials _credentials;
        HttpTest _httpTest;

        [SetUp]
        public void Init()
        {
            _credentials = new WithingsCredentials();
            _credentials.SetCallbackUrl("http://localhost/callback");
            _credentials.SetClientProperties("test_client_id", "test_client_secret");
            _authenticator = new Authenticator(_credentials);
            _httpTest = new HttpTest();
        }

        [TearDown]
        public void Dispose()
        {
            _httpTest.Dispose();
        }

        [Test]
        public void GetAuthorizeUrlTest()
        {
            var url = _authenticator.GetAuthorizeUrl("test_state", "user.info", "http://localhost/callback");
            url.Should().StartWith("https://account.withings.com/oauth2_user/authorize2");
            url.Should().Contain("client_id=test_client_id");
            url.Should().Contain("scope=user.info");
            url.Should().Contain("state=test_state");
            url.Should().Contain("redirect_uri=http%3A%2F%2Flocalhost%2Fcallback");
        }

        [Test]
        public async Task GetAccessTokenTest()
        {
            // Mock getnonce response
            _httpTest.RespondWithJson(new { status = 0, body = new { nonce = "test_nonce" } });
            // Mock requesttoken response
            _httpTest.RespondWithJson(new
            {
                status = 0,
                body = new
                {
                    access_token = "test_access_token",
                    refresh_token = "test_refresh_token",
                    expires_in = 10800,
                    scope = "user.info",
                    token_type = "Bearer",
                    userid = "123"
                }
            });

            var response = await _authenticator.GetAccessToken("test_code");

            response.Should().NotBeNull();
            response.AccessToken.Should().Be("test_access_token");
            response.RefreshToken.Should().Be("test_refresh_token");
            response.UserId.Should().Be("123");

            // Verify calls
            _httpTest.ShouldHaveCalled("https://wbsapi.withings.net/v2/signature")
                .WithVerb(System.Net.Http.HttpMethod.Post);

            _httpTest.ShouldHaveCalled("https://wbsapi.withings.net/v2/oauth2")
                .WithVerb(System.Net.Http.HttpMethod.Post)
                .WithRequestBody("*grant_type=authorization_code*")
                .WithRequestBody("*code=test_code*")
                .WithRequestBody("*nonce=test_nonce*");
        }

        [Test]
        public async Task RefreshAccessTokenTest()
        {
            // Mock getnonce response
            _httpTest.RespondWithJson(new { status = 0, body = new { nonce = "test_nonce" } });
            // Mock requesttoken response
            _httpTest.RespondWithJson(new
            {
                status = 0,
                body = new
                {
                    access_token = "new_access_token",
                    refresh_token = "new_refresh_token",
                    expires_in = 10800,
                    scope = "user.info",
                    token_type = "Bearer",
                    userid = "123"
                }
            });

            var response = await _authenticator.RefreshAccessToken("old_refresh_token");

            response.Should().NotBeNull();
            response.AccessToken.Should().Be("new_access_token");

            // Verify calls
            _httpTest.ShouldHaveCalled("https://wbsapi.withings.net/v2/signature");

            _httpTest.ShouldHaveCalled("https://wbsapi.withings.net/v2/oauth2")
                .WithRequestBody("*grant_type=refresh_token*")
                .WithRequestBody("*refresh_token=old_refresh_token*");
        }
    }
}
