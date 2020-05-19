using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using lz4;
using SharpBSABA2.Enums;
using SharpBSABA2.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SharpBSABA2
{
    public abstract class Archive
    {
        public const int BufferSize = 4096 * 10;
        public const int DefaultProgressInterval = 1000;

        public bool RetrieveRealSize { get; set; }

        public string FullPath { get; private set; }
        public string FileName => Path.GetFileName(this.FullPath);

        public virtual int Chunks { get; set; }
        public virtual int FileCount { get; set; }
        public virtual bool HasNameTable { get; set; }
        public virtual string VersionString { get; set; } = "None";

        public virtual ArchiveTypes Type { get; protected set; }

        public Inflater Inflater { get; private set; } = new Inflater();
        public List<ArchiveEntry> Files { get; private set; } = new List<ArchiveEntry>();

        public BinaryReader BinaryReader { get; private set; }

        static Archive()
        {
            lz4.AnyCPU.loader.LZ4Loader.DisableVCRuntimeDetection = true;
        }

        public Archive(string filePath) : this(filePath, Encoding.UTF7) { }
        public Archive(string filePath, Encoding encoding) : this(filePath, encoding, false) { }

        public Archive(string filePath, Encoding encoding, bool retrieveRealSize)
        {
            this.FullPath = filePath;
            this.RetrieveRealSize = retrieveRealSize;
            this.BinaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read), encoding);

            this.Open(filePath);
        }

        /// <summary>
        /// Decompresses <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to decompress.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to decompress to.</param>
        public void Decompress(Stream input,
                               uint length,
                               Stream output,
                               Action<ulong> progressReport,
                               long progressInterval = DefaultProgressInterval)
        {
            var raw = input.ReadBytes((int)length);
            var sw = new Stopwatch();

            Inflater.Reset();
            Inflater.SetInput(raw, 0, raw.Length);
            sw.Start();

            int count;
            ulong written = 0;
            byte[] buffer = new byte[BufferSize];
            while ((count = Inflater.Inflate(buffer)) > 0)
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
        public void DecompressLZ4(Stream input,
                                  uint length,
                                  Stream output,
                                  Action<ulong> progressReport,
                                  long progressInterval = DefaultProgressInterval)
        {
            ulong written = 0;
            Stopwatch sw = new Stopwatch();

            byte[] data = new byte[length];
            input.Read(data, 0, data.Length);

            using (var ms = new MemoryStream(data, false))
            using (var lz4Stream = LZ4Stream.CreateDecompressor(ms, LZ4StreamMode.Read))
            {
                sw.Start();

                byte[] buffer = new byte[BufferSize];
                int count;
                while ((count = lz4Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, count);
                    written += (ulong)count;

                    if (sw.ElapsedMilliseconds >= progressInterval)
                    {
                        progressReport?.Invoke(written);
                        sw.Restart();
                    }
                }
            }

            progressReport?.Invoke(written);
            sw.Stop();
        }

        /// <summary>
        /// Writes section of <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to read from.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to write to.</param>
        public void WriteSectionToStream(Stream input,
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
        public void WriteSectionToStream(Stream input,
                                         ulong length,
                                         Stream output,
                                         Action<ulong> progressReport,
                                         long progressInterval = 1000)
        {
            ulong written = 0;
            Stopwatch sw = new Stopwatch();

            sw.Start();

            byte[] buffer = new byte[BufferSize];
            int count;
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

        public void Close()
        {
            this.BinaryReader?.Close();
        }

        public void Extract(string filePath, string destination, bool preserveFolder)
        {
            this.FindFile(filePath).Extract(destination, preserveFolder);
        }

        public ArchiveEntry FindFile(string fullpath)
        {
            return this.Files.Find(x => x.FullPath == fullpath);
        }

        public ArchiveEntry[] FindFiles(params string[] files)
        {
            return this.Files.FindAll(x => Array.IndexOf(files, x.FullPath) >= 0).ToArray();
        }

        protected abstract void Open(string filePath);
    }
}
