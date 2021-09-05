using System;
using System.IO;

namespace SharpBSABA2
{
    public abstract class ArchiveEntry
    {
        #region Properties

        public ulong BytesWritten { get; protected set; }

        /// <summary>
        /// Gets the index of the entry in the <see cref="SharpBSABA2.Archive"/>.
        /// </summary>
        public int Index { get; internal set; } = -1;

        public uint nameHash { get; protected set; }
        public uint dirHash { get; protected set; }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Gets the file name only including extension.
        /// </summary>
        public string FileName => Path.GetFileName(this.FullPath);

        /// <summary>
        /// Gets the folder.
        /// </summary>
        public string Folder => Path.GetDirectoryName(this.FullPath);

        /// <summary>
        /// Gets or sets the full file path.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Gets the full path in lower case.
        /// </summary>
        public string LowerPath => this.FullPath.ToLower();

        /// <summary>
        /// Gets the original unchanged full path.
        /// </summary>
        public string FullPathOriginal { get; internal set; }

        /// <summary>
        /// Gets if the file is compressed.
        /// </summary>
        public virtual bool Compressed { get; protected set; }

        public virtual ulong Offset { get; protected set; }

        /// <summary>
        /// Gets the uncompressed file size.
        /// </summary>
        public virtual uint RealSize { get; protected internal set; }

        /// <summary>
        /// Gets the file size.
        /// </summary>
        public virtual uint Size { get; protected set; }

        /// <summary>
        /// Gets a file size more suited for display in GUIs.
        /// </summary>
        public abstract uint DisplaySize { get; }

        /// <summary>
        /// Gets the <see cref="SharpBSABA2.Archive"/> containing this <see cref="ArchiveEntry"/>.
        /// </summary>
        public Archive Archive { get; private set; }

        #endregion

        protected ArchiveEntry(Archive archive)
        {
            this.Archive = archive;
        }

        public void Extract(bool preserveFolder) { Extract(string.Empty, preserveFolder); }
        public void Extract(string destination, bool preserveFolder) { Extract(destination, preserveFolder, FileName); }
        public void Extract(string destination, bool preserveFolder, string newName) { Extract(destination, preserveFolder, newName, Archive.BinaryReader); }
        public void Extract(string destination, bool preserveFolder, string newName, BinaryReader reader)
        {
            string path = preserveFolder ? this.Folder : string.Empty;

            path = Path.Combine(path, newName);

            if (!string.IsNullOrEmpty(destination))
                path = Path.Combine(destination, path);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var fs = File.Create(path))
                this.WriteDataToStream(fs, reader);

            if (this.Archive.MatchLastWriteTime)
                File.SetLastWriteTime(path, this.Archive.LastWriteTime);
        }

        public virtual MemoryStream GetDataStream()
        {
            return this.GetDataStream(this.Archive.BinaryReader);
        }

        /// <summary>
        /// Extracts and uncompresses data and then returns the stream.
        /// </summary>
        public virtual MemoryStream GetDataStream(BinaryReader reader)
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms, reader);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public MemoryStream GetRawDataStream()
        {
            return this.GetRawDataStream(this.Archive.BinaryReader);
        }

        public virtual MemoryStream GetRawDataStream(BinaryReader reader)
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms, reader, false);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public virtual string GetToolTipText()
        {
            return "Undefined";
        }

        protected abstract void WriteDataToStream(Stream stream, BinaryReader reader, bool decompress = true);
    }
}
