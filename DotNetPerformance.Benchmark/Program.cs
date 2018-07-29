using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using System.IO;
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
            al = new ReadAsyncLoop(Program.DefaultDocument);
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

        static public string DefaultDocument => GetConfiguration()["Data:DefaultDocument"];

        public static IConfiguration GetConfiguration()
        {

            var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json")
    .AddJsonFile("usersettings.json");

            return builder.Build();
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LoadSamples>();
        }
    }
}
