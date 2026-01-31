using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Withings.NET.Models;

namespace Withings.NET.Client
{
    public class Authenticator
    {
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _callbackUrl;
        const string BaseUri = "https://wbsapi.withings.net/v2";

        public Authenticator(WithingsCredentials credentials)
        {
            _clientId = credentials.ClientId;
            _clientSecret = credentials.ClientSecret;
            _callbackUrl = credentials.CallbackUrl;
        }

        public string GetAuthorizeUrl(string state, string scope, string redirectUri = null)
        {
            var uri = "https://account.withings.com/oauth2_user/authorize2"
                .SetQueryParam("response_type", "code")
                .SetQueryParam("client_id", _clientId)
                .SetQueryParam("scope", scope)
                .SetQueryParam("state", state)
                .SetQueryParam("redirect_uri", redirectUri ?? _callbackUrl);

            return uri.ToString();
        }

        public async Task<AuthResponse> GetAccessToken(string code, string redirectUri = null)
        {
            var nonce = await GetNonce();
            var action = "requesttoken";
            var grantType = "authorization_code";
            var rUri = redirectUri ?? _callbackUrl;

            var signature = GenerateSignature(action, _clientId, nonce);

            var response = await (BaseUri + "/oauth2")
                .PostUrlEncodedAsync(new
                {
                    action,
                    client_id = _clientId,
                    grant_type = grantType,
                    code,
                    redirect_uri = rUri,
                    nonce,
                    signature
                })
                .ReceiveJson<ResponseWrapper<AuthResponse>>();

            return response.Body;
        }

        public async Task<AuthResponse> RefreshAccessToken(string refreshToken)
        {
            var nonce = await GetNonce();
            var action = "requesttoken";
            var grantType = "refresh_token";

            var signature = GenerateSignature(action, _clientId, nonce);

            var response = await (BaseUri + "/oauth2")
                .PostUrlEncodedAsync(new
                {
                    action,
                    client_id = _clientId,
                    grant_type = grantType,
                    refresh_token = refreshToken,
                    nonce,
                    signature
                })
                .ReceiveJson<ResponseWrapper<AuthResponse>>();

            return response.Body;
        }

        private async Task<string> GetNonce()
        {
            var action = "getnonce";
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var signature = GenerateSignature(action, _clientId, timestamp);

            var response = await (BaseUri + "/signature")
                .PostUrlEncodedAsync(new
                {
                    action,
                    client_id = _clientId,
                    timestamp,
                    signature
                })
                .ReceiveJson<ResponseWrapper<NonceResponse>>();

            return response.Body.Nonce;
        }

        private string GenerateSignature(string action, string clientId, object thirdParam)
        {
            // For getnonce: action, client_id, timestamp
            // For requesttoken: action, client_id, nonce
            // The documentation says "Concatenate the sorted values (alphabetically by key name)".
            // In both cases, the keys are:
            // 1. action
            // 2. client_id
            // 3. timestamp OR nonce
            // alphabetically: action, client_id, nonce OR action, client_id, timestamp

            // So the order is always action, clientId, thirdParam.

            var data = $"{action},{clientId},{thirdParam}";

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_clientSecret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private class ResponseWrapper<T>
        {
            public int Status { get; set; }
            public T Body { get; set; }
        }

        private class NonceResponse
        {
            public string Nonce { get; set; }
        }
    }
}
