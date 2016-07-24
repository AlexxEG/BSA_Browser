using System.IO;

namespace SharpBSABA2.BA2Util
{
    public class BA2FileEntry : ArchiveEntry
    {
        public uint flags { get; set; }
        public ulong offset { get; set; }
        public uint align { get; set; }

        /// <summary>
        /// Gets if the file is compressed.
        /// </summary>
        public bool Compressed
        {
            get { return RealSize != 0; }
        }
        /// <summary>
        /// Gets the file size. (packSz)
        /// </summary>
        public uint Size { get; private set; }
        /// <summary>
        /// Gets the uncompressed file size. (fullSz)
        /// </summary>
        public uint RealSize { get; private set; }

        public BA2FileEntry(Archive ba2, int index) : base(ba2, index)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();
            flags = ba2.BinaryReader.ReadUInt32();
            offset = ba2.BinaryReader.ReadUInt64();
            Size = ba2.BinaryReader.ReadUInt32();
            RealSize = ba2.BinaryReader.ReadUInt32();
            align = ba2.BinaryReader.ReadUInt32();
        }

        public override void Extract(string destination, bool preserveFolder)
        {
            BinaryReader.BaseStream.Seek((long)offset, SeekOrigin.Begin);

            string path = preserveFolder ? this.FullPath : this.FileName;

            if (!string.IsNullOrEmpty(destination))
                path = Path.Combine(destination, path);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var fs = File.Create(path))
            {
                byte[] bytes = new byte[this.Size];
                BinaryReader.Read(bytes, 0, (int)this.Size);

                if (!this.Compressed)
                {
                    fs.Write(bytes, 0, (int)this.Size);
                }
                else
                {
                    byte[] uncompressed = new byte[this.RealSize];

                    this.Archive.Inflater.Reset();
                    this.Archive.Inflater.SetInput(bytes);
                    this.Archive.Inflater.Inflate(uncompressed);

                    fs.Write(uncompressed, 0, uncompressed.Length);
                }
            }
        }
    }
}
