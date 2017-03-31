using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Material.Infrastructure.Credentials;
using Material.OAuth.Workflow;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]
namespace Withings.NET.Client
{
    public class Authenticator
    {
        readonly string ConsumerKey;
        readonly string ConsumerSecret;
        readonly string CallbackUrl;

        public Authenticator(OAuth1Credentials credentials)
        {
            ConsumerKey = credentials.ConsumerKey;
            ConsumerSecret = credentials.ConsumerSecret;
            CallbackUrl = credentials.CallbackUrl;
        }

        /// <summary>
        /// GET USER REQUEST URL
        /// </summary>
        /// <returns>string</returns>
        public async Task<string> UserRequstUrl(string username)
        {
            var uri = await GetAuthorizationUriAsync(username).ConfigureAwait(false);
            return uri.AbsoluteUri;
        }

        /// <summary>
        /// GET USER ACCESS TOKEN
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="userId"></param>
        /// <returns>OAuth1Credentials</returns>
        public async Task<OAuth1Credentials> ExchangeRequestTokenForAccessToken(Uri requestUri, string userId)
        {
            OAuth1Web<Material.Infrastructure.ProtectedResources.Withings> app = WithingApp();
            return await app.GetAccessTokenAsync(requestUri,userId).ConfigureAwait(false);
        }


        #region Private Methods

        private OAuth1Web<Material.Infrastructure.ProtectedResources.Withings> WithingApp()
            => new OAuth1Web<Material.Infrastructure.ProtectedResources.Withings>(ConsumerKey, ConsumerSecret, CallbackUrl);

        private async Task<Uri> GetAuthorizationUriAsync(string username) => await WithingApp().GetAuthorizationUriAsync(username).ConfigureAwait(false);

        #endregion
    }
}
