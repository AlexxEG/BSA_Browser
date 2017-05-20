using System;
using System.IO;

namespace SharpBSABA2.BA2Util
{
    public class BA2FileEntry : ArchiveEntry
    {
        public uint flags { get; set; }
        public uint align { get; set; }

        public override bool Compressed
        {
            get { return RealSize != 0; }
        }
        public override uint DisplaySize
        {
            get
            {
                return this.Compressed ? this.RealSize : this.Size;
            }
        }

        public BA2FileEntry(Archive ba2, int index) : base(ba2, index)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();
            flags = ba2.BinaryReader.ReadUInt32();
            Offset = ba2.BinaryReader.ReadUInt64();
            Size = ba2.BinaryReader.ReadUInt32();
            RealSize = ba2.BinaryReader.ReadUInt32();
            align = ba2.BinaryReader.ReadUInt32();
        }
        
        protected override void WriteDataToStream(Stream stream)
        {
            BinaryReader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);

            byte[] bytes = new byte[this.Size];

            if (this.Size == 0)
            {
                bytes = new byte[this.RealSize];
                BinaryReader.Read(bytes, 0, (int)this.RealSize);
                stream.Write(bytes, 0, (int)this.RealSize);
                stream.Seek(0, SeekOrigin.Begin);
                return;
            }

            BinaryReader.Read(bytes, 0, (int)this.Size);

            if (!this.Compressed)
            {
                stream.Write(bytes, 0, (int)this.Size);
            }
            else
            {
                byte[] uncompressed = new byte[this.RealSize];

                this.Archive.Inflater.Reset();
                this.Archive.Inflater.SetInput(bytes);
                this.Archive.Inflater.Inflate(uncompressed);

                stream.Write(uncompressed, 0, uncompressed.Length);
            }
        }
    }
}
