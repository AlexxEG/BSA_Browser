using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpBSABA2.BA2Util
{
    public class BA2GNFEntry : ArchiveEntry
    {
        private const int GNF_HEADER_MAGIC = 0x20464E47;
        private const int GNF_HEADER_CONTENT_SIZE = 248;

        public List<BA2TextureChunk> Chunks { get; private set; } = new List<BA2TextureChunk>();

        /// <summary>
        /// Unknown.
        /// </summary>
        private uint unk { get; set; }
        /// <summary>
        /// Unknown. 00 00 00 00.
        /// </summary>
        private uint unk2 { get; set; }
        private uint align { get; set; }
        private uint numChunks { get; set; }

        /// <summary>
        /// Part of the header that will be in GNF file.
        /// </summary>
        private byte[] GNFHeader { get; set; }

        public override uint DisplaySize
        {
            get
            {
                uint size = this.RealSize;
                foreach (var chunk in this.Chunks)
                    size += chunk.fullSz;
                return size;
            }
        }

        public BA2GNFEntry(Archive ba2, int index) : base(ba2, index)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();

            ba2.BinaryReader.ReadByte(); // Unknown
            numChunks = ba2.BinaryReader.ReadByte();
            unk = ba2.BinaryReader.ReadUInt16();

            GNFHeader = ba2.BinaryReader.ReadBytes(32);
            Offset = ba2.BinaryReader.ReadUInt64();
            Size = ba2.BinaryReader.ReadUInt32();
            RealSize = ba2.BinaryReader.ReadUInt32();
            unk2 = ba2.BinaryReader.ReadUInt32();
            align = ba2.BinaryReader.ReadUInt32();

            for (int i = 0; i < (numChunks - 1); i++)
            {
                this.Chunks.Add(new BA2TextureChunk(ba2.BinaryReader));
            }
        }

        public override MemoryStream GetRawDataStream()
        {
            var ms = new MemoryStream();

            this.WriteDateToStream(ms, false);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        protected override void WriteDataToStream(Stream stream)
        {
            this.WriteDateToStream(stream, true);
        }

        protected void WriteDateToStream(Stream stream, bool decompress)
        {
            if (decompress)
            {
                var writer = new BinaryWriter(stream);

                writer.Write(GNF_HEADER_MAGIC); // 'GNF ' magic
                writer.Write(GNF_HEADER_CONTENT_SIZE); // Content-size. Seems to be either 4 or 8 bytes

                writer.Write((byte)0x2); // Version
                writer.Write((byte)0x1); // Texture Count
                writer.Write((byte)0x8); // Alignment
                writer.Write((byte)0x0); // Unused

                writer.Write(BitConverter.GetBytes(this.RealSize + 256).Reverse().ToArray()); // File size + header size
                writer.Write(this.GNFHeader);

                for (int i = 0; i < 208; i++)
                    writer.Write((byte)0x0); // Padding
            }

            byte[] bytes = new byte[this.Size];
            byte[] uncompressed = new byte[this.RealSize];

            BinaryReader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);
            BinaryReader.Read(bytes, 0, (int)this.Size);

            if (!decompress)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                try
                {
                    this.Archive.Inflater.Reset();
                    this.Archive.Inflater.SetInput(bytes);
                    this.Archive.Inflater.Inflate(uncompressed);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Couldn't decompress zlib texture data. Size: {this.Size}, RealSize: {this.RealSize}", ex);
                }

                stream.Write(uncompressed, 0, uncompressed.Length);
            }

            this.WriteChunks(stream, decompress);
        }

        private void WriteChunks(Stream stream, bool decompress)
        {
            for (int i = 0; i < (numChunks - 1); i++)
            {
                byte[] compressed = new byte[this.Chunks[i].packSz];
                byte[] full = new byte[this.Chunks[i].fullSz];

                this.BinaryReader.BaseStream.Seek((long)this.Chunks[i].offset, SeekOrigin.Begin);
                this.BinaryReader.Read(compressed, 0, compressed.Length);

                if (!decompress)
                {
                    stream.Write(compressed, 0, compressed.Length);
                }
                else
                {
                    // Uncompress
                    this.Archive.Inflater.Reset();
                    this.Archive.Inflater.SetInput(compressed);
                    this.Archive.Inflater.Inflate(full);

                    stream.Write(full, 0, full.Length);
                }
            }
        }
    }
}
