using System.Configuration;
using System.Threading.Tasks;
using Nancy;
using Nancy.Helpers;
using Nancy.Responses;
using Withings.NET.Client;

namespace NancyExample.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            var credentials = new WithingsCredentials(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                                                      ConfigurationManager.AppSettings["WithingsConsumerSecret"],
                                                      ConfigurationManager.AppSettings["WithingsCallbackUrl"]);

            var client = new WithingsClient(credentials);

            Get["/"] = nothing => new RedirectResponse("api/oauth/authorize", RedirectResponse.RedirectType.Permanent);

            Get["api/oauth/authorize"] = nothing => new RedirectResponse(client.UserRequstUrl());

            Get["api/oauth/callback", true] = async (nothing, cancellationToken) =>
            {
                var oauthToken = Request.Query["oauth_token"].ToString();
                var oauthVerifier = Request.Query["oauth_verifier"].ToString();
                var userid = Request.Query["userid"].ToString();
                var token =  await client.DoGetAccessToken(oauthToken, userid);
                return token;
            };
        }
    }
}