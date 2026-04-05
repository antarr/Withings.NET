using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Withings.NET.Models;

[assembly: InternalsVisibleTo("Withings.Net.Tests")]
namespace Withings.NET.Client
{
    public class Authenticator
    {
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _callbackUrl;
        readonly HttpClient _httpClient;

        const string AuthorizeUrl = "https://account.withings.com/oauth2_user/authorize2";
        const string TokenUrl = "https://wbsapi.withings.net/v2/oauth2";

        public Authenticator(WithingsCredentials credentials)
            : this(credentials, new HttpClient())
        {
        }

        internal Authenticator(WithingsCredentials credentials, HttpClient httpClient)
        {
          _clientId = credentials.ClientId;
          _clientSecret = credentials.ClientSecret;
          _callbackUrl = credentials.CallbackUrl;
          _httpClient = httpClient;
        }

        public string GetAuthCodeUrl(string scope, string state)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["response_type"] = "code";
            query["client_id"] = _clientId;
            query["state"] = state;
            query["scope"] = scope;
            query["redirect_uri"] = _callbackUrl;

            return $"{AuthorizeUrl}?{query}";
        }

        public async Task<OAuthToken> GetAccessToken(string code)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", "requesttoken"),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _callbackUrl)
            });

            var httpResponse = await _httpClient.PostAsync(TokenUrl, content).ConfigureAwait(false);
            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<WithingsResponse<OAuthToken>>(json);

            if (response.Status != 0)
            {
                 throw new WithingsApiException(response.Status);
            }

            return response.Body;
        }

        public async Task<OAuthToken> RefreshAccessToken(string refreshToken)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", "requesttoken"),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            });

            var httpResponse = await _httpClient.PostAsync(TokenUrl, content).ConfigureAwait(false);
            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<WithingsResponse<OAuthToken>>(json);

            if (response.Status != 0)
            {
                 throw new WithingsApiException(response.Status);
            }

            return response.Body;
        }

        private class WithingsResponse<T>
        {
            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("body")]
            public T Body { get; set; }

            [JsonPropertyName("error")]
            public string Error { get; set; }
        }
    }

}
