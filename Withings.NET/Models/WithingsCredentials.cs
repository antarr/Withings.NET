namespace Withings.NET.Models
{
    public class WithingsCredentials
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
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
    }
}
