using System;
using System.Configuration;
using AsyncOAuth;
using Nancy;
using Nancy.Helpers;
using Nancy.Responses;
using Newtonsoft.Json;
using Withings.NET.Client;
using Withings.NET.Models;

namespace NancyExample.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            var _credentials = new WithingsCredentials();
            _credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);
            _credentials.SetConsumerProperties(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                ConfigurationManager.AppSettings["WithingsConsumerSecret"]);

            var authenticator = new Authenticator(_credentials);

            Get["/"] = nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent);

            Get["api/oauth/authorize", true] = async (nothing, ct) =>
            {
                var requestToken = await authenticator.GetRequestToken();
                ConfigurationManager.AppSettings["RequestToken"] = JsonConvert.SerializeObject(requestToken);
                return new RedirectResponse(authenticator.UserRequestUrl(requestToken));
            };

            Get["api/oauth/callback", true] = async (parameters, ct) =>
            {
                var ps = HttpUtility.ParseQueryString(Request.Url.Query);
                ConfigurationManager.AppSettings["UserId"] = ps["userid"];
                var verifier = ps["oauth_verifier"];
                var credentials = await authenticator.ExchangeRequestTokenForAccessToken(JsonConvert.DeserializeObject<RequestToken>(ConfigurationManager.AppSettings["RequestToken"]), verifier);
                ConfigurationManager.AppSettings["OAuthToken"] = credentials.Key;
                ConfigurationManager.AppSettings["OAuthSecret"] = credentials.Secret;
                return new JsonResponse(credentials, new DefaultJsonSerializer());
            };

            Get["api/withings/activity"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetActivityMeasures
                (
                    DateTime.Parse("2017-01-01"),
                    DateTime.Parse("2017-03-30"),
                    ConfigurationManager.AppSettings["UserId"],
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<WithingsWeighInResponse>(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/dailyactivity"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetActivityMeasures
                (
                    DateTime.Today.AddDays(-30),
                    ConfigurationManager.AppSettings["UserId"],
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<WithingsWeighInResponse>(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/sleepsummary"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetSleepSummary
                (
                    "2017-01-01",
                    "2017-03-30",
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/workouts"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetWorkouts
                (
                    "2017-01-01",
                    "2017-03-30",
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse(JsonConvert.SerializeObject(activity), new DefaultJsonSerializer());
            };

            Get["api/withings/sleepmeasures"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetSleepMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    DateTime.Now.AddDays(-1),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/body"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetBodyMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Parse("2017-05-08"),
                    DateTime.Parse("2017-05-10"),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<object>(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/bodysince"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetBodyMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Parse("2017-05-08"),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<WithingsWeighInResponse>(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/intraday"] = nothing =>
            {
                var client = new WithingsClient(_credentials);
                var activity = client.GetIntraDayActivity(
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    DateTime.Now.AddDays(-1),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse(activity, new DefaultJsonSerializer());
            };

            Get["api/oauth/requesttoken", true] = async (nothing, ct) =>
            {
                var token = await authenticator.GetRequestToken();
                return new JsonResponse<RequestToken>(token, new DefaultJsonSerializer());
            };
        }
    }
}