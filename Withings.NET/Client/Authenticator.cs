using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AsyncOAuth;
using Material.Infrastructure.Credentials;
using Material.OAuth.Workflow;

[assembly: InternalsVisibleTo("Withings.Net.Specifications")]

namespace Withings.NET.Client
{
  public class Authenticator
  {
    readonly string _consumerKey;
    readonly string _consumerSecret;
    readonly string _callbackUrl;

    public Authenticator(OAuth1Credentials credentials)
    {
      _consumerKey = credentials.ConsumerKey;
      _consumerSecret = credentials.ConsumerSecret;
      _callbackUrl = credentials.CallbackUrl;

      OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };
    }

    public async Task<RequestToken> GetRequestToken(string callbackUrl)
    {
      var authorizer = new OAuthAuthorizer(_consumerKey, _consumerSecret);
      var parameters = new List<KeyValuePair<string, string>>
      {
        new KeyValuePair<string, string>("oauth_callback", Uri.EscapeUriString(callbackUrl))
      };
      TokenResponse<RequestToken> tokenResponse = await authorizer.GetRequestToken("https://oauth.withings.com/account/request_token", parameters);
      return tokenResponse.Token;
    }

    /// <summary>
    /// GET USER REQUEST URL
    /// </summary>
    /// <returns>string</returns>
    public string UserRequestUrl(RequestToken token)
    {
      var authorizer = new OAuthAuthorizer(_consumerKey, _consumerSecret);
      return authorizer.BuildAuthorizeUrl("https://oauth.withings.com/account/authorize", token);
    }

    /// <summary>
    /// GET USER ACCESS TOKEN
    /// </summary>
    /// <returns>OAuth1Credentials</returns>
    public async Task<AccessToken> ExchangeRequestTokenForAccessToken(RequestToken requestToken, string oAuthVerifier)
    {
      var authorizer = new OAuthAuthorizer(_consumerKey, _consumerSecret);
      TokenResponse<AccessToken> accessTokenResponse = await authorizer.GetAccessToken("https://oauth.withings.com/account/access_token", requestToken, oAuthVerifier);
      return accessTokenResponse.Token;
    }

    #region Private Methods

    private OAuth1Web<Material.Infrastructure.ProtectedResources.Withings> WithingApp()
      => new OAuth1Web<Material.Infrastructure.ProtectedResources.Withings>(_consumerKey, _consumerSecret,
        _callbackUrl);

    private async Task<Uri> GetAuthorizationUriAsync(string username) => await WithingApp()
      .GetAuthorizationUriAsync(username)
      .ConfigureAwait(false);

    #endregion
  }
}