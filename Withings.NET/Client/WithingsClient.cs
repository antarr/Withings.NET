using System;
using System.Threading.Tasks;
using Material.Infrastructure.Credentials;
using Material.Infrastructure.Requests;
using Material.Infrastructure.Responses;
using Material.OAuth.Workflow;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;

namespace Withings.NET.Client
{
    public class WithingsClient
    {

        const string ResourceUrl = "https://wbsapi.withings.net/v2/";

        private readonly OAuth1Credentials _credentials;

        public WithingsClient(OAuth1Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<WithingsWeighInResponse> GetActivityMeasures(DateTime date)
        {
            var client = new RestClient(ResourceUrl)
            {
                Authenticator = OAuth1Authenticator.ForProtectedResource(_credentials.ConsumerKey, _credentials.ConsumerSecret, _credentials.OAuthToken, _credentials.OAuthSecret)
            };

            ((OAuth1Authenticator)client.Authenticator).ParameterHandling = OAuthParameterHandling.UrlOrPostParameters;

            var request = new WithingsWeighIn
            {
                Lastupdate = date
            };
            var response =
                await new AuthorizedRequester(_credentials)
                    .MakeOAuthRequestAsync<WithingsWeighIn, WithingsWeighInResponse>(request).ConfigureAwait(true);

            return response;
        }
    }
}