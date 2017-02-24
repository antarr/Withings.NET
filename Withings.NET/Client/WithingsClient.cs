using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]

namespace Withings.NET.Client
{
    public class WithingsClient
    {
        #region Constants

        const string RequestUrl = "http://oauth.withings.com/account/request_token";
        const string UserAuthorizeUrl = "http://oauth.withings.com/account/authorize";
        const string AccessUrl = "http://oauth.withings.com/account/access_token";
        const string OauthSignatureMethod = "HMAC-SHA1";
        const string OauthVersion = "1.0";

        #endregion

        IOAuthSession _session;
        IToken _requestToken;

        readonly string _consumerKey;
        readonly string _consumerSecret;

        static string _callbackUrl;

        string OauthToken;
        string OauthSecret;
        string UserId;

        public WithingsClient(WithingsCredentials credentials)
        {
            _consumerKey = credentials.ConsumerKey;
            _consumerSecret = credentials.ConsumerSecret;
            _callbackUrl = credentials.CallbackUrl;
        }

        public string UserRequstUrl()
        {
            InitSession();
            _requestToken = _session.GetRequestToken();
            return DoGetAuthUrl(_requestToken);
        }

        public string AuthorizeUser()
        {
            return "";
        }

        public IToken GenUserToken(IToken requestToken, string verifier, string userId)
        {
            if (_session == null)
                InitSession();

            OauthToken = requestToken.Token;
            UserId = userId;
            OauthSecret = requestToken.TokenSecret;

            var accessToken = _session.ExchangeRequestTokenForAccessToken(requestToken, "GET", verifier);
            return accessToken;
        }

        public async Task<string> DoGetAccessToken(string token, string userId)
        {
            //objective is to get the initial token following teh 3 steps
            //Step 1 : get a oAuth "request token"
            string result = null;
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));

            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);

            var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            var nUri = new Uri(AccessUrl);
            var oauthSignature = GenerateSignature(nonce, timestamp, nUri, _consumerKey);

            var uri = AccessUrl 
                + "?oauth_consumer_key=" + HttpUtility.UrlEncode(_consumerKey)
                + "&oauth_nonce=" + HttpUtility.UrlEncode(nonce)
                + "&oauth_signature=" + HttpUtility.UrlEncode(oauthSignature)
                + "&oauth_signature_method=" + OauthSignatureMethod
                + "&oauth_timestamp=" + HttpUtility.UrlEncode(timestamp)
                + "&oauth_token=" + HttpUtility.UrlEncode(token)
                + "&oauth_version=" + OauthVersion
                + "&userid=" + HttpUtility.UrlEncode(userId);


            // Create an HttpClient instance 
            var client = new HttpClient();
            // Send a request asynchronously continue when complete 
            await client.GetAsync(uri).ContinueWith(
                requestTask =>
                {
                    // Get HTTP response from completed task. 
                    var response = requestTask.Result;

                    // Check that response was successful or throw exception 
                    // Read response asynchronously as JsonValue and write out top facts for each country 
                    response.Content.ReadAsStringAsync().ContinueWith(readTask => result = readTask.Result);
                });
            return result;
        }

        #region Private Methods

        OAuthConsumerContext CustomerContext()
        {
            return new OAuthConsumerContext
            {
                SignatureMethod = OauthSignatureMethod,
                ConsumerKey = _consumerKey,
                ConsumerSecret = _consumerSecret,
                UseHeaderForOAuthParameters = false
            };
        }

        string DoGetAuthUrl(IToken requestToken)
        {
            int counter;
            const int maxTries = 4;
            string authorizationLink = null;
            for (counter = 0; counter < maxTries; counter++)
            {
                authorizationLink = _session.GetUserAuthorizationUrlForToken(requestToken, _callbackUrl);
                if (string.IsNullOrEmpty(authorizationLink))
                    continue;
                break;
            }
            return authorizationLink;
        }

        void InitSession()
        {
            _session = new OAuthSession(
                    CustomerContext(),
                    RequestUrl,
                    UserAuthorizeUrl,
                    AccessUrl)
                {CallbackUri = new Uri(_callbackUrl)};
        }

        string GenerateSignature(string nonce, string timeStamp, Uri url, string clientId)
        {
            var signatureBase = GenerateBase(nonce, timeStamp, url);
            ////var signatureKey = $"{clientId}&{""}";
            var signatureKey = $"{clientId}&";
            var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            return Convert.ToBase64String(hmac.ComputeHash(new ASCIIEncoding().GetBytes(signatureBase)));
        }

        private string GenerateBase(string nonce, string timeStamp, Uri url)
        {
            var parameters = new SortedDictionary<string, string>
            {
                {"oauth_consumer_key", _consumerKey},
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
    }

    #endregion
}