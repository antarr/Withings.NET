using System;
using Material.Infrastructure.Credentials;

namespace Withings.NET.Client
{
	public class WithingsCredentials : OAuth1Credentials
    {

        public string OauthToken { get; set; }
        public string OauthTokenSecret { get; set; }
        public string UserId { get; set; }

  //      public WithingsCredentials(string consumerKey, string consumerSecret, string callbackUrl)
		//{
		//	if (string.IsNullOrEmpty(consumerKey))
		//		throw new ArgumentNullException(nameof(consumerKey));
		//	if (string.IsNullOrEmpty(consumerSecret))
		//		throw new ArgumentNullException(nameof(consumerSecret));
		//	if (string.IsNullOrEmpty(callbackUrl))
		//		throw new ArgumentNullException(nameof(callbackUrl));

		//	ConsumerKey = consumerKey;
		//	ConsumerSecret = consumerSecret;
		//	CallbackUrl = callbackUrl;
		//}
	}
}
