using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Antlr.Runtime;
using DevDefined.OAuth.Storage.Basic;
using Newtonsoft.Json;
using Withings.NET.Client;
using IToken = DevDefined.OAuth.Framework.IToken;

namespace Withings.NET.MVC2Example.Controllers
{
    ////[Authorize]
    public class OAuthController : ApiController
    {
        readonly WithingsClient _client;

        public OAuthController()
        {
            var credentials = new WithingsCredentials(ConfigurationManager.AppSettings["WithingsConsumerKey"],
                                                      ConfigurationManager.AppSettings["WithingsConsumerSecret"],
                                                      ConfigurationManager.AppSettings["WithingsCallbackUrl"]);

            _client = new WithingsClient(credentials);
        }

        [Route("api/oauth/authorize")]
        [HttpGet]
        public HttpResponseMessage Authorize()
        {
            var requestUrl = _client.UserRequstUrl();
            return Request.CreateResponse(HttpStatusCode.OK, requestUrl, Configuration.Formatters.JsonFormatter);
        }

        [Route("api/oauth/callback")]
        [HttpGet]
        public async Task<HttpResponseMessage> Callback()
        {
            var query = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var oauthToken = query["oauth_token"];
            var oauthVerifier = query["oauth_verifier"];
            var userid = query["userid"];
            var token = await _client.DoGetAccessToken(oauthToken, userid);
            return Request.CreateResponse(HttpStatusCode.OK, token, Configuration.Formatters.JsonFormatter);
        }

        [Route("api/oauth/token")]
        [HttpGet]
        public HttpResponseMessage Token(string token, string verifier, string userid)
        {
            var accessToken = new AccessToken
            {
                Token = token,
                UserName = userid,
                ConsumerKey = ConfigurationManager.AppSettings["WithingsConsumerKey"],
                TokenSecret = ConfigurationManager.AppSettings["WithingsConsumerSecret"]
            };


            var result = _client.GenUserToken(accessToken, verifier, userid);
            return Request.CreateResponse(HttpStatusCode.OK, result, Configuration.Formatters.JsonFormatter);
        }
    }
}
