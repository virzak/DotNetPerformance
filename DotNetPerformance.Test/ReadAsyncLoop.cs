using System;
using System.Diagnostics;
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

        [Fact]
        public async Task CompareRunningTimes()
        {
            var sw = new Stopwatch();
            var asyncLoop = new DotNetPerformance.ReadAsyncLoop(@"C:\Users\virzak\Videos\OAM.mp4");
            //await lol.CompareRunningTimes().ConfigureAwait(false);

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
