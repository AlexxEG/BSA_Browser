using System.IO;

namespace SharpBSABA2
{
    public abstract class ArchiveEntry
    {
        public uint nameHash { get; protected set; }
        public uint dirHash { get; protected set; }

        /// <summary>
        /// Gets the index of this file in the BA2 archive.
        /// </summary>
        public int Index { get; private set; }

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

        protected ArchiveEntry(Archive archive, int index)
        {
            this.Archive = archive;
            this.Index = index;
        }

        public virtual void Extract(bool preserveFolder)
        {
            this.Extract(string.Empty, preserveFolder);
        }

        public abstract void Extract(string destination, bool preserveFolder);

        public abstract void Extract(string destination, bool preserveFolder, string newName);

        /// <summary>
        /// Extracts and uncompresses data and then returns the stream.
        /// </summary>
        public abstract MemoryStream GetDataStream();
    }
}
