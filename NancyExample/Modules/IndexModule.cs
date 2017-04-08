using System;
using System.Configuration;
using AsyncOAuth;
using Foundations.HttpClient.Enums;
using Material.Infrastructure.Credentials;
using Nancy;
using Material.Infrastructure.Responses;

using Nancy.Helpers;
using Nancy.Responses;
using Newtonsoft.Json;
using Withings.NET.Client;

namespace NancyExample.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            var credentials = new OAuth1Credentials();
            credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);
            credentials.SetConsumerProperties(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                ConfigurationManager.AppSettings["WithingsConsumerSecret"]);
            credentials.SetParameterHandling(HttpParameterType.Querystring);

            var authenticator = new Authenticator(credentials);

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
                var accessToken = await authenticator.ExchangeRequestTokenForAccessToken(JsonConvert.DeserializeObject<RequestToken>(ConfigurationManager.AppSettings["RequestToken"]), verifier);
                ConfigurationManager.AppSettings["OAuthToken"] = accessToken.Key;
                ConfigurationManager.AppSettings["OAuthSecret"] = accessToken.Secret;
                return new JsonResponse(accessToken, new DefaultJsonSerializer());
            };

            Get["api/withings/activity"] = nothing =>
            {
                var client = new WithingsClient(credentials);
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
                var client = new WithingsClient(credentials);
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
                var client = new WithingsClient(credentials);
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
                var client = new WithingsClient(credentials);
                var activity = client.GetWorkouts
                (
                    DateTime.Parse("2017-01-03"),
                    DateTime.Parse("2017-03-30"),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/sleepmeasures"] = nothing =>
            {
                var client = new WithingsClient(credentials);
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
                var client = new WithingsClient(credentials);
                var activity = client.GetBodyMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    DateTime.Now.AddDays(-1),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<WithingsBody>(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/bodysince"] = nothing =>
            {
                var client = new WithingsClient(credentials);
                var activity = client.GetBodyMeasures
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<WithingsBody>(activity, new DefaultJsonSerializer());
            };

            Get["api/withings/intraday"] = nothing =>
            {
                var client = new WithingsClient(credentials);
                var activity = client.GetIntraDayActivity
                (
                    ConfigurationManager.AppSettings["UserId"],
                    DateTime.Now.AddDays(-90),
                    DateTime.Today, 
                    ConfigurationManager.AppSettings["OAuthToken"],
                    ConfigurationManager.AppSettings["OAuthSecret"]
                );
                return new JsonResponse<object>(activity, new DefaultJsonSerializer());
            };

            Get["api/oauth/requesttoken", true] = async (nothing, ct) =>
            {
                var token = await authenticator.GetRequestToken();
                return new JsonResponse<RequestToken>(token, new DefaultJsonSerializer());
            };
        }
    }
}