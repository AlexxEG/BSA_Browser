using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpBSABA2.Utils;

namespace SharpBSABA2.BA2Util
{
    public class BA2GNFEntry : ArchiveEntry
    {
        private const int GNF_HEADER_MAGIC = 0x20464E47;
        private const int GNF_HEADER_CONTENT_SIZE = 248;
        private const int IntFirst14BitMask = (1 << 14) - 1;

        public List<BA2TextureChunk> Chunks { get; private set; } = new List<BA2TextureChunk>();

        /// <summary>
        /// Unknown.
        /// </summary>
        private ushort chunkHdrLen { get; set; }
        /// <summary>
        /// Unknown. 00 00 00 00.
        /// </summary>
        private uint unk2 { get; set; }
        private uint align { get; set; }

        public readonly uint numChunks;
        public readonly uint format;
        public readonly uint numFormat;
        public readonly uint height;
        public readonly uint width;

        /// <summary>
        /// Part of the header that will be in GNF file.
        /// </summary>
        public byte[] GNFHeader { get; set; }

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

        public override ulong GetSizeInArchive(SharedExtractParams extractParams) => Math.Max(this.Size, this.RealSize);

        public BA2GNFEntry(Archive ba2) : base(ba2)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();

            FullPath = dirHash > 0 ? $"{dirHash:X}_" : string.Empty;
            FullPath += $"{nameHash:X}.{Extension.TrimEnd('\0')}";
            FullPathOriginal = FullPath;

            ba2.BinaryReader.ReadByte(); // Unknown
            numChunks = ba2.BinaryReader.ReadByte();
            chunkHdrLen = ba2.BinaryReader.ReadUInt16();

            GNFHeader = ba2.BinaryReader.ReadBytes(32);

            uint formatInfo = BitConverter.ToUInt32(GNFHeader.Skip(4).Take(4).ToArray(), 0);
            format = formatInfo >> 20 & ((1 << 6) - 1); // Skip first 20 bits then take 6 next bits
            numFormat = formatInfo >> 26 & ((1 << 4) - 1); // Skip first 26 bits then take 4 next bits

            uint size = BitConverter.ToUInt32(GNFHeader.Skip(8).Take(4).ToArray(), 0);
            width = (size & IntFirst14BitMask) + 1; // Get first 14 bits
            height = (size >> 14 & IntFirst14BitMask) + 1; // Shifts past first 14 bits then get first 14 bits again

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

        public override string GetToolTipText()
        {
            string dxgi = Enum.GetName(typeof(DXGI_FORMAT_FULL), format);

            return $"Name hash:\t {nameHash}\n" +
                $"Directory hash:\t {dirHash}\n" +
                $"DXGI format:\t {dxgi} ({format})\n" +
                $"Resolution:\t {width}x{height}\n" +
                $"Chunks:\t\t {numChunks}\n" +
                $"Chunk header len:\t {chunkHdrLen}\n" +
                $"Num format:\t {numFormat}\n" +
                $"Offset:\t\t {Offset}\n" +
                $"Size:\t\t {Size}\n" +
                $"Real Size:\t {RealSize}\n" +
                $"Align:\t\t {align:X}\n\n" +
                $"{nameof(unk2)}:\t\t {unk2}";
        }

        protected override void WriteDataToStream(Stream stream, SharedExtractParams extractParams, bool decompress)
        {
            var reader = extractParams.Reader;
            reader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);
            // Reset at start since value might still be in used for a bit after
            this.BytesWritten = 0;

            if (!decompress)
            {
                StreamUtils.WriteSectionToStream(reader.BaseStream,
                    Math.Max(this.Size, this.RealSize), // Lazy hack, only one should be set when not compressed
                    stream,
                    bytesWritten => this.BytesWritten = bytesWritten);
            }
            else
            {
                this.WriteHeader(stream);

                try
                {
                    CompressionUtils.Decompress(reader.BaseStream,
                        this.Size,
                        stream,
                        bytesWritten => this.BytesWritten = bytesWritten,
                        extractParams);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Couldn't decompress zlib texture data. Size: {this.Size}, RealSize: {this.RealSize}", ex);
                }
            }

            this.WriteChunks(stream, extractParams, decompress);
        }

        private void WriteChunks(Stream stream, SharedExtractParams extractParams, bool decompress)
        {
            var reader = extractParams.Reader;

            for (int i = 0; i < (numChunks - 1); i++)
            {
                reader.BaseStream.Seek((long)this.Chunks[i].offset, SeekOrigin.Begin);


                if (!decompress)
                {
                    ulong prev = this.BytesWritten;
                    StreamUtils.WriteSectionToStream(reader.BaseStream,
                        Math.Max(Chunks[i].packSz, Chunks[i].fullSz),  // Lazy hack, only one should be set when not compressed
                        stream,
                        bytesWritten => this.BytesWritten = prev + bytesWritten);
                }
                else
                {
                    ulong prev = this.BytesWritten;
                    CompressionUtils.Decompress(reader.BaseStream,
                        this.Chunks[i].packSz,
                        stream,
                        bytesWritten => this.BytesWritten = prev + bytesWritten,
                        extractParams);
                }
            }
        }

        private void WriteHeader(Stream stream)
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
    }
}
