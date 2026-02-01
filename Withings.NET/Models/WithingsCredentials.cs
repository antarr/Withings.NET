namespace Withings.NET.Models
{
    public class WithingsCredentials
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public string ClientId
        {
            get => ConsumerKey;
            set => ConsumerKey = value;
        }

        public string ClientSecret
        {
            get => ConsumerSecret;
            set => ConsumerSecret = value;
        }

        public string CallbackUrl { get; set; }

        public void SetCallbackUrl(string url)
        {
            CallbackUrl = url;
        }

        public void SetConsumerProperties(string key, string secret)
        {
            ConsumerKey = key;
            ConsumerSecret = secret;
        }

        public void SetClientProperties(string id, string secret)
        {
            ClientId = id;
            ClientSecret = secret;
        }
    }
}
