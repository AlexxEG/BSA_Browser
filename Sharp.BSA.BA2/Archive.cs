using ICSharpCode.SharpZipLib.Zip.Compression;
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

        public bool MatchLastWriteTime { get; set; }
        public bool RetrieveRealSize { get; protected set; }

        public long FileSize { get; protected set; }

        public string FullPath { get; protected set; }
        public string FileName => Path.GetFileName(this.FullPath);

        public DateTime LastWriteTime { get; protected set; }

        public virtual int Chunks { get; set; }
        public virtual int FileCount { get; set; }
        public virtual bool HasNameTable { get; set; }
        public virtual string VersionString { get; set; } = "None";

        public virtual ArchiveTypes Type { get; protected set; }

        public Encoding Encoding { get; protected set; }
        public Inflater Inflater { get; protected set; } = new Inflater();
        public List<ArchiveEntry> Files { get; protected set; } = new List<ArchiveEntry>();
        public BinaryReader BinaryReader { get; protected set; }

        static Archive()
        {
            lz4.AnyCPU.loader.LZ4Loader.DisableVCRuntimeDetection = true;
        }

        protected Archive(string filePath) : this(filePath, Encoding.UTF7) { }
        protected Archive(string filePath, Encoding encoding) : this(filePath, encoding, false) { }
        protected Archive(string filePath, Encoding encoding, bool retrieveRealSize)
        {
            this.FullPath = filePath;
            this.Encoding = encoding;
            this.LastWriteTime = File.GetLastWriteTime(this.FullPath);
            this.RetrieveRealSize = retrieveRealSize;
            this.BinaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read), encoding);
            this.FileSize = this.BinaryReader.BaseStream.Length;

            this.Open(filePath);
        }

        /// <summary>
        /// Decompresses <paramref name="input"/> to <paramref name="output"/> with progress reports.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> with the data to decompress.</param>
        /// <param name="length">The length of the data in the <paramref name="input"/>.</param>
        /// <param name="output">The <see cref="Stream"/> to decompress to.</param>
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
        public void Decompress(Stream input,
                               uint length,
                               Stream output,
                               Action<ulong> progressReport,
                               SharedExtractParams extractParams,
                               long progressInterval = DefaultProgressInterval)
        {
            var raw = input.ReadBytes((int)length);
            var sw = new Stopwatch();

            extractParams.Inflater.Reset();
            extractParams.Inflater.SetInput(raw, 0, raw.Length);
            sw.Start();

            int count;
            ulong written = 0;
            byte[] buffer = new byte[BufferSize];
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
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
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
        /// <param name="progressReport">Invokes at interval, based on <paramref name="progressInterval"/>, the amount of bytes written.</param>
        /// <param name="progressInterval">The interval at which to invoke <paramref name="progressReport"/>.</param>
        public void WriteSectionToStream(Stream input,
                                         ulong length,
                                         Stream output,
                                         Action<ulong> progressReport,
                                         long progressInterval = DefaultProgressInterval)
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

        /// <summary>
        /// Returns a new <see cref="Inflater"/> with default settings.
        /// </summary>
        public Inflater CloneInflater() => new Inflater();
        /// <summary>
        /// Returns a new <see cref="BinaryReader"/> for <see cref="Archive"/> with default settings.
        /// </summary>
        public BinaryReader CloneReader() => new BinaryReader(new FileStream(FullPath, FileMode.Open, FileAccess.Read), Encoding);

        /// <summary>
        /// Returns a <see cref="SharedExtractParams"/> with <see cref="BinaryReader"/> and <see cref="Inflater"/> originally used for multi threading.
        /// </summary>
        /// <param name="reader">True if a new <see cref="BinaryReader"/> should be created.</param>
        /// <param name="inflater">True if a new <see cref="Inflater"/> should be created.</param>
        public SharedExtractParams CreateSharedParams(bool reader, bool inflater) => new SharedExtractParams(this, reader, inflater);

        protected abstract void Open(string filePath);
    }
}
