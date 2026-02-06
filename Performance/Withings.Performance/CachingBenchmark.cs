using BenchmarkDotNet.Attributes;
using Flurl.Http.Testing;
using Microsoft.Extensions.Caching.Memory;
using Withings.NET.Client;
using Withings.NET.Models;
using System;
using System.Threading.Tasks;

namespace Withings.Performance
{
    [MemoryDiagnoser]
    public class CachingBenchmark
    {
        private WithingsClient _client;
        private IMemoryCache _cache;
        private HttpTest _httpTest;
        private string _accessToken;
        private string _userId;
        private DateTime _start;
        private DateTime _end;

        [GlobalSetup]
        public void Setup()
        {
            var credentials = new WithingsCredentials();
            credentials.SetCallbackUrl("http://localhost");
            credentials.SetConsumerProperties("key", "secret");
            _client = new WithingsClient(credentials);
            _cache = new MemoryCache(new MemoryCacheOptions());
            _accessToken = "access_token";
            _userId = "123";
            _start = DateTime.Parse("2017-01-01");
            _end = DateTime.Parse("2017-03-30");

            _httpTest = new HttpTest();
            // Respond with some dummy JSON that matches ExpandoObject structure implicitly
            // The library expects standard Withings response format
            _httpTest.RespondWith("{\"status\": 0, \"body\": { \"activities\": [] }}");
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _httpTest.Dispose();
            _cache.Dispose();
        }

        [Benchmark(Baseline = true)]
        public async Task<object> NoCache()
        {
            return await _client.GetActivityMeasures(_start, _end, _userId, _accessToken);
        }

        [Benchmark]
        public async Task<object> WithCache()
        {
            var key = $"activity_{_userId}_{_start:yyyyMMdd}_{_end:yyyyMMdd}";
            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return await _client.GetActivityMeasures(_start, _end, _userId, _accessToken);
            });
        }
    }
}
