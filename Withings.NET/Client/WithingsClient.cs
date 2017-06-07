using RestSharp;
using RestSharp.Extensions.MonoHttp;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Withings.NET.Models;

namespace Withings.NET.Client
{
    public class WithingsClient
    {
        internal RestClient Client;
        readonly WithingsCredentials _credentials;
        const string BaseUri = "https://wbsapi.withings.net/v2";

        public WithingsClient(WithingsCredentials credentials)
        {
            Client = new RestClient(BaseUri);
            _credentials = credentials;
        }

        #region Get Activity Measures    

        public WithingsWeighInResponse GetActivityMeasures(DateTime startDay, DateTime endDay, string userId, string token, string secret)
        {
            var oAuth = new OAuthBase();
            var request = new RestRequest("measure", Method.GET);
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"{BaseUri}/measure?action=getactivity&userid={userId}&startdateymd={startDay:yyyy-MM-dd}&enddateymd={endDay:yyyy-MM-dd}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);

            request
                .AddQueryParameter("action", "getactivity")
                .AddQueryParameter("userid", userId)
                .AddQueryParameter("startdateymd", startDay.ToString("yyyy-MM-dd"))
                .AddQueryParameter("enddateymd", startDay.ToString("yyyy-MM-dd"));
            AddOAuthParameters(request, nonce, timeStamp, signature, token);

            IRestResponse<WithingsWeighInResponse> response = Client.Execute<WithingsWeighInResponse>(request);
            return response.Data;
        }

        public WithingsWeighInResponse GetActivityMeasures(DateTime lastUpdate, string userId, string token, string secret)
        {
            var oAuth = new OAuthBase();
            var request = new RestRequest("measure", Method.GET);
            var uri = $"{BaseUri}/measure?action=getactivity&userid={userId}&date={lastUpdate:yyyy-MM-dd}";

            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
            request
                .AddQueryParameter("action", "getactivity")
                .AddQueryParameter("userid", userId)
                .AddQueryParameter("date", lastUpdate.ToString("yyyy-MM-dd"));
            AddOAuthParameters(request, nonce, timeStamp, signature, token);


            IRestResponse<WithingsWeighInResponse> response = Client.Execute<WithingsWeighInResponse>(request);
            return response.Data;
        }

        #endregion

        #region Get Sleep Measures/Summary

        public object GetSleepSummary(string startday, string endday, string token, string secret)
        {
            var oAuth = new OAuthBase();

            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"{BaseUri}/sleep?action=getsummary&startdateymd={startday}&enddateymd={endday}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);

            signature = HttpUtility.UrlEncode(signature);

            var requestUri = new StringBuilder(uri);
            requestUri.AppendFormat("&oauth_consumer_key={0}&", _credentials.ConsumerKey);
            requestUri.AppendFormat("oauth_nonce={0}&", nonce);
            requestUri.AppendFormat("oauth_signature={0}&", signature);
            requestUri.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            requestUri.AppendFormat("oauth_timestamp={0}&", timeStamp);
            requestUri.AppendFormat("oauth_token={0}&", token);
            requestUri.AppendFormat("oauth_version={0}", "1.0");

            var wrGeturl = WebRequest.Create(requestUri.ToString());
            var objStream = wrGeturl.GetResponse().GetResponseStream();
            const string sLine = "";

            if (objStream == null) return sLine;
            var objReader = new StreamReader(objStream);
            return JsonConvert.DeserializeObject(objReader.ReadLine());
        }

        public object GetSleepMeasures(string userid, DateTime startday, DateTime endday, string token, string secret)
        {
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"{BaseUri}/sleep?action=get&userid={userid}&startdate={startday.ToUnixTime()}&enddate={endday.ToUnixTime()}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);

            signature = HttpUtility.UrlEncode(signature);

            var requestUri = new StringBuilder(uri);
            requestUri.AppendFormat("&oauth_consumer_key={0}&", _credentials.ConsumerKey);
            requestUri.AppendFormat("oauth_nonce={0}&", nonce);
            requestUri.AppendFormat("oauth_signature={0}&", signature);
            requestUri.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            requestUri.AppendFormat("oauth_timestamp={0}&", timeStamp);
            requestUri.AppendFormat("oauth_token={0}&", token);
            requestUri.AppendFormat("oauth_version={0}", "1.0");

            var wrGeturl = WebRequest.Create(requestUri.ToString());
            var objStream = wrGeturl.GetResponse().GetResponseStream();
            const string sLine = "";

