using Material.Infrastructure.Credentials;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Text;

namespace Withings.NET.Client
{
    public class WithingsClient
    {
        internal RestClient Client;
        readonly OAuth1Credentials _credentials;
        string baseUri = "http://wbsapi.withings.net/v2";

        public WithingsClient(OAuth1Credentials credentials)
        {
            Client = new RestClient(baseUri)
            {
                Authenticator = OAuth1Authenticator.ForProtectedResource
                (
                    credentials.ConsumerKey,
                    credentials.ConsumerSecret,
                    credentials.OAuthToken,
                    credentials.OAuthSecret
                )
            };
            ((OAuth1Authenticator)Client.Authenticator).ParameterHandling = OAuthParameterHandling.UrlOrPostParameters;
            _credentials = credentials;
        }

        public string GetActivityMeasures(string startDay, string endDay, string userId, string token, string secret)
        {
            var request = new RestRequest("measures");
            request.AddParameter("action", "getactivity");
            request.AddParameter("userid", userId);
            request.AddParameter("startdateymd", startDay);
            request.AddParameter("enddateymd", endDay);
            var oAuth = new OAuthBase();

            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"{baseUri}measure?action=getactivity&userid={userId}&startdateymd={startDay}&enddateymd={endDay}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature
                (
                    new Uri(uri),
                    _credentials.ConsumerKey,
                  _credentials.ConsumerSecret,
                   token,
                   secret,
                   "GET",
                   timeStamp,
                   nonce,
                    OAuthBase.SignatureTypes.HMACSHA1,
                    out normalizedUrl,
                    out parameters
                );

            signature = HttpUtility.UrlEncode(signature);

            request.AddParameter("oauth_consumer_key", _credentials.ConsumerKey);
            request.AddParameter("oauth_nonce", nonce);
            request.AddParameter("oauth_signature", signature);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_timestamp", timeStamp);
            request.AddParameter("oauth_token", token);
            request.AddParameter("oauth_version", "1.0");

            var response = Client.Execute(request);
            return response.Content;
            //var requestUri = new StringBuilder(uri);
            //requestUri.AppendFormat("&oauth_consumer_key={0}&", _credentials.ConsumerKey);
            //requestUri.AppendFormat("oauth_nonce={0}&", nonce);
            //requestUri.AppendFormat("oauth_signature={0}&", signature);
            //requestUri.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            //requestUri.AppendFormat("oauth_timestamp={0}&", timeStamp);
            //requestUri.AppendFormat("oauth_token={0}&", token);
            //requestUri.AppendFormat("oauth_version={0}", "1.0");

            //var wrGeturl = WebRequest.Create(requestUri.ToString());
            //var objStream = wrGeturl.GetResponse().GetResponseStream();
            //const string sLine = "";

            //if (objStream == null) return sLine;
            //var objReader = new StreamReader(objStream);
            //return objReader.ReadLine();
        }
    }
}