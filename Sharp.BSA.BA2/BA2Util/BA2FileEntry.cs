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

        public override void Extract(string destination, bool preserveFolder)
        {
            this.Extract(destination, preserveFolder, this.FileName);
        }

        public override void Extract(string destination, bool preserveFolder, string newName)
        {
            BinaryReader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);

            string path = preserveFolder ? this.Folder : string.Empty;

            path = Path.Combine(path, newName);

            if (!string.IsNullOrEmpty(destination))
                path = Path.Combine(destination, path);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var fs = File.Create(path))
            {
                byte[] bytes = new byte[this.Size];

                if (this.Size == 0)
                {
                    bytes = new byte[this.RealSize];
                    BinaryReader.Read(bytes, 0, (int)this.RealSize);
                    fs.Write(bytes, 0, (int)this.RealSize);
                    return;
                }

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

        public override MemoryStream GetDataStream()
        {
            var result_ms = new MemoryStream();

            BinaryReader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);

            byte[] bytes = new byte[this.Size];

            if (this.Size == 0)
            {
                bytes = new byte[this.RealSize];
                BinaryReader.Read(bytes, 0, (int)this.RealSize);
                result_ms.Write(bytes, 0, (int)this.RealSize);
                result_ms.Seek(0, SeekOrigin.Begin);
                return result_ms;
            }

            BinaryReader.Read(bytes, 0, (int)this.Size);

            if (!this.Compressed)
            {
                result_ms.Write(bytes, 0, (int)this.Size);
            }
            else
            {
                byte[] uncompressed = new byte[this.RealSize];

                this.Archive.Inflater.Reset();
                this.Archive.Inflater.SetInput(bytes);
                this.Archive.Inflater.Inflate(uncompressed);

                result_ms.Write(uncompressed, 0, uncompressed.Length);
            }

            result_ms.Seek(0, SeekOrigin.Begin);
            return result_ms;
        }
    }
}
