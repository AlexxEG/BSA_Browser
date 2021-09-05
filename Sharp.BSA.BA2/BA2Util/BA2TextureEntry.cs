using ICSharpCode.SharpZipLib.Zip.Compression;
using SharpBSABA2.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private const uint TILE_MODE_DEFAULT = 0x08;
        private const uint XBOX_BASE_ALIGNMENT = 256;
        private const uint XBOX_XDK_VERSION = 13202;

        /// <summary>
        /// Gets or sets whether to generate DDS header.
        /// </summary>
        public bool GenerateTextureHeader { get; set; } = true;
        public List<BA2TextureChunk> Chunks { get; private set; } = new List<BA2TextureChunk>();
        public long dataSizePosition = -1;

        public readonly byte unk1;
        public readonly byte numChunks;
        public readonly ushort chunkHdrLen;
        public readonly ushort height;
        public readonly ushort width;
        public readonly byte numMips;
        public readonly byte format;
        public readonly byte isCubemap;
        public readonly byte tileMode;

        // After testing it seems like ALL textures are compressed.
        public override bool Compressed => this.Chunks[0].packSz != 0;
        public override uint Size
        {
            get
            {
                uint size = this.GetHeaderSize();
                bool compressed = Chunks[0].packSz != 0;

                foreach (var chunk in Chunks)
                    size += compressed ? chunk.packSz : chunk.fullSz;

                return size;
            }
        }
        // Start of with the size of the DDS Magic + DDS header + if applicable DDS DXT10 header
        public override uint RealSize => this.GetHeaderSize() + (uint)this.Chunks.Sum(x => Math.Max(x.fullSz, x.packSz));
        public override uint DisplaySize => this.GetHeaderSize() + (uint)this.Chunks.Sum(x => x.fullSz);
        public override ulong Offset => this.Chunks[0].offset;

        public BA2TextureEntry(Archive ba2) : base(ba2)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();

            FullPath = dirHash > 0 ? $"{dirHash:X}_" : string.Empty;
            FullPath += $"{nameHash:X}.{Extension.TrimEnd('\0')}";
            FullPathOriginal = FullPath;

            unk1 = ba2.BinaryReader.ReadByte();
            numChunks = ba2.BinaryReader.ReadByte();
            chunkHdrLen = ba2.BinaryReader.ReadUInt16();
            height = ba2.BinaryReader.ReadUInt16();
            width = ba2.BinaryReader.ReadUInt16();
            numMips = ba2.BinaryReader.ReadByte();
            format = ba2.BinaryReader.ReadByte();
            isCubemap = ba2.BinaryReader.ReadByte();
            tileMode = ba2.BinaryReader.ReadByte();

            for (int i = 0; i < numChunks; i++)
            {
                this.Chunks.Add(new BA2TextureChunk(ba2.BinaryReader));
            }
        }

        public override string GetToolTipText()
        {
            return $"{nameof(nameHash)}: {nameHash}\n" +
                $"{nameof(FullPath)}: {FullPath}\n" +
                $"{nameof(Extension)}: {Extension.TrimEnd('\0')}\n" +
                $"{nameof(dirHash)}: {dirHash}\n" +
                $"{nameof(unk1)}: {unk1}\n" +
                $"{nameof(numChunks)}: {numChunks}\n" +
                $"{nameof(chunkHdrLen)}: {chunkHdrLen}\n" +
                $"{nameof(height)}: {height}\n" +
                $"{nameof(width)}: {width}\n" +
                $"{nameof(numMips)}: {numMips}\n" +
                $"{nameof(format)}: {format}\n" +
                $"{nameof(isCubemap)}: {isCubemap}\n" +
                $"{nameof(tileMode)}: {tileMode}";
        }

        public bool IsFormatSupported()
        {
            return Enum.IsDefined(typeof(DXGI_FORMAT), (int)format);
        }

        private uint GetHeaderSize()
        {
            uint size = 0;

            // DDS.DDS_MAGIC
            size += 4;
            size += DDS_HEADER.GetSize();

            // If DXT10 add that size too
            switch ((DXGI_FORMAT)format)
            {
                case DXGI_FORMAT.BC1_UNORM_SRGB:
                case DXGI_FORMAT.BC3_UNORM_SRGB:
                case DXGI_FORMAT.BC4_UNORM:
                case DXGI_FORMAT.BC5_SNORM:
                case DXGI_FORMAT.BC7_UNORM:
                case DXGI_FORMAT.BC7_UNORM_SRGB:
                    size += DDS_HEADER_DXT10.GetSize();
                    break;
            }
            return size;
        }

        private void WriteHeader(BinaryWriter bw)
        {
            var ddsHeader = new DDS_HEADER();

            ddsHeader.dwSize = DDS_HEADER.GetSize();
            ddsHeader.dwHeaderFlags = DDS.DDS_HEADER_FLAGS_TEXTURE | DDS.DDS_HEADER_FLAGS_LINEARSIZE | DDS.DDS_HEADER_FLAGS_MIPMAP;
            ddsHeader.dwHeight = height;
            ddsHeader.dwWidth = width;
            ddsHeader.dwMipMapCount = numMips;
            ddsHeader.PixelFormat.dwSize = DDS_PIXELFORMAT.GetSize();
            ddsHeader.dwSurfaceFlags = DDS.DDS_SURFACE_FLAGS_TEXTURE | DDS.DDS_SURFACE_FLAGS_MIPMAP;
            ddsHeader.dwCubemapFlags = isCubemap == 1 ? (uint)(DDSCAPS2.CUBEMAP
                | DDSCAPS2.CUBEMAP_NEGATIVEX | DDSCAPS2.CUBEMAP_POSITIVEX
                | DDSCAPS2.CUBEMAP_NEGATIVEY | DDSCAPS2.CUBEMAP_POSITIVEY
                | DDSCAPS2.CUBEMAP_NEGATIVEZ | DDSCAPS2.CUBEMAP_POSITIVEZ
                | DDSCAPS2.CUBEMAP_ALLFACES) : 0u;

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
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('A', 'T', 'I', '2');
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

            // If tileMode is NOT TILE_MODE_DEFAULT assume Xbox format
            if (tileMode != TILE_MODE_DEFAULT)
            {
                switch ((DXGI_FORMAT)format)
                {
                    case DXGI_FORMAT.BC1_UNORM:
                    case DXGI_FORMAT.BC1_UNORM_SRGB:
                    case DXGI_FORMAT.BC2_UNORM:
                    case DXGI_FORMAT.BC3_UNORM:
                    case DXGI_FORMAT.BC3_UNORM_SRGB:
                    case DXGI_FORMAT.BC4_UNORM:
                    case DXGI_FORMAT.BC5_SNORM:
                    case DXGI_FORMAT.BC5_UNORM:
                    case DXGI_FORMAT.BC7_UNORM:
                    case DXGI_FORMAT.BC7_UNORM_SRGB:
                        ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('X', 'B', 'O', 'X');
                        break;
                }
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
                    new DDS_HEADER_DXT10()
                    {
                        dxgiFormat = format,
                        resourceDimension = (uint)DXT10_RESOURCE_DIMENSION.DIMENSION_TEXTURE2D,
                        miscFlag = isCubemap == 1 ? DDS.DDS_RESOURCE_MISC_TEXTURECUBE : 0u,
                        arraySize = 1,
                        miscFlags2 = DDS.DDS_ALPHA_MODE_UNKNOWN
                    }.Write(bw);
                    break;
                default:
                    if (tileMode != TILE_MODE_DEFAULT)
                    {
                        new DDS_HEADER_DXT10()
                        {
                            dxgiFormat = format,
                            resourceDimension = (uint)DXT10_RESOURCE_DIMENSION.DIMENSION_TEXTURE2D,
                            miscFlag = isCubemap == 1 ? DDS.DDS_RESOURCE_MISC_TEXTURECUBE : 0u,
                            arraySize = 1,
                            miscFlags2 = DDS.DDS_ALPHA_MODE_UNKNOWN
                        }.Write(bw);
                    }
                    break;
            }

            // If tileMode is NOT TILE_MODE_DEFAULT assume Xbox format
            if (tileMode != TILE_MODE_DEFAULT)
            {
                bw.Write((uint)tileMode);
                bw.Write(XBOX_BASE_ALIGNMENT);
                dataSizePosition = bw.BaseStream.Position;
                bw.Write((uint)0);
                bw.Write(XBOX_XDK_VERSION);
            }
        }

        protected override void WriteDataToStream(Stream stream, BinaryReader reader, Inflater inflater, bool decompress)
        {
            var bw = new BinaryWriter(stream);

            // Reset at start since value might still be in used for a bit after
            this.BytesWritten = 0;

            if (decompress && GenerateTextureHeader)
            {
                this.WriteHeader(bw);
            }

            for (int i = 0; i < numChunks; i++)
            {
                bool isCompressed = this.Chunks[i].packSz != 0;
                ulong prev = this.BytesWritten;

                reader.BaseStream.Seek((long)this.Chunks[i].offset, SeekOrigin.Begin);

                if (!decompress || !isCompressed)
                {
                    Archive.WriteSectionToStream(reader.BaseStream,
                                                 Chunks[i].fullSz,
                                                 stream,
                                                 bytesWritten => this.BytesWritten = prev + bytesWritten);
                }
                else
                {
                    Archive.Decompress(reader.BaseStream,
                                       this.Chunks[i].packSz,
                                       stream,
                                       bytesWritten => this.BytesWritten = prev + bytesWritten,
                                       inflater);
                }
            }

            if (dataSizePosition > -1)
            {
                bw.WriteAt(dataSizePosition, (uint)bw.BaseStream.Length - 164);
            }
        }
    }
}
