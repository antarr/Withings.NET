using System;
using System.Configuration;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET.Client
{
	public class WithingsClient
	{
	    const string ApiRoot = "https://oauth.withings.com/account";
	    static string _consumerKey = ConfigurationManager.AppSettings.Get("WithingsKey");
		static string _consumerSecret = ConfigurationManager.AppSettings.Get("WithingsSecret");
		static string _callbackUrl = ConfigurationManager.AppSettings.Get("WithingsCallbackUrl");

		internal string OauthToken;
		internal string OauthSecret;

		RestClient _client;

	    public string GetUserRequstUrl()
	    {
            string requestUrl = null;
            try
            {
                if (_client == null)
                    _client = new RestClient(new Uri(ApiRoot));

                _client.Authenticator = OAuth1Authenticator.ForRequestToken(_consumerKey, _consumerSecret, _callbackUrl);
                ((OAuth1Authenticator)_client.Authenticator).ParameterHandling = OAuthParameterHandling.UrlOrPostParameters;

                var request = new RestRequest("request_token",Method.GET);
                var response = _client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var query = HttpUtility.ParseQueryString(response.Content);
                    OauthToken = query["oauth_token"];
                    OauthSecret = query["oauth_token_secret"];

                    request.AddParameter("oauth_token", OauthToken);
                    requestUrl = _client.BuildUri(request).ToString();
                }
            }
	        catch (Exception e)
	        {
	            Console.WriteLine("Exception: " + JsonConvert.SerializeObject(e));
	        }
	        return requestUrl;
	    }

        public string AuthorizeUser()
	    {
	        var request = new RestRequest();
	        _client.Authenticator = OAuth1Authenticator.ForClientAuthentication(_consumerKey, _consumerSecret, "", "");
	        var response = _client.Execute(request);
            if(response.StatusCode != HttpStatusCode.OK)
                return null;
	        var query = HttpUtility.ParseQueryString(response.Content);
	        return JsonConvert.SerializeObject(query);
	    }
	}
}
