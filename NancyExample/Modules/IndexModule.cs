using System;
using System.Configuration;
using Material.Infrastructure.Credentials;
using Nancy;
using Nancy.Responses;
using Withings.NET.Client;

namespace NancyExample.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            var Credentials = new OAuth1Credentials();
            Credentials.SetConsumerProperties(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                ConfigurationManager.AppSettings["WithingsConsumerSecret"]);
            Credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);

            var authenticator = new Authenticator(Credentials);

            Get["/"] = nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent);

            Get["api/oauth/authorize", true] = async (nothing, ct) => new RedirectResponse(await authenticator.UserRequstUrl("nancy_user").ConfigureAwait(true));

            Get["api/oauth/callback", true] = async (nothing, ct) =>
            {
                var credentials = await authenticator.ExchangeRequestTokenForAccessToken(Request.Url, "nancy_user").ConfigureAwait(true);
                return new JsonResponse<OAuth1Credentials>(credentials, new DefaultJsonSerializer());
            };

            Get["api/withings/activity"] = nothing =>
            {
                var activity = new WithingsClient(Credentials).GetActivityMeasures(DateTime.UtcNow);
                return new JsonResponse(activity, new DefaultJsonSerializer());
            };
        }
    }
}