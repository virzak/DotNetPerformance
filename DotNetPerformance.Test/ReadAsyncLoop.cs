using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DotNetPerformance.Test
{
    public class ReadAsyncLoop
    {

        ITestOutputHelper Output { get; }

        public ReadAsyncLoop(ITestOutputHelper output)
        {
            Output = output;
        }

        static public string DefaultDocument => GetConfiguration()["Data:DefaultDocument"];

        public static IConfiguration GetConfiguration()
        {

            var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json")
    .AddJsonFile("usersettings.json");

            return builder.Build();
        }


        [Fact]
        public async Task CompareRunningTimes()
        {
            var sw = new Stopwatch();
            var asyncLoop = new DotNetPerformance.ReadAsyncLoop(DefaultDocument);
            //await asyncLoop.CompareRunningTimes().ConfigureAwait(false);

            sw.Restart();
            asyncLoop.ReadBytes();
            sw.Stop();
            Output.WriteLine($"FINISHED (Sync) in {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            await asyncLoop.ReadBytesAsync().ConfigureAwait(false);
            sw.Stop();
            Output.WriteLine($"FINISHED (Async) in {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            await asyncLoop.ReadBytesInParallelAsync().ConfigureAwait(false);
            sw.Stop();
            Output.WriteLine($"FINISHED (Async In Parallel) in {sw.ElapsedMilliseconds} ms");
        }
    }
}
