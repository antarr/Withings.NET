using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Withings.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = ManualConfig.Create(DefaultConfig.Instance)
                .AddJob(Job.Default.WithLaunchCount(1).WithWarmupCount(1).WithIterationCount(5));
            var summary = BenchmarkRunner.Run<CachingBenchmark>(config);
        }
    }
}
