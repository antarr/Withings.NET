using System;
using System.Runtime.CompilerServices;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET
{
	public class WithingsClient
	{
		#region Constants

		const string RequestUrl = "http://oauth.withings.com/account/request_token";
		const string UserAuthorizeUrl = "http://oauth.withings.com/account/authorize";
		const string AccessUrl = "http://oauth.withings.com/account/access_token";
		const string OauthSignatureMethod = "HMAC-SHA1";
		const string OauthVersion = "1.0";

		#endregion

		IOAuthSession _session;
		IToken _requestToken;

		readonly string _consumerKey;
		readonly string _consumerSecret;

		static string _callbackUrl;

		string OauthToken;
		string OauthSecret;
		string UserId;

		public WithingsClient(WithingsCredentials credentials)
		{
			_consumerKey = credentials.ConsumerKey;
			_consumerSecret = credentials.ConsumerSecret;
			_callbackUrl = credentials.CallbackUrl;
		}

		public string UserRequstUrl()
		{
			InitSession();
			_requestToken = _session.GetRequestToken();
			return DoGetAuthUrl(_requestToken);
		}

		public string AuthorizeUser()
		{
			return "";
		}

		#region Private Methods

		OAuthConsumerContext CustomerContext()
		{
			return new OAuthConsumerContext
			{
				SignatureMethod = OauthSignatureMethod,
				ConsumerKey = _consumerKey,
				ConsumerSecret = _consumerSecret,
				UseHeaderForOAuthParameters = false,
			};
		}

		string DoGetAuthUrl(IToken requestToken)
		{
			int counter;
			const int maxTries = 4;
			string authorizationLink = null;
			for (counter = 0; counter < maxTries; counter++)
			{
				authorizationLink = _session.GetUserAuthorizationUrlForToken(requestToken, _callbackUrl);
				if (string.IsNullOrEmpty(authorizationLink))
					continue;
				break;
			}
			return authorizationLink;
		}

		void InitSession()
		{
			_session = new OAuthSession(
				CustomerContext(),
				RequestUrl,
				UserAuthorizeUrl,
				AccessUrl)
			{ CallbackUri = new Uri(_callbackUrl) };

		}
	}

	#endregion
}
