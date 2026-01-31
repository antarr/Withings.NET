using System;
using System.Dynamic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Withings.NET.Client
{
    public class WithingsClient
    {
        const string BaseUri = "https://wbsapi.withings.net";

        public WithingsClient()
        {
        }

        #region Get Activity Measures

        public async Task<ExpandoObject> GetActivityMeasures(DateTime startDay, DateTime endDay, string userId, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("v2/measure")
                .SetQueryParam("action", "getactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("startdateymd", $"{startDay:yyyy-MM-dd}")
                .SetQueryParam("enddateymd", $"{endDay:yyyy-MM-dd}")
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetActivityMeasures(DateTime lastUpdate, string userId, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("v2/measure")
                .SetQueryParam("action", "getactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("date", $"{lastUpdate:yyyy-MM-dd}")
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        #endregion

        #region Get Sleep Measures/Summary

        public async Task<ExpandoObject> GetSleepSummary(string startday, string endday, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("v2/sleep")
                .SetQueryParam("action", "getsummary")
                .SetQueryParam("startdateymd", startday)
                .SetQueryParam("enddateymd", endday)
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetSleepMeasures(string userid, DateTime startday, DateTime endday, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("v2/sleep")
                .SetQueryParam("action", "get")
                .SetQueryParam("startdate", startday.ToUnixTime())
                .SetQueryParam("enddate", endday.ToUnixTime())
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        #endregion

        #region Get Workouts

        public async Task<ExpandoObject> GetWorkouts(string startday, string endday, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("v2/measure")
                .SetQueryParam("action", "getworkouts")
                .SetQueryParam("startdateymd", startday)
                .SetQueryParam("enddateymd", endday)
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        #endregion

        #region Get Intraday Activity

        public async Task<ExpandoObject> GetIntraDayActivity(string userId, DateTime start, DateTime end, string accessToken)
        {
            var query = BaseUri.AppendPathSegment("v2/measure")
                .SetQueryParam("action", "getintradayactivity")
                .SetQueryParam("userid", userId)
                .SetQueryParam("startdate", start.ToUnixTime())
                .SetQueryParam("enddate", end.ToUnixTime())
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        #endregion

        #region Get Body Measures

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime start, DateTime end, string accessToken)
        {
            // Original code used v1 for this endpoint
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getmeas")
                .SetQueryParam("userid", userid)
                .SetQueryParam("startdate", start.ToUnixTime())
                .SetQueryParam("enddate", end.ToUnixTime())
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime lastupdate, string accessToken)
        {
            // Original code used v1 for this endpoint
            var query = BaseUri.AppendPathSegment("measure")
                .SetQueryParam("action", "getmeas")
                .SetQueryParam("userid", userid)
                .SetQueryParam("lastupdate", lastupdate.ToUnixTime())
                .WithOAuthBearerToken(accessToken);

            return await query.GetJsonAsync<ExpandoObject>().ConfigureAwait(false);
        }

        #endregion
    }
}
