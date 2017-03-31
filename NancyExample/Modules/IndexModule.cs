using System.Configuration;
using Foundations.HttpClient.Enums;
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
            var _credentials = new OAuth1Credentials();
            _credentials.SetCallbackUrl(ConfigurationManager.AppSettings["WithingsCallbackUrl"]);
            _credentials.SetConsumerProperties(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                ConfigurationManager.AppSettings["WithingsConsumerSecret"]);
            _credentials.SetParameterHandling(HttpParameterType.Querystring);

            var authenticator = new Authenticator(_credentials);

            Get["/"] = nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent);

            Get["api/oauth/authorize", true] = async (nothing, ct) => new RedirectResponse(await authenticator.UserRequstUrl("nancy_user").ConfigureAwait(true));

            Get["api/oauth/callback", true] = async (nothing, ct) =>
            {
                var credentials = await authenticator.ExchangeRequestTokenForAccessToken(Request.Url, "nancy_user").ConfigureAwait(true);
                return new JsonResponse<OAuth1Credentials>(credentials, new DefaultJsonSerializer());
            };
        }
    }
}