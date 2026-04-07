using SharpBSABA2.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SharpBSABA2.Utils
{
    public static class CompressionUtils
    {
        private const int BufferSize = 4096 * 10;
        private const int DefaultProgressInterval = 1000;

        /// <summary>
        /// Decompresses <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to decompress.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to decompress to.</param>
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
        public static void Decompress(Stream input,
                                      uint length,
                                      Stream output,
                                      Action<ulong> progressReport,
                                      SharedExtractParams extractParams,
                                      long progressInterval = DefaultProgressInterval)
        {
            int count;
            ulong written = 0;
            byte[] buffer = new byte[BufferSize];

            var raw = input.ReadBytes((int)length);
            extractParams.Inflater.Reset();
            extractParams.Inflater.SetInput(raw, 0, raw.Length);

            var sw = Stopwatch.StartNew();

            while ((count = extractParams.Inflater.Inflate(buffer)) > 0)
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

        /// <summary>
        /// Decompresses <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to decompress.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to decompress to.</param>
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
        public static void DecompressLZ4(Stream input,
                                         uint length,
                                         uint uncompressedLength,
                                         Stream output,
                                         Action<ulong> progressReport,
                                         long progressInterval = DefaultProgressInterval)
        {
            byte[] data = input.ReadBytes((int)length);
            byte[] decompressed = new byte[uncompressedLength];

            int written = K4os.Compression.LZ4.LZ4Codec.Decode(data, decompressed);

            output.Write(decompressed, 0, written);

            progressReport?.Invoke((ulong)written);
        }

        public static byte[] DecompressBattlespireLzss(byte[] input)
        {
            byte[] window = new byte[4096];

            for (int i = 0; i < 4078; i++)
                window[i] = 0x20;

            int windowPos = 4078;
            int pos = 0;
            var output = new List<byte>(input.Length * 2);

            while (pos < input.Length)
            {
                byte marker = input[pos++];

                for (int bit = 0; bit < 8; bit++)
                {
                    bool rawByte = ((marker >> bit) & 0x1) != 0;

                    if (rawByte)
                    {
                        if (pos >= input.Length)
                            return output.ToArray();

                        byte value = input[pos++];
                        window[windowPos] = value;
                        windowPos = (windowPos + 1) & 0x0FFF;
                        output.Add(value);
                    }
                    else
                    {
                        if (pos + 1 >= input.Length)
                            return output.ToArray();

                        byte b0 = input[pos++];
                        byte b1 = input[pos++];
                        int offset = b0 | ((b1 & 0xF0) << 4);
                        int length = (b1 & 0x0F) + 3;

                        for (int i = 0; i < length; i++)
                        {
                            byte value = window[(offset + i) & 0x0FFF];
                            window[windowPos] = value;
                            windowPos = (windowPos + 1) & 0x0FFF;
                            output.Add(value);
                        }
                    }
                }
            }

            return output.ToArray();
        }
    }
}
