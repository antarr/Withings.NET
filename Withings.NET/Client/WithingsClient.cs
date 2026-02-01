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

        public async Task<ExpandoObject> GetActivityMeasures(DateTime startDay, DateTime endDay, string userId, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("startdateymd", $"{startDay:yyyy-MM-dd}")
                .SetQueryParam("enddateymd", $"{endDay:yyyy-MM-dd}");

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetActivityMeasures(DateTime lastUpdate, string userId, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("date", $"{lastUpdate:yyyy-MM-dd}");

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        #endregion

        #region Get Sleep Measures/Summary

        public async Task<ExpandoObject> GetSleepSummary(string startday, string endday, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("sleep")
                .SetQueryParam("action", "getsummary")
                .SetQueryParam("startdateymd", startday)
                .SetQueryParam("enddateymd", endday);

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetSleepMeasures(string userid, DateTime startday, DateTime endday, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("sleep")
                .SetQueryParam("action", "get")
                .SetQueryParam("startdate", startday.ToUnixTime())
                .SetQueryParam("enddate", endday.ToUnixTime());

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        #endregion

        #region Get Workouts

        public async Task<ExpandoObject> GetWorkouts(string startday, string endday, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("measure").SetQueryParam("action", "getworkouts")
                .SetQueryParam("startdateymd", startday).SetQueryParam("enddateymd", endday);

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        #endregion

        #region Get Intraday Activity

        public async Task<ExpandoObject> GetIntraDayActivity(string userId, DateTime start, DateTime end, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getintradayactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("startdate", start.ToUnixTime())
                .SetQueryParam("enddate", end.ToUnixTime());

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        #endregion

        #region Get Body Measures

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime start, DateTime end, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getmeas")
                .SetQueryParam("userid", userid)
                .SetQueryParam("startdate", start.ToUnixTime())
                .SetQueryParam("enddate", end.ToUnixTime());

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime lastupdate, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getmeas")
                .SetQueryParam("userid", userid)
                .SetQueryParam("lastupdate", lastupdate.ToUnixTime());

            return await query
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
