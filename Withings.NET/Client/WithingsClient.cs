using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<ExpandoObject>(stream, _jsonOptions).ConfigureAwait(false);
        }

        private static string BuildUrl(string path, params (string key, string value)[] queryParams)
        {
            var query = new StringBuilder();
            foreach (var (key, value) in queryParams)
            {
                if (query.Length > 0)
                {
                    query.Append('&');
                }

                query
                    .Append(Uri.EscapeDataString(key))
                    .Append('=')
                    .Append(Uri.EscapeDataString(value ?? string.Empty));
            }

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

        #region Heart

        public async Task<ExpandoObject> GetHeartList(DateTime startDate, DateTime endDate, string accessToken)
        {
            var url = BuildUrl("heart",
                ("action", "list"),
                ("startdate", startDate.ToUnixTime().ToString()),
                ("enddate", endDate.ToUnixTime().ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetHeartRecording(string signalId, string accessToken)
        {
            var url = BuildUrl("heart",
                ("action", "get"),
                ("signalid", signalId));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion

        #region User

        public async Task<ExpandoObject> GetDevices(string accessToken)
        {
            var url = BuildUrl("user",
                ("action", "getdevice"));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetGoals(string accessToken)
        {
            var url = BuildUrl("user",
                ("action", "getgoals"));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion

        #region Nudge (Webhook Subscriptions)

        public async Task<ExpandoObject> Subscribe(string callbackUrl, int appli, string accessToken)
        {
            var url = BuildUrl("notify",
                ("action", "subscribe"),
                ("callbackurl", callbackUrl),
                ("appli", appli.ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> RevokeSubscription(string callbackUrl, int appli, string accessToken)
        {
            var url = BuildUrl("notify",
                ("action", "revoke"),
                ("callbackurl", callbackUrl),
                ("appli", appli.ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> GetSubscription(string callbackUrl, int appli, string accessToken)
        {
            var url = BuildUrl("notify",
                ("action", "get"),
                ("callbackurl", callbackUrl),
                ("appli", appli.ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        public async Task<ExpandoObject> ListSubscriptions(int appli, string accessToken)
        {
            var url = BuildUrl("notify",
                ("action", "list"),
                ("appli", appli.ToString()));

            return await GetAsync(url, accessToken).ConfigureAwait(false);
        }

        #endregion
    }
}
