using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
        public HttpResponseMessage Callback()
        {
            return Request.CreateResponse(HttpStatusCode.OK, Request.RequestUri, Configuration.Formatters.JsonFormatter);
        }
    }
}
