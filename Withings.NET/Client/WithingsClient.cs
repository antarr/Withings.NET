using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevDefined.OAuth.Framework;
using Foundations.HttpClient.Enums;
using Material.Infrastructure.Credentials;
using Material.OAuth.Workflow;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET.Client
{
    public class WithingsClient
    {
        #region Constants

        const string RequestUrl = "https://oauth.withings.com/account/request_token";
        const string UserAuthorizeUrl = "https://oauth.withings.com/account/authorize";
        const string AccessUrl = "https://oauth.withings.com/account/access_token";
        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";

        #endregion

        readonly string ConsumerKey;
        readonly string ConsumerSecret;
        readonly string CallbackUrl;

        public WithingsClient(WithingsCredentials credentials)
        {
            ConsumerKey = credentials.ConsumerKey;
            ConsumerSecret = credentials.ConsumerSecret;
            CallbackUrl = credentials.CallbackUrl;
        }

        /// <summary>
        /// GET USER REQUEST URL
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> UserRequstUrl()
        {
            var uri = await GetAuthorizationUriAsync();
            return uri.AbsoluteUri;
        }

        /// <summary>
        /// GET USER ACCESS TOKEN
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="userId"></param>
        /// <returns>OAuth1Credentials</returns>
        public async Task<OAuth1Credentials> ExchangeRequestTokenForAccessToken(Uri requestUri,string userId)
        {
            OAuth1Web<Material.Infrastructure.ProtectedResources.Withings> app = WithingApp();
            return await app.GetAccessTokenAsync(requestUri, userId);
        }

        public async Task<string> UserDataAccessToken(string oauthtoken, string userId)
        {
            ////objective is to get the initial token following teh 3 steps
            ////Step 1 : get a oAuth "request token"
            string result = null;
            var nonce = Nonce();
            ////var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            var timestamp = DateTime.UtcNow.Epoch().ToString(); //// Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture));
            var nUri = new Uri(AccessUrl);
            var oauthSignature = GenerateSignature(nonce, timestamp, nUri, ConsumerKey);

            var uri = AccessUrl
                      + "?oauth_consumer_key=" + HttpUtility.UrlEncode(ConsumerKey)
                      + "&oauth_nonce=" + HttpUtility.UrlEncode(nonce)
                      + "&oauth_signature=" + HttpUtility.UrlEncode(oauthSignature)
                      + "&oauth_signature_method=HMAC-SHA1"
                      + $"&oauth_timestamp={HttpUtility.UrlEncode(timestamp)}"
                      + $"&oauth_token={oauthtoken}"
                      + "&oauth_version=1.0"
                      + $"&userid={userId}";

            var client = new HttpClient();
            await client.GetAsync(uri).ContinueWith(requestTask =>
            {
                var response = requestTask.Result;
                response.Content.ReadAsStringAsync().ContinueWith(readTask => result = readTask.Result);
            });
            return result;
        }

        #region Private Methods

        private OAuth1Web<Material.Infrastructure.ProtectedResources.Withings> WithingApp()
            => new OAuth1Web<Material.Infrastructure.ProtectedResources.Withings>(ConsumerKey, ConsumerSecret, CallbackUrl);

        private async Task<Uri> GetAuthorizationUriAsync() => await WithingApp().GetAuthorizationUriAsync("anyuser");

        private OAuth1Credentials Credentials()
        {
            var credentials = new OAuth1Credentials();
            credentials.SetConsumerProperties(ConsumerKey, ConsumerSecret);
            credentials.SetCallbackUrl(CallbackUrl);
            credentials.SetParameterHandling(HttpParameterType.Querystring);
            return credentials;
        }


        string GenerateSignature(string nonce, string timeStamp, Uri url, string clientId)
        {
            var signatureBase = GenerateBase(nonce, timeStamp, url);
            var signatureKey = $"{clientId}&{""}";
            ////var signatureKey = $"{clientId}&";
            var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            return Convert.ToBase64String(hmac.ComputeHash(new ASCIIEncoding().GetBytes(signatureBase)));
        }

        string GenerateBase(string nonce, string timeStamp, Uri url)
        {
            var parameters = new SortedDictionary<string, string>
            {
                {"oauth_consumer_key", ConsumerKey},
                {"oauth_signature_method", OauthSignatureMethod},
                {"oauth_timestamp", timeStamp},
                {"oauth_nonce", nonce},
                {"oauth_version", OauthVersion}
            };

            var sb = new StringBuilder();
            sb.Append("GET");
            sb.Append("&" + Uri.EscapeDataString(url.AbsoluteUri));
            sb.Append("&" + Uri.EscapeDataString(NormalizeParameters(parameters)));
            return sb.ToString();
        }

        static string NormalizeParameters(SortedDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                if (i > 0)
                    sb.Append("&");
                sb.AppendFormat("{0}={1}", parameter.Key, parameter.Value);
                i++;
            }
            return sb.ToString();
        }

        static string Nonce() => Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));

        #endregion
    }
}
