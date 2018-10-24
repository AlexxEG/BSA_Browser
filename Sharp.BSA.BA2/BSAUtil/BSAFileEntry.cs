using System.IO;
using SharpBSABA2.Extensions;

namespace SharpBSABA2.BSAUtil
{
    public enum BSAFileVersion
    {
        BSA,
        Morrowind,
        Fallout2
    }

    public class BSAFileEntry : ArchiveEntry
    {
        public new BSA Archive => base.Archive as BSA;
        public override uint DisplaySize
        {
            get
            {
                return this.RealSize > 0 ? this.RealSize : this.Size;
            }
        }

        public BSAFileVersion Version { get; private set; }

        public BSAFileEntry(Archive archive, int index, bool compressed, string folder, uint offset, uint size)
            : base(archive, index)
        {
            this.Version = BSAFileVersion.BSA;

            this.Compressed = compressed;
            this.FullPath = folder;
            this.Offset = offset;
            this.Size = size;

            if (this.Archive.RetrieveRealSize)
            {
                this.RealSize = this.BinaryReader.ReadUInt32From((long)Offset);
            }
        }

        public BSAFileEntry(Archive archive, int index, string path, uint offset, uint size)
            : base(archive, index)
        {
            this.Version = BSAFileVersion.Morrowind;

            this.FullPath = path;
            this.Offset = offset;
            this.Size = size;
        }

        public BSAFileEntry(Archive archive, int index, string path, uint offset, uint size, uint realSize)
            : base(archive, index)
        {
            this.Version = BSAFileVersion.Fallout2;

            this.FullPath = path;
            this.Offset = offset;
            this.Size = size;
            this.RealSize = realSize;
            this.Compressed = realSize != 0;
        }

        public override MemoryStream GetRawDataStream()
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms, false);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        protected override void WriteDataToStream(Stream stream)
        {
            this.WriteDataToStream(stream, true);
        }

        protected void WriteDataToStream(Stream stream, bool decompress)
        {
            this.BinaryReader.BaseStream.Position = (long)Offset;

            if (this.Archive.Version == BSA.SSE_HEADER_VERSION)
            {
                // Separate Skyrim Special Edition extraction
                ulong filesz = this.Size & 0x3fffffff;
                if (this.Archive.ContainsFileNameBlobs)
                {
                    int len = this.BinaryReader.ReadByte();
                    filesz -= (ulong)len + 1;
                    this.BinaryReader.BaseStream.Seek((int)this.Offset + 1 + len, SeekOrigin.Begin);
                }

                uint filesize = (uint)filesz;
                if (this.Size > 0 && this.Compressed)
                {
                    filesize = this.BinaryReader.ReadUInt32();
                    filesz -= 4;
                }

                byte[] content = this.BinaryReader.ReadBytes((int)filesz);

                if (!decompress || this.Compressed == false)
                {
                    stream.Write(content, 0, content.Length);
                }
                else
                {
                    using (var ms = new MemoryStream(content, false))
                    using (var lz4Stream = lz4.LZ4Stream.CreateDecompressor(ms, lz4.LZ4StreamMode.Read))
                    {
                        lz4Stream.CopyTo(stream);
                    }
                }
            }
            else
            {
                // Skip ahead
                if (this.Archive.ContainsFileNameBlobs)
                    this.BinaryReader.BaseStream.Position += this.BinaryReader.ReadByte() + 1;

                if (!decompress || !this.Compressed)
                {
                    byte[] content = this.BinaryReader.ReadBytes((int)this.Size);
                    stream.Write(content, 0, content.Length);
                }
                else
                {
                    byte[] uncompressed;
                    if (this.RealSize == 0)
                        uncompressed = new byte[this.BinaryReader.ReadUInt32()];
                    else
                        uncompressed = new byte[this.RealSize];
                    byte[] compressed = new byte[this.Size - 4];
                    this.BinaryReader.Read(compressed, 0, compressed.Length);
                    this.Archive.Inflater.Reset();
                    this.Archive.Inflater.SetInput(compressed);
                    this.Archive.Inflater.Inflate(uncompressed);
                    stream.Write(uncompressed, 0, uncompressed.Length);
                }
            }
        }
    }
}
