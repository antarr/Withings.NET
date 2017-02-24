using System;

namespace Withings.NET.Client
{
	public class WithingsCredentials
	{
		internal string CallbackUrl;
		internal string ConsumerKey;
		internal string ConsumerSecret;

		public WithingsCredentials(string consumerKey, string consumerSecret, string callbackUrl)
		{
			if (string.IsNullOrEmpty(consumerKey))
				throw new ArgumentNullException(nameof(consumerKey));
			if (string.IsNullOrEmpty(consumerSecret))
				throw new ArgumentNullException(nameof(consumerSecret));
			if (string.IsNullOrEmpty(callbackUrl))
				throw new ArgumentNullException(nameof(callbackUrl));

			ConsumerKey = consumerKey;
			ConsumerSecret = consumerSecret;
			CallbackUrl = callbackUrl;
		}
	}
}
