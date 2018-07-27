using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace DotNetPerformance.Benchmark
{
    [KeepBenchmarkFiles]
    [Config(typeof(Config))]
    public class LoadSamples
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(Job.Clr.WithWarmupCount(0).WithIterationCount(10).WithInvocationCount(1).WithUnrollFactor(1).WithId("ClrQuick"));
                Add(Job.Core.WithWarmupCount(0).WithIterationCount(10).WithInvocationCount(1).WithUnrollFactor(1).WithId("CoreQuick"));
            }
        }

        ReadAsyncLoop al;

        [GlobalSetup]
        public void Setup()
        {
            al = new ReadAsyncLoop(@"C:\Users\virzak\Videos\OAM.mp4");
        }

        [Benchmark]
        public async Task ReadSamplesAsync()
        {
            await al.ReadBytesAsync().ConfigureAwait(false);
        }

        [Benchmark]
        public async Task ReadSamplesInParallelAsync()
        {
            await al.ReadBytesInParallelAsync().ConfigureAwait(false);
        }

        [Benchmark]
        public void ReadSamples()
        {
            al.ReadBytes();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LoadSamples>();
        }
    }
}
