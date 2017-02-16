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
		const string api_root = "https://oauth.withings.com/account";

		static string consumerKey;
		static string consumerSecret;
		static string callbackUrl;

		string OauthToken;
		string OauthSecret;

		RestClient client;

		public WithingsClient(WithingsCredentials credentials)
		{
			client = new RestClient(api_root);

			consumerKey = credentials.ConsumerKey;
			consumerSecret = credentials.ConsumerSecret;
			callbackUrl = credentials.CallbackUrl;
		}

		public string UserRequstUrl()
		{
			string requestUrl = null;
			client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, callbackUrl);
			((OAuth1Authenticator)client.Authenticator).ParameterHandling = OAuthParameterHandling.UrlOrPostParameters;

			var request = new RestRequest("/request_token", Method.GET);
			var response = client.Execute(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var query = HttpUtility.ParseQueryString(response.Content);
				OauthToken = query["oauth_token"];
				OauthSecret = query["oauth_token_secret"];

				request.AddParameter("oauth_token", OauthToken);
				requestUrl = client.BuildUri(request).ToString();
			}
			request = new RestRequest(api_root);
			request.AddParameter("oauth_token", OauthToken);
			requestUrl = client.BuildUri(request).ToString();
			return requestUrl;
		}

		public string AuthorizeUser()
		{
			var request = new RestRequest();
			client.Authenticator = OAuth1Authenticator.ForClientAuthentication(consumerKey, consumerSecret, "", "");
			var response = client.Execute(request);
			if (response.StatusCode != HttpStatusCode.OK)
				return null;
			var query = HttpUtility.ParseQueryString(response.Content);
			return JsonConvert.SerializeObject(query);
		}
	}
}
