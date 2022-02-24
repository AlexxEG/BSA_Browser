using System;
using System.Diagnostics;
using System.IO;

namespace SharpBSABA2.Utils
{
    public static class StreamUtils
    {
        private const int BufferSize = 4096 * 10;
        private const int DefaultProgressInterval = 1000;

        /// <summary>
        /// Writes section of <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to read from.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to write to.</param>
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
        public static void WriteSectionToStream(Stream input,
                                                uint length,
                                                Stream output,
                                                Action<ulong> progressReport,
                                                long progressInterval = DefaultProgressInterval)
        {
            WriteSectionToStream(input, (ulong)length, output, progressReport, progressInterval);
        }

        /// <summary>
        /// Writes section of <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to read from.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to write to.</param>
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
        public static void WriteSectionToStream(Stream input,
                                                ulong length,
                                                Stream output,
                                                Action<ulong> progressReport,
                                                long progressInterval = DefaultProgressInterval)
        {
            int count;
            ulong written = 0;
            byte[] buffer = new byte[BufferSize];

            var sw = Stopwatch.StartNew();

            while ((count = input.Read(buffer, 0, (int)Math.Min(length - written, (ulong)buffer.Length))) > 0)
            {
                output.Write(buffer, 0, count);
                written += (ulong)count;

                if (sw.ElapsedMilliseconds >= progressInterval)
                {
                    progressReport?.Invoke(written);
                    sw.Restart();
                }
            }

            progressReport?.Invoke(written);
            sw.Stop();
        }
    }
}
