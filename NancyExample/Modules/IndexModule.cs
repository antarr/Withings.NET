using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
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
            _credentials.SetCallbackUrl(Environment.GetEnvironmentVariable("WithingsCallbackUrl"));
            _credentials.SetConsumerProperties(Environment.GetEnvironmentVariable("WithingsConsumerKey"),
                Environment.GetEnvironmentVariable("WithingsConsumerSecret"));

            var authenticator = new Authenticator(_credentials);

            Get("/", nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent));

            Get("api/oauth/authorize", async (nothing, ct) =>
            {
                var requestToken = await authenticator.GetRequestToken();
                ConfigurationManager.AppSettings["RequestToken"] = JsonConvert.SerializeObject(requestToken);
                return new RedirectResponse(authenticator.UserRequestUrl(requestToken));
            });

            Get("api/oauth/callback", async (parameters, ct) =>
            {
                var ps = HttpUtility.ParseQueryString(Request.Url.Query);
                ConfigurationManager.AppSettings["UserId"] = ps["userid"];
                var verifier = ps["oauth_verifier"];
                var credentials = await authenticator.ExchangeRequestTokenForAccessToken(JsonConvert.DeserializeObject<RequestToken>(ConfigurationManager.AppSettings["RequestToken"]), verifier);
                ConfigurationManager.AppSettings["OAuthToken"] = credentials.Key;
                ConfigurationManager.AppSettings["OAuthSecret"] = credentials.Secret;
                return Response.AsJson(credentials);
            });

            Get("api/withings/activity", async (nothing, ctx) =>
            {
              var client = new WithingsClient(_credentials);
              var activity = await client.GetActivityMeasures
                    (
                        DateTime.Parse("2017-01-01"),
                        DateTime.Parse("2017-03-30"),
                        ConfigurationManager.AppSettings["UserId"],
                        ConfigurationManager.AppSettings["OAuthToken"],
                        ConfigurationManager.AppSettings["OAuthSecret"]
                    );
              return Response.AsJson(activity);
            });

            Get("api/withings/dailyactivity", async (nothing,ctx) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetActivityMeasures
                (
                    DateTime.Today.AddDays(-30),
                    ConfigurationManager.AppSettings["UserId"],
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/withings/sleepsummary", async (nothing, ct) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetSleepSummary
                (
                    "2017-01-01",
                    "2017-03-30",
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/withings/workouts", async (nothing, ctx) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetWorkouts
                (
                    "2017-06-01",
                    "2017-06-05",
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/withings/sleepmeasures", async (nothing, ct) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetSleepMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    DateTime.Now.AddDays(-1),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/withings/body", async (nothing, ctx) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetBodyMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Parse("2017-05-08"),
                    DateTime.Parse("2017-05-10"),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/withings/bodysince", async (nothing,ctx) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetBodyMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Parse("2017-05-08"),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/withings/intraday", async (nothing, ctx) =>
            {
                var client = new WithingsClient(_credentials);
                var activity = await client.GetIntraDayActivity(
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    DateTime.Now.AddDays(-1),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return Response.AsJson(activity);
            });

            Get("api/oauth/requesttoken", async (nothing, ct) =>
            {
                var token = await authenticator.GetRequestToken();
                return Response.AsJson(token);
            });
        }
    }
}
