using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
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
            var query = BuildQueryString(new[]
            {
                new KeyValuePair<string, string>("response_type", "code"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("state", state),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("redirect_uri", _callbackUrl)
            });

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

            return await SendTokenRequest(content).ConfigureAwait(false);
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

            return await SendTokenRequest(content).ConfigureAwait(false);
        }

        private async Task<OAuthToken> SendTokenRequest(FormUrlEncodedContent content)
        {
            var httpResponse = await _httpClient.PostAsync(TokenUrl, content).ConfigureAwait(false);
            httpResponse.EnsureSuccessStatusCode();

            using var stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var response = await JsonSerializer.DeserializeAsync<WithingsResponse<OAuthToken>>(stream).ConfigureAwait(false);

            if (response == null)
            {
                 throw new WithingsApiException(-1, "Empty response from Withings API");
            }

            if (response.Status != 0)
            {
                 throw new WithingsApiException(response.Status);
            }

            return response.Body;
        }

        private static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            var query = new StringBuilder();
            foreach (var parameter in queryParams)
            {
                if (query.Length > 0)
                {
                    query.Append('&');
                }

                query
                    .Append(Uri.EscapeDataString(parameter.Key))
                    .Append('=')
                    .Append(Uri.EscapeDataString(parameter.Value ?? string.Empty));
            }

            return query.ToString();
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
