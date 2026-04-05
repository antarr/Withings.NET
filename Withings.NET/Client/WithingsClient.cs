using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Withings.NET.Models;

namespace Withings.NET.Client
{
    public class WithingsClient
    {
        const string BaseUri = "https://wbsapi.withings.net/v2";
        readonly HttpClient _httpClient;
        readonly JsonSerializerOptions _jsonOptions;

        public WithingsClient(WithingsCredentials credentials)
            : this(credentials, new HttpClient())
        {
        }

        internal WithingsClient(WithingsCredentials credentials, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new ExpandoObjectConverter());
        }

        private async Task<ExpandoObject> GetAsync(string url, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<ExpandoObject>(json, _jsonOptions);
        }

        private static string BuildUrl(string path, params (string key, string value)[] queryParams)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (var (key, value) in queryParams)
                query[key] = value;

            return $"{BaseUri}/{path}?{query}";
        }

        #region Get Activity Measures

        public async Task<ExpandoObject> GetActivityMeasures(DateTime startDay, DateTime endDay, string userId, string accessToken)
        {
            var url = BuildUrl("measure",
                ("action", "getactivity"),
                ("userid", userId),
                ("startdateymd", $"{startDay:yyyy-MM-dd}"),
                ("enddateymd", $"{endDay:yyyy-MM-dd}"));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetActivityMeasures(DateTime lastUpdate, string userId, string accessToken)
        {
            var url = BuildUrl("measure",
                ("action", "getactivity"),
                ("userid", userId),
                ("date", $"{lastUpdate:yyyy-MM-dd}"));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion

        #region Get Sleep Measures/Summary

        public async Task<ExpandoObject> GetSleepSummary(string startday, string endday, string accessToken)
        {
            var url = BuildUrl("sleep",
                ("action", "getsummary"),
                ("startdateymd", startday),
                ("enddateymd", endday));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetSleepMeasures(string userid, DateTime startday, DateTime endday, string accessToken)
        {
            var url = BuildUrl("sleep",
                ("action", "get"),
                ("startdate", startday.ToUnixTime().ToString()),
                ("enddate", endday.ToUnixTime().ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion

        #region Get Workouts

        public async Task<ExpandoObject> GetWorkouts(string startday, string endday, string accessToken)
        {
            var url = BuildUrl("measure",
                ("action", "getworkouts"),
                ("startdateymd", startday),
                ("enddateymd", endday));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion

        #region Get Intraday Activity

        public async Task<ExpandoObject> GetIntraDayActivity(string userId, DateTime start, DateTime end, string accessToken)
        {
            var url = BuildUrl("measure",
                ("action", "getintradayactivity"),
                ("userid", userId),
                ("startdate", start.ToUnixTime().ToString()),
                ("enddate", end.ToUnixTime().ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion

        #region Get Body Measures

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime start, DateTime end, string accessToken)
        {
            var url = BuildUrl("measure",
                ("action", "getmeas"),
                ("userid", userid),
                ("startdate", start.ToUnixTime().ToString()),
                ("enddate", end.ToUnixTime().ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetBodyMeasures(string userid, DateTime lastupdate, string accessToken)
        {
            var url = BuildUrl("measure",
                ("action", "getmeas"),
                ("userid", userid),
                ("lastupdate", lastupdate.ToUnixTime().ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion
    }
}
