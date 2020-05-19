using System;
using System.IO;

namespace SharpBSABA2
{
    public abstract class ArchiveEntry
    {
        public uint nameHash { get; protected set; }
        public uint dirHash { get; protected set; }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        public string Extension { get; protected set; }
        /// <summary>
        /// Gets the file name only including extension.
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.FullPath);
            }
        }
        /// <summary>
        /// Gets the folder.
        /// </summary>
        public string Folder
        {
            get
            {
                return Path.GetDirectoryName(this.FullPath);
            }
        }
        /// <summary>
        /// Gets or sets the full file path.
        /// </summary>
        public string FullPath { get; set; }
        public string LowerPath
        {
            get
            {
                return this.FullPath.ToLower();
            }
        }

        /// <summary>
        /// Gets if the file is compressed.
        /// </summary>
        public virtual bool Compressed { get; protected set; }
        public virtual ulong Offset { get; protected set; }
        /// <summary>
        /// Gets the uncompressed file size.
        /// </summary>
        public virtual uint RealSize { get; protected set; }
        /// <summary>
        /// Gets the file size.
        /// </summary>
        public virtual uint Size { get; protected set; }
        /// <summary>
        /// Gets a file size more suited for display in GUIs.
        /// </summary>
        public abstract uint DisplaySize { get; }

        public Archive Archive { get; private set; }

        public BinaryReader BinaryReader { get { return this.Archive.BinaryReader; } }

        protected ArchiveEntry(Archive archive)
        {
            this.Archive = archive;
        }

        public virtual void Extract(bool preserveFolder)
        {
            this.Extract(string.Empty, preserveFolder);
        }

        public virtual void Extract(string destination, bool preserveFolder)
        {
            this.Extract(destination, preserveFolder, this.FileName);
        }

        public virtual void Extract(string destination, bool preserveFolder, string newName)
        {
            string path = preserveFolder ? this.Folder : string.Empty;

            path = Path.Combine(path, newName);

            if (!string.IsNullOrEmpty(destination))
                path = Path.Combine(destination, path);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var fs = File.Create(path))
                this.WriteDataToStream(fs);
        }

        /// <summary>
        /// Extracts and uncompresses data and then returns the stream.
        /// </summary>
        public virtual MemoryStream GetDataStream()
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public virtual MemoryStream GetRawDataStream()
        {
            throw new NotImplementedException();
        }

        public virtual string GetToolTipText()
        {
            return "Undefined";
        }

        protected abstract void WriteDataToStream(Stream stream);
    }
}
