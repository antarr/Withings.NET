using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Withings.NET.Client;
using Withings.NET.Models;
using Withings.Specifications.Helpers;

namespace Withings.Specifications.E2E
{
    [SetUpFixture]
    [Category("E2E")]
    public class E2EFixture
    {
        public static string AccessToken { get; private set; }
        public static string UserId { get; private set; }
        public static WithingsCredentials Credentials { get; private set; }

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            EnvLoader.Load();

            var clientId = Environment.GetEnvironmentVariable("WITHINGS_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("WITHINGS_CLIENT_SECRET");
            var callbackUrl = Environment.GetEnvironmentVariable("WITHINGS_CALLBACK_URL");
            var refreshToken = Environment.GetEnvironmentVariable("WITHINGS_REFRESH_TOKEN");

            if (string.IsNullOrEmpty(refreshToken))
            {
                Assert.Ignore("WITHINGS_REFRESH_TOKEN not set. Run ./scripts/bootstrap-token.sh first.");
                return;
            }

            Credentials = new WithingsCredentials();
            Credentials.SetClientProperties(clientId, clientSecret);
            Credentials.SetCallbackUrl(callbackUrl);

            var authenticator = new Authenticator(Credentials);
            var token = await authenticator.RefreshAccessToken(refreshToken);

            AccessToken = token.AccessToken;
            UserId = token.UserId ?? Environment.GetEnvironmentVariable("WITHINGS_USER_ID");

            // Withings refresh tokens are single-use — save the new one to .env
            if (!string.IsNullOrEmpty(token.RefreshToken))
            {
                Environment.SetEnvironmentVariable("WITHINGS_REFRESH_TOKEN", token.RefreshToken);
                UpdateEnvFile("WITHINGS_REFRESH_TOKEN", token.RefreshToken);
            }
        }

        private static void UpdateEnvFile(string key, string value)
        {
            var dir = AppContext.BaseDirectory;
            while (dir != null)
            {
                if (Directory.GetFiles(dir, "*.sln").Length > 0)
                    break;
                dir = Directory.GetParent(dir)?.FullName;
            }
            if (dir == null) return;

            var envPath = Path.Combine(dir, ".env");
            if (!File.Exists(envPath)) return;

            var lines = File.ReadAllLines(envPath);
            var updated = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith($"{key}="))
                {
                    lines[i] = $"{key}={value}";
                    updated = true;
                    break;
                }
            }

            if (updated)
            {
                File.WriteAllLines(envPath, lines);
            }
        }
    }
}
