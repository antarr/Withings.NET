using System;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Withings.NET.Models;

namespace Withings.NET.Client
{
    public class WithingsClient
    {
        readonly WithingsCredentials _credentials;
        const string BaseUri = "https://wbsapi.withings.net/v2";

        public WithingsClient(WithingsCredentials credentials)
        {
            _credentials = credentials;
        }

        #region Get Activity Measures    

        public async Task<ExpandoObject> GetActivityMeasures(DateTime startDay, DateTime endDay, string userId, string token, string secret)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("startdateymd", $"{startDay:yyyy-MM-dd}")
                .SetQueryParam("enddateymd", $"{endDay:yyyy-MM-dd}");
            var oAuth = new OAuthBase();
            string nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            string signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
            query.SetQueryParam("oauth_consumer_key", _credentials.ConsumerKey);
            query.SetQueryParam("oauth_nonce", nonce);
            query.SetQueryParam("oauth_signature", signature);
            query.SetQueryParam("oauth_signature_method", "HMAC-SHA1");
            query.SetQueryParam("oauth_timestamp", timeStamp);
            query.SetQueryParam("oauth_token", token);
            query.SetQueryParam("oauth_version", "1.0");
            return await query.GetJsonAsync().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetActivityMeasures(DateTime lastUpdate, string userId, string token, string secret)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("date", $"{lastUpdate:yyyy-MM-dd}");
            var oAuth = new OAuthBase();
            string nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            string signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
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

        #region Get Sleep Measures/Summary

        public async Task<ExpandoObject> GetSleepSummary(string startday, string endday, string token, string secret)
        {
            var query = BaseUri.AppendPathSegment("sleep")
                .SetQueryParam("action", "getsummary")
                .SetQueryParam("startdateymd", startday)
                .SetQueryParam("enddateymd", endday);
            var oAuth = new OAuthBase();
            string nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            string signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
            query.SetQueryParam("oauth_consumer_key", _credentials.ConsumerKey);
            query.SetQueryParam("oauth_nonce", nonce);
            query.SetQueryParam("oauth_signature", signature);
            query.SetQueryParam("oauth_signature_method", "HMAC-SHA1");
            query.SetQueryParam("oauth_timestamp", timeStamp);
            query.SetQueryParam("oauth_token", token);
            query.SetQueryParam("oauth_version", "1.0");

            return await query.GetJsonAsync().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetSleepMeasures(string userid, DateTime startday, DateTime endday, string token, string secret)
        {
            var query = BaseUri.AppendPathSegment("sleep")
                .SetQueryParam("action", "get")
                .SetQueryParam("startdate", startday.ToUnixTime())
                .SetQueryParam("enddate", endday.ToUnixTime());
            var oAuth = new OAuthBase();
            string nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
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
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
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

        public async Task<ExpandoObject> GetIntraDayActivity(string userId, DateTime start, DateTime end, string token, string secret)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getintradayactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("startdate", start.ToUnixTime())
                .SetQueryParam("enddate", end.ToUnixTime());
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
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

        #region Get Body Measures

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime start, DateTime end, string token, string secret)
        {
            var query = "https://wbsapi.withings.net".AppendPathSegment("measure")
                .SetQueryParam("action", "getmeas")
                .SetQueryParam("userid", userid)
                .SetQueryParam("startdate", start.ToUnixTime())
                .SetQueryParam("enddate", end.ToUnixTime());
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
            query.SetQueryParam("oauth_consumer_key", _credentials.ConsumerKey);
            query.SetQueryParam("oauth_nonce", nonce);
            query.SetQueryParam("oauth_signature", signature);
            query.SetQueryParam("oauth_signature_method", "HMAC-SHA1");
            query.SetQueryParam("oauth_timestamp", timeStamp);
            query.SetQueryParam("oauth_token", token);
            query.SetQueryParam("oauth_version", "1.0");

            return await query.GetJsonAsync().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime lastupdate, string token, string secret)
        {
            var query = "https://wbsapi.withings.net".AppendPathSegment("measure")
                .SetQueryParam("action", "getmeas")
                .SetQueryParam("userid", userid)
                .SetQueryParam("lastupdate", lastupdate.ToUnixTime());
            var oAuth = new OAuthBase();
            var nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeStamp = oAuth.GenerateTimeStamp();
            string normalizedUrl;
            string parameters;
            var signature = oAuth.GenerateSignature(new Uri(query), _credentials.ConsumerKey, _credentials.ConsumerSecret,
                token, secret, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out normalizedUrl, out parameters);
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
    }
}