using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using SharpBSABA2.Enums;

namespace SharpBSABA2
{
    public abstract class Archive
    {
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
