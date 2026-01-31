using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Withings.NET.Models;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET.Client
{
    public class Authenticator
    {
        private static readonly OAuthBase _oAuth = new OAuthBase();
        private static readonly HttpClient _httpClient = new HttpClient();

        readonly string _consumerKey;
        readonly string _consumerSecret;
        readonly string _callbackUrl;

        public Authenticator(WithingsCredentials credentials)
        {
          _consumerKey = credentials.ConsumerKey;
          _consumerSecret = credentials.ConsumerSecret;
          _callbackUrl = credentials.CallbackUrl;
        }

        public async Task<OAuthToken> GetRequestToken()
        {
          var url = new Uri("https://oauth.withings.com/account/request_token"
              + "?oauth_callback=" + Uri.EscapeDataString(_callbackUrl ?? ""));

          string nonce = _oAuth.GenerateNonce();
          string timeStamp = _oAuth.GenerateTimeStamp();
          string normalizedUrl;
          string normalizedParams;
          string signature = _oAuth.GenerateSignature(url, _consumerKey, _consumerSecret,
              null, null, "GET", timeStamp, nonce,
              OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedParams);

          var requestUrl = normalizedUrl + "?" + normalizedParams
              + "&oauth_signature=" + Uri.EscapeDataString(signature);

          var response = await _httpClient.GetStringAsync(requestUrl).ConfigureAwait(false);
          var parsed = ParseResponseParameters(response);

          return new OAuthToken(parsed["oauth_token"], parsed["oauth_token_secret"]);
        }

        public string UserRequestUrl(OAuthToken token)
        {
          return "https://oauth.withings.com/account/authorize?oauth_token=" + Uri.EscapeDataString(token.Key);
        }

        public async Task<OAuthToken> ExchangeRequestTokenForAccessToken(OAuthToken requestToken, string oAuthVerifier)
        {
            var url = new Uri("https://oauth.withings.com/account/access_token");

            string nonce = _oAuth.GenerateNonce();
            string timeStamp = _oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string normalizedParams;
            string signature = _oAuth.GenerateSignature(url, _consumerKey, _consumerSecret,
                requestToken.Key, requestToken.Secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedParams);

            var requestUrl = normalizedUrl + "?" + normalizedParams
                + "&oauth_verifier=" + Uri.EscapeDataString(oAuthVerifier)
                + "&oauth_signature=" + Uri.EscapeDataString(signature);

            var response = await _httpClient.GetStringAsync(requestUrl).ConfigureAwait(false);
            var parsed = ParseResponseParameters(response);

            return new OAuthToken(parsed["oauth_token"], parsed["oauth_token_secret"]);
        }

        private static Dictionary<string, string> ParseResponseParameters(string response)
        {
            var result = new Dictionary<string, string>();
            foreach (var pair in response.Split('&'))
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                    result[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
            }
            return result;
        }
    }
}
