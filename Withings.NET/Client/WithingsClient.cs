using System;
using System.Configuration;
using System.Web;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using RestSharp;
using RestSharp.Authenticators;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET
{
	public class WithingsClient
	{
		static string apiRoot = "oauth.withings.com/account";
		static string consumerKey = Environment.GetEnvironmentVariable("WithingsKey"); // ConfigurationManager.AppSettings["WithingsKey"];
		static string consumerSecret = Environment.GetEnvironmentVariable("WithingsSecret");
		static string callbackUrl = Environment.GetEnvironmentVariable("WithingsCallbackUrl");
		string ResquestToken;

		internal string oauthToken;
		internal string oauthSecret;

		static RestClient _client;

		public WithingsClient()
		{
			_client = new RestClient(apiRoot);
		}

		public void GetRequestToken()
		{
			_client.Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, callbackUrl);
			var request = new RestRequest("access_token", Method.GET);
			IRestResponse response = _client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
				return;
			NameValueCollection query = HttpUtility.ParseQueryString(response.Content);
			oauthToken = query["oauth_token"];
			oauthSecret = query["oauth_token_secret"];
			Console.WriteLine($"{oauthToken} : {oauthSecret}");
		}
	}
}
