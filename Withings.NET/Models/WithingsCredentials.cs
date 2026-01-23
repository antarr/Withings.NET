namespace Withings.NET.Models
{
    public class WithingsCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackUrl { get; set; }

        public void SetCallbackUrl(string url)
        {
            CallbackUrl = url;
        }

        public void SetClientProperties(string id, string secret)
        {
            ClientId = id;
            ClientSecret = secret;
        }
    }
}
