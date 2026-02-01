using System;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using Withings.NET.Client;

namespace TestJson
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("{\"access_token\": \"at123\", \"refresh_token\": \"rt123\", \"expires_in\": 3600, \"scope\": \"scope1\", \"token_type\": \"Bearer\", \"userid\": \"u123\"}");

                var response = await "http://example.com"
                    .GetJsonAsync<OAuthToken>();

                if (response.AccessToken == "at123" && response.RefreshToken == "rt123")
                {
                    Console.WriteLine("Deserialization SUCCESS");
                }
                else
                {
                    Console.WriteLine($"Deserialization FAILED. AccessToken: {response.AccessToken}");
                }
            }
        }
    }
}
