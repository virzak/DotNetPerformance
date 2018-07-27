using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPerformance
{
    public class ReadAsyncLoop
    {

        public ReadAsyncLoop(string fileName)
        {
            FilePath = fileName;
            var fi = new FileInfo(FilePath);
            FileSize = fi.Length;
            SampleCount = FileSize / sampleSize;
        }

        const int sampleSize = 1000;
        const int sampleCount = 8000;

        long SampleCount { get; }
        string FilePath { get; }
        long FileSize { get; }


        public async Task CompareRunningTimes()
        {
            var sw = new Stopwatch();
            sw.Start();
            await ReadBytesAsync().ConfigureAwait(false);
            sw.Stop();
            Console.WriteLine($"FINISHED (Async) in {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            await ReadBytesInParallelAsync().ConfigureAwait(false);
            sw.Stop();
            Console.WriteLine($"FINISHED (Async in parallel) in {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            ReadBytes();
            sw.Stop();
            Console.WriteLine($"FINISHED (Sync) in {sw.ElapsedMilliseconds} ms");

        }

        public async Task ReadBytesInParallelAsync()
        {
            var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);

            var tasks = new List<Task>();
            for (var i = 0; i < sampleCount; ++i)
            {
                var sampleIndex = (long)(i / (double)SampleCount * FileSize);
                var bytes = new byte[sampleSize];
                tasks.Add(ReadSampleBytesFromFileAsync(fs, sampleIndex, bytes));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task ReadBytesAsync()
        {
            var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
            for (var i = 0; i < sampleCount; ++i)
            {
                var sampleIndex = (long)(i / (double)SampleCount * FileSize);
                var bytes = new byte[sampleSize];
                await ReadSampleBytesFromFileAsync(fs, sampleIndex, bytes).ConfigureAwait(false);
            }
        }

        public void ReadBytes()
        {
            var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096);
            for (var i = 0; i < sampleCount; ++i)
            {
                var sampleIndex = (long)(i / (double)SampleCount * FileSize);
                var bytes = new byte[sampleSize];
                ReadSampleBytesFromFile(fs, sampleIndex, bytes);
            }
        }

        static private async Task ReadSampleBytesFromFileAsync(FileStream fs, long sampleGlobalIndex, byte[] sampleBytes, CancellationToken ct = default)
        {
            var seekPosition = sampleGlobalIndex * sampleSize;
            var lActual = fs.Seek(seekPosition, SeekOrigin.Begin);

            await fs.ReadAsync(sampleBytes, 0, sampleSize, ct).ConfigureAwait(false);
        }

        static private void ReadSampleBytesFromFile(FileStream fs, long sampleGlobalIndex, byte[] sampleBytes)
        {
            var seekPosition = sampleGlobalIndex * sampleSize;
            var lActual = fs.Seek(seekPosition, SeekOrigin.Begin);

            fs.Read(sampleBytes, 0, sampleSize);
        }
    }
}