            if (objStream == null) return sLine;
            var objReader = new StreamReader(objStream);
            return JsonConvert.DeserializeObject(objReader.ReadLine());
        }

        #endregion

        #region Get Workouts

        public async Task<ExpandoObject> GetWorkouts(string startday, string endday, string token, string secret)
        {
            var query = BaseUri.AppendPathSegment("measure").SetQueryParam("action", "getworkouts")
              .SetQueryParam("startdateymd", startday).SetQueryParam("enddateymd", endday);
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
          string normalizedUrl;
          string parameters;
      var signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
            token, secret, "GET", timeStamp, nonce,
            
      OAuthBase.SignatureTypes.HMACSHA1, out  normalizedUrl, out parameters);
            query.SetQueryParam("oauth_consumer_key", _credentials.ConsumerKey);
            query.SetQueryParam("oauth_nonce", nonce);
            query.SetQueryParam("oauth_signature", signature);
            query.SetQueryParam("oauth_signature_method", "HMAC-SHA1");
            query.SetQueryParam("oauth_timestamp", timeStamp);
            query.SetQueryParam("oauth_token", token);
            query.SetQueryParam("oauth_version", "1.0");
      
            return await query.GetJsonAsync().ConfigureAwait(false);
        }

        #endregion

        #region Get Intraday Activity

        public object GetIntraDayActivity(string userId, DateTime start, DateTime end, string token, string secret)
        {
            var oAuth = new OAuthBase();
            var request = new RestRequest("measure", Method.GET);
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"{BaseUri}/measure?action=getintradayactivity&userid={userId}&startdate={start.ToUnixTime()}&enddate={end.ToUnixTime()}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);

            request
                .AddQueryParameter("action", "getintradayactivity")
                .AddQueryParameter("userid", userId)
                .AddQueryParameter("startdate", start.ToUnixTime().ToString())
                .AddQueryParameter("enddate", end.ToUnixTime().ToString());
            AddOAuthParameters(request, nonce, timeStamp, signature, token);

            var response = Client.Execute(request);
            return JsonConvert.SerializeObject(response.Content);
        }

        #endregion

        #region Get Body Measures

        public object GetBodyMeasures(string userid, DateTime start, DateTime end, string token, string secret)
        {
            var client = new RestClient("https://wbsapi.withings.net");
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"https://wbsapi.withings.net/measure?action=getmeas&userid={userid}&startdate={start.ToUnixTime()}&enddate={end.ToUnixTime()}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
            token, secret, "GET", timeStamp, nonce,
            OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
            var request = new RestRequest("measure", Method.GET);
            request
                .AddQueryParameter("action", "getmeas")
                .AddQueryParameter("userid", userid)
                .AddQueryParameter("startdate", start.ToUnixTime().ToString())
                .AddQueryParameter("enddate", end.ToUnixTime().ToString());
            AddOAuthParameters(request, nonce, timeStamp, signature, token);

            var response = client.Execute(request);
            return JsonConvert.DeserializeObject(response.Content);
        }

        public WithingsWeighInResponse GetBodyMeasures(string userid, DateTime lastupdate, string token, string secret)
        {
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            var uri = $"https://wbsapi.withings.net/measure?action=getmeas&userid={userid}&lastupdate={lastupdate.ToUnixTime()}";
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(uri), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
            var request = new RestRequest("measure", Method.GET);
            request
                .AddQueryParameter("action", "getmeas")
                .AddQueryParameter("userid", userid)
                .AddQueryParameter("lastupdate", lastupdate.ToUnixTime().ToString());
            AddOAuthParameters(request, nonce, timeStamp, signature, token);
            var client = new RestClient("https://wbsapi.withings.net");
            var response = client.Execute<WithingsWeighInResponse>(request);
            return response.Data;
        }

        #endregion

        void AddOAuthParameters(IRestRequest request, string nonce, string timeStamp, string signature, string token)
        {
            request
                .AddQueryParameter("oauth_consumer_key", _credentials.ConsumerKey)
                .AddQueryParameter("oauth_nonce", nonce)
                .AddQueryParameter("oauth_signature", signature)
                .AddQueryParameter("oauth_timestamp", timeStamp)
                .AddQueryParameter("oauth_token", token)
                .AddQueryParameter("oauth_signature_method", "HMAC-SHA1")
                .AddQueryParameter("oauth_version", "1.0");
        }
    }
}