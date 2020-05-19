using System.IO;

namespace SharpBSABA2.BA2Util
{
    public class BA2FileEntry : ArchiveEntry
    {
        public uint flags { get; set; }
        public uint align { get; set; }

        public override bool Compressed
        {
            get { return this.Size != 0; }
        }
        public override uint DisplaySize
        {
            get
            {
                return this.RealSize;
            }
        }

        public BA2FileEntry(Archive ba2) : base(ba2)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            FullPath = nameHash.ToString("X");
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();
            flags = ba2.BinaryReader.ReadUInt32();
            Offset = ba2.BinaryReader.ReadUInt64();
            Size = ba2.BinaryReader.ReadUInt32();
            RealSize = ba2.BinaryReader.ReadUInt32();
            align = ba2.BinaryReader.ReadUInt32();
        }

        public override MemoryStream GetRawDataStream()
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms, false);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public override string GetToolTipText()
        {
            return $"{nameof(nameHash)}: {nameHash}\n" +
                $"{nameof(FullPath)}: {FullPath}\n" +
                $"{nameof(Extension)}: {Extension.TrimEnd('\0')}\n" +
                $"{nameof(dirHash)}: {dirHash}\n" +
                $"{nameof(flags)}: {flags}\n" +
                $"{nameof(Offset)}: {Offset}\n" +
                $"{nameof(Size)}: {Size}\n" +
                $"{nameof(RealSize)}: {RealSize}\n" +
                $"{nameof(align)}: {align}";
        }

        protected override void WriteDataToStream(Stream stream)
        {
            this.WriteDataToStream(stream, true);
        }

        protected void WriteDataToStream(Stream stream, bool decompress)
        {
            BinaryReader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);

            uint len = this.Compressed ? this.Size : this.RealSize;
            byte[] bytes = new byte[len];

            BinaryReader.Read(bytes, 0, bytes.Length);

            if (!decompress || !this.Compressed)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                byte[] uncompressed = new byte[this.RealSize];

                this.Archive.Decompress(bytes, uncompressed);

                stream.Write(uncompressed, 0, uncompressed.Length);
            }
        }
    }
}
