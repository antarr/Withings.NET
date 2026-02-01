using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Withings.NET.Models;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET.Client
{
    public class Authenticator
    {
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _callbackUrl;

        const string AuthorizeUrl = "https://account.withings.com/oauth2_user/authorize2";
        const string TokenUrl = "https://wbsapi.withings.net/v2/oauth2";

        public Authenticator(WithingsCredentials credentials)
        {
          _clientId = credentials.ClientId;
          _clientSecret = credentials.ClientSecret;
          _callbackUrl = credentials.CallbackUrl;
        }

        public string GetAuthCodeUrl(string scope, string state)
        {
            return AuthorizeUrl
                .SetQueryParam("response_type", "code")
                .SetQueryParam("client_id", _clientId)
                .SetQueryParam("state", state)
                .SetQueryParam("scope", scope)
                .SetQueryParam("redirect_uri", _callbackUrl)
                .ToString();
        }

        public async Task<OAuthToken> GetAccessToken(string code)
        {
            var response = await TokenUrl
                .PostUrlEncodedAsync(new
                {
                    action = "requesttoken",
                    grant_type = "authorization_code",
                    client_id = _clientId,
                    client_secret = _clientSecret,
                    code = code,
                    redirect_uri = _callbackUrl
                })
                .ReceiveJson<WithingsResponse<OAuthToken>>()
                .ConfigureAwait(false);

            if (response.Status != 0)
            {
                 throw new Exception($"Withings API Error: {response.Status} - {response.Error}");
            }

            return response.Body;
        }

        public async Task<OAuthToken> RefreshAccessToken(string refreshToken)
        {
             var response = await TokenUrl
                .PostUrlEncodedAsync(new
                {
                    action = "requesttoken",
                    grant_type = "refresh_token",
                    client_id = _clientId,
                    client_secret = _clientSecret,
                    refresh_token = refreshToken
                })
                .ReceiveJson<WithingsResponse<OAuthToken>>()
                .ConfigureAwait(false);

            if (response.Status != 0)
            {
                 throw new Exception($"Withings API Error: {response.Status} - {response.Error}");
            }

            return response.Body;
        }

        private class WithingsResponse<T>
        {
            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("body")]
            public T Body { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }
    }
}
