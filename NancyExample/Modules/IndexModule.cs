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
            var withingsCredentials = new WithingsCredentials(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                                                      ConfigurationManager.AppSettings["WithingsConsumerSecret"],
                                                      ConfigurationManager.AppSettings["WithingsCallbackUrl"]);

            var authenticator = new Authenticator(withingsCredentials);

            Get["/"] = nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent);

            Get["api/oauth/authorize", true] = async (nothing, ct) => new RedirectResponse(await authenticator.UserRequstUrl("nancy_user"));

            Get["api/oauth/callback", true] = async (nothing, ct) =>
            {
                var credentials = await authenticator.ExchangeRequestTokenForAccessToken(Request.Url, "nancy_user");
                return new JsonResponse<OAuth1Credentials>(credentials, new DefaultJsonSerializer());
            };
        }
    }
}