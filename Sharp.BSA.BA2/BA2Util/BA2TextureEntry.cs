using System;
using System.Collections.Generic;
using System.IO;

namespace SharpBSABA2.BA2Util
{
    public class UnsupportedDDSException : Exception
    {
        public UnsupportedDDSException() : base() { }
        public UnsupportedDDSException(string message) : base(message) { }
        public UnsupportedDDSException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BA2TextureEntry : ArchiveEntry
    {
        public List<BA2TextureChunk> Chunks { get; private set; } = new List<BA2TextureChunk>();

        public readonly byte unk8;
        public readonly byte numChunks;
        public readonly ushort chunkHdrLen;
        public readonly ushort height;
        public readonly ushort width;
        public readonly byte numMips;
        public readonly byte format;
        public readonly ushort unk16;

        public override bool Compressed
        {
            get
            {
                // After testing it seems like ALL textures are compressed.
                return this.Chunks[0].packSz != 0;
            }
        }
        public override uint DisplaySize
        {
            get
            {
                uint size = 0;
                foreach (var chunk in this.Chunks)
                    size += chunk.fullSz;
                return size;
            }
        }
        public override ulong Offset
        {
            get
            {
                return this.Chunks[0].offset;
            }
        }

        public BA2TextureEntry(Archive ba2) : base(ba2)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            FullPath = nameHash.ToString("X");
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();
            unk8 = ba2.BinaryReader.ReadByte();
            numChunks = ba2.BinaryReader.ReadByte();
            chunkHdrLen = ba2.BinaryReader.ReadUInt16();
            height = ba2.BinaryReader.ReadUInt16();
            width = ba2.BinaryReader.ReadUInt16();
            numMips = ba2.BinaryReader.ReadByte();
            format = ba2.BinaryReader.ReadByte();
            unk16 = ba2.BinaryReader.ReadUInt16();

            for (int i = 0; i < numChunks; i++)
            {
                this.Chunks.Add(new BA2TextureChunk(ba2.BinaryReader));
            }
        }

        public override MemoryStream GetRawDataStream()
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms, false);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private void WriteHeader(BinaryWriter bw)
        {
            var ddsHeader = new DDS_HEADER();

            ddsHeader.dwSize = ddsHeader.GetSize();
            ddsHeader.dwHeaderFlags = DDS.DDS_HEADER_FLAGS_TEXTURE | DDS.DDS_HEADER_FLAGS_LINEARSIZE | DDS.DDS_HEADER_FLAGS_MIPMAP;
            ddsHeader.dwHeight = height;
            ddsHeader.dwWidth = width;
            ddsHeader.dwMipMapCount = numMips;
            ddsHeader.PixelFormat.dwSize = ddsHeader.PixelFormat.GetSize();
            ddsHeader.dwSurfaceFlags = DDS.DDS_SURFACE_FLAGS_TEXTURE | DDS.DDS_SURFACE_FLAGS_MIPMAP;

            switch ((DXGI_FORMAT)format)
            {
                case DXGI_FORMAT.BC1_UNORM:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '1');
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height / 2); // 4bpp
                    break;
                case DXGI_FORMAT.BC2_UNORM:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '3');
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height); // 8bpp
                    break;
                case DXGI_FORMAT.BC3_UNORM:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '5');
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height); // 8bpp
                    break;
                case DXGI_FORMAT.BC5_UNORM:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                    if ((this.Archive as BA2).UseATIFourCC)
                        ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('A', 'T', 'I', '2'); // this is more correct but the only thing I have found that supports it is the nvidia photoshop plugin
                    else
                        ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '5');
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height); // 8bpp
                    break;
                case DXGI_FORMAT.BC1_UNORM_SRGB:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', '1', '0');
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height / 2); // 4bpp
                    break;
                case DXGI_FORMAT.BC3_UNORM_SRGB:
                case DXGI_FORMAT.BC4_UNORM:
                case DXGI_FORMAT.BC5_SNORM:
                case DXGI_FORMAT.BC7_UNORM:
                case DXGI_FORMAT.BC7_UNORM_SRGB:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', '1', '0');
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height); // 8bpp
                    break;
                case DXGI_FORMAT.R8G8B8A8_UNORM:
                case DXGI_FORMAT.R8G8B8A8_UNORM_SRGB:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_RGBA;
                    ddsHeader.PixelFormat.dwRGBBitCount = 32;
                    ddsHeader.PixelFormat.dwRBitMask = 0x000000FF;
                    ddsHeader.PixelFormat.dwGBitMask = 0x0000FF00;
                    ddsHeader.PixelFormat.dwBBitMask = 0x00FF0000;
                    ddsHeader.PixelFormat.dwABitMask = 0xFF000000;
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height * 4); // 32bpp
                    break;
                case DXGI_FORMAT.B8G8R8A8_UNORM:
                case DXGI_FORMAT.B8G8R8X8_UNORM:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_RGBA;
                    ddsHeader.PixelFormat.dwRGBBitCount = 32;
                    ddsHeader.PixelFormat.dwRBitMask = 0x00FF0000;
                    ddsHeader.PixelFormat.dwGBitMask = 0x0000FF00;
                    ddsHeader.PixelFormat.dwBBitMask = 0x000000FF;
                    ddsHeader.PixelFormat.dwABitMask = 0xFF000000;
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height * 4); // 32bpp
                    break;
                case DXGI_FORMAT.R8_UNORM:
                    ddsHeader.PixelFormat.dwFlags = DDS.DDS_RGB;
                    ddsHeader.PixelFormat.dwRGBBitCount = 8;
                    ddsHeader.PixelFormat.dwRBitMask = 0xFF;
                    ddsHeader.dwPitchOrLinearSize = (uint)(width * height); // 8bpp
                    break;
                default:
                    throw new UnsupportedDDSException("Unsupported DDS header format. File: " + this.FullPath);
            }

            bw.Write((uint)DDS.DDS_MAGIC);
            ddsHeader.Write(bw);

            switch ((DXGI_FORMAT)format)
            {
                case DXGI_FORMAT.BC1_UNORM_SRGB:
                case DXGI_FORMAT.BC3_UNORM_SRGB:
                case DXGI_FORMAT.BC4_UNORM:
                case DXGI_FORMAT.BC5_SNORM:
                case DXGI_FORMAT.BC7_UNORM:
                case DXGI_FORMAT.BC7_UNORM_SRGB:
                    var dxt10 = new DDS_HEADER_DXT10()
                    {
                        dxgiFormat = format,
                        resourceDimension = (uint)DXT10_RESOURCE_DIMENSION.DIMENSION_TEXTURE2D,
                        miscFlag = 0,
                        arraySize = 1,
                        miscFlags2 = DDS.DDS_ALPHA_MODE_UNKNOWN
                    };
                    dxt10.Write(bw);
                    break;
            }
        }

        protected override void WriteDataToStream(Stream stream)
        {
            this.WriteDataToStream(stream, true);
        }

        protected void WriteDataToStream(Stream stream, bool decompress)
        {
            var bw = new BinaryWriter(stream);

            if (decompress)
            {
                this.WriteHeader(bw);
            }

            for (int i = 0; i < numChunks; i++)
            {
                byte[] full = new byte[this.Chunks[i].fullSz];
                bool isCompressed = this.Chunks[i].packSz != 0;

                this.BinaryReader.BaseStream.Seek((long)this.Chunks[i].offset, SeekOrigin.Begin);

                if (!decompress || !isCompressed)
                {
                    this.BinaryReader.Read(full, 0, full.Length);
                }
                else
                {
                    byte[] compressed = new byte[this.Chunks[i].packSz];

                    this.BinaryReader.Read(compressed, 0, compressed.Length);
                    // Uncompress
                    this.Archive.Decompress(compressed, full);
                }

                bw.Write(full);
            }
        }
    }
}
