using System;
using System.Configuration;
using Foundations.HttpClient.Enums;
using Material.Infrastructure.Credentials;
using Material.Infrastructure.Responses;
using Nancy;
using Nancy.Responses;
using Withings.NET.Client;

namespace NancyExample.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            var _credentials = new OAuth1Credentials();
            _credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);
            _credentials.SetConsumerProperties(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                ConfigurationManager.AppSettings["WithingsConsumerSecret"]);
            _credentials.SetParameterHandling(HttpParameterType.Querystring);

            var authenticator = new Authenticator(_credentials);

            Get["/"] = nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent);

            Get["api/oauth/authorize", true] =
                async (nothing, ct) =>
                    new RedirectResponse(await authenticator.UserRequstUrl("nancy_user").ConfigureAwait(true));

            Get["api/oauth/callback", true] = async (nothing, ct) =>
            {
                var credentials =
                    await authenticator.ExchangeRequestTokenForAccessToken(Request.Url, "nancy_user")
                        .ConfigureAwait(true);

                ConfigurationManager.AppSettings["OAuthToken"] = credentials.OAuthToken;
                ConfigurationManager.AppSettings["OAuthSecret"] = credentials.OAuthSecret;
                ConfigurationManager.AppSettings["UserId"] = credentials.ExternalUserId;
                return new JsonResponse<OAuth1Credentials>(credentials, new DefaultJsonSerializer());
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
                return new JsonResponse<string>(activity, new DefaultJsonSerializer());
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
                return new JsonResponse<string>(activity, new DefaultJsonSerializer());
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
                return new JsonResponse<string>(activity, new DefaultJsonSerializer());
            };
        }
    }
}