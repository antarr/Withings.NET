namespace Withings.NET.Client
{
    public class OAuthToken
    {
        public OAuthToken(string key, string secret)
        {
            Key = key;
            Secret = secret;
        }

        public string Key { get; }
        public string Secret { get; }
    }
}
