using System;
using System.Runtime.InteropServices;

/* 
 * Copied from dds.h. Includes (almost) only stuff I need in this project.
 * 
 * Link: https://github.com/digitalutopia1/BA2Lib/blob/master/BA2Lib/dds.h
 * 
 */

namespace SharpBSABA2.BA2Util
{
    public class DDS
    {
        public const int DDS_MAGIC = 0x20534444; // "DDS "

        public static uint MAKEFOURCC(char ch0, char ch1, char ch2, char ch3)
        {
            // This is alien to me...
            return ((uint)(byte)(ch0) | ((uint)(byte)(ch1) << 8) | ((uint)(byte)(ch2) << 16 | ((uint)(byte)(ch3) << 24)));
        }

        public const int DDS_FOURCC = 0x00000004; // DDPF_FOURCC
        public const int DDS_RGB = 0x00000040; // DDPF_RGB
        public const int DDS_RGBA = 0x00000041; // DDPF_RGB | DDPF_ALPHAPIXELS

        public const int DDS_HEADER_FLAGS_TEXTURE = 0x00001007; // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        public const int DDS_HEADER_FLAGS_MIPMAP = 0x00020000; // DDSD_MIPMAPCOUNT
        public const int DDS_HEADER_FLAGS_LINEARSIZE = 0x00080000; // DDSD_LINEARSIZE

        public const int DDS_SURFACE_FLAGS_TEXTURE = 0x00001000; // DDSCAPS_TEXTURE
        public const int DDS_SURFACE_FLAGS_MIPMAP = 0x00400008; // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP

        public const int DDS_ALPHA_MODE_UNKNOWN = 0x0;

        public const uint DDS_RESOURCE_MISC_TEXTURECUBE = 0x4;
    }

    #region dxgiformat.h

    /// <summary>
    /// Supported DXGI_FORMATS.
    /// </summary>
    public enum DXGI_FORMAT
    {
        R8G8B8A8_UNORM = 28,
        R8G8B8A8_UNORM_SRGB = 29,
        R8_UNORM = 61,
        BC1_UNORM = 71,
        BC1_UNORM_SRGB = 72,
        BC2_UNORM = 74,
        BC3_UNORM = 77,
        BC3_UNORM_SRGB = 78,
        BC4_UNORM = 80,
        BC5_UNORM = 83,
        BC5_SNORM = 84,
        B8G8R8A8_UNORM = 87,
        B8G8R8X8_UNORM = 88,
        BC7_UNORM = 98,
        BC7_UNORM_SRGB = 99
    }

    /// <summary>
    /// Full DXGI_FORMATS, not everything is supported.
    /// </summary>
    public enum DXGI_FORMAT_FULL : uint
    {
        UNKNOWN = 0,
        R32G32B32A32_TYPELESS = 1,
        R32G32B32A32_FLOAT = 2,
        R32G32B32A32_UINT = 3,
        R32G32B32A32_SINT = 4,
        R32G32B32_TYPELESS = 5,
        R32G32B32_FLOAT = 6,
        R32G32B32_UINT = 7,
        R32G32B32_SINT = 8,
        R16G16B16A16_TYPELESS = 9,
        R16G16B16A16_FLOAT = 10,
        R16G16B16A16_UNORM = 11,
        R16G16B16A16_UINT = 12,
        R16G16B16A16_SNORM = 13,
        R16G16B16A16_SINT = 14,
        R32G32_TYPELESS = 15,
        R32G32_FLOAT = 16,
        R32G32_UINT = 17,
        R32G32_SINT = 18,
        R32G8X24_TYPELESS = 19,
        D32_FLOAT_S8X24_UINT = 20,
        R32_FLOAT_X8X24_TYPELESS = 21,
        X32_TYPELESS_G8X24_UINT = 22,
        R10G10B10A2_TYPELESS = 23,
        R10G10B10A2_UNORM = 24,
        R10G10B10A2_UINT = 25,
        R11G11B10_FLOAT = 26,
        R8G8B8A8_TYPELESS = 27,
        R8G8B8A8_UNORM = 28,
        R8G8B8A8_UNORM_SRGB = 29,
        R8G8B8A8_UINT = 30,
        R8G8B8A8_SNORM = 31,
        R8G8B8A8_SINT = 32,
        R16G16_TYPELESS = 33,
        R16G16_FLOAT = 34,
        R16G16_UNORM = 35,
        R16G16_UINT = 36,
        R16G16_SNORM = 37,
        R16G16_SINT = 38,
        R32_TYPELESS = 39,
        D32_FLOAT = 40,
        R32_FLOAT = 41,
        R32_UINT = 42,
        R32_SINT = 43,
        R24G8_TYPELESS = 44,
        D24_UNORM_S8_UINT = 45,
        R24_UNORM_X8_TYPELESS = 46,
        X24_TYPELESS_G8_UINT = 47,
        R8G8_TYPELESS = 48,
        R8G8_UNORM = 49,
        R8G8_UINT = 50,
        R8G8_SNORM = 51,
        R8G8_SINT = 52,
        R16_TYPELESS = 53,
        R16_FLOAT = 54,
        D16_UNORM = 55,
        R16_UNORM = 56,
        R16_UINT = 57,
        R16_SNORM = 58,
        R16_SINT = 59,
        R8_TYPELESS = 60,
        R8_UNORM = 61,
        R8_UINT = 62,
        R8_SNORM = 63,
        R8_SINT = 64,
        A8_UNORM = 65,
        R1_UNORM = 66,
        R9G9B9E5_SHAREDEXP = 67,
        R8G8_B8G8_UNORM = 68,
        G8R8_G8B8_UNORM = 69,
        BC1_TYPELESS = 70,
        BC1_UNORM = 71,
        BC1_UNORM_SRGB = 72,
        BC2_TYPELESS = 73,
        BC2_UNORM = 74,
        BC2_UNORM_SRGB = 75,
        BC3_TYPELESS = 76,
        BC3_UNORM = 77,
        BC3_UNORM_SRGB = 78,
        BC4_TYPELESS = 79,
        BC4_UNORM = 80,
        BC4_SNORM = 81,
        BC5_TYPELESS = 82,
        BC5_UNORM = 83,
        BC5_SNORM = 84,
        B5G6R5_UNORM = 85,
        B5G5R5A1_UNORM = 86,
        B8G8R8A8_UNORM = 87,
        B8G8R8X8_UNORM = 88,
        R10G10B10_XR_BIAS_A2_UNORM = 89,
        B8G8R8A8_TYPELESS = 90,
        B8G8R8A8_UNORM_SRGB = 91,
        B8G8R8X8_TYPELESS = 92,
        B8G8R8X8_UNORM_SRGB = 93,
        BC6H_TYPELESS = 94,
        BC6H_UF16 = 95,
        BC6H_SF16 = 96,
        BC7_TYPELESS = 97,
        BC7_UNORM = 98,
        BC7_UNORM_SRGB = 99,
        AYUV = 100,
        Y410 = 101,
        Y416 = 102,
        NV12 = 103,
        P010 = 104,
        P016 = 105,
        _420_OPAQUE = 106,
        YUY2 = 107,
        Y210 = 108,
        Y216 = 109,
        NV11 = 110,
        AI44 = 111,
        IA44 = 112,
        P8 = 113,
        A8P8 = 114,
        B4G4R4A4_UNORM = 115,

        P208 = 130,
        V208 = 131,
        V408 = 132,


        FORCE_UINT = 0xffffffff
    }

    #endregion

    [Flags]
    public enum DDSCAPS2 : uint
    {
        CUBEMAP = 0x200,
        CUBEMAP_POSITIVEX = 0x400,
        CUBEMAP_NEGATIVEX = 0x800,
        CUBEMAP_POSITIVEY = 0x1000,
        CUBEMAP_NEGATIVEY = 0x2000,
        CUBEMAP_POSITIVEZ = 0x4000,
        CUBEMAP_NEGATIVEZ = 0x8000,
        CUBEMAP_ALLFACES = 0xFC00
    }

    public enum DXT10_RESOURCE_DIMENSION
    {
        DIMENSION_TEXTURE1D = 2,
        DIMENSION_TEXTURE2D = 3,
        DIMENSION_TEXTURE3D = 4,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DDS_HEADER
    {
        public uint dwSize;
        public uint dwHeaderFlags;
        public uint dwHeight;
        public uint dwWidth;
        public uint dwPitchOrLinearSize;
        public uint dwDepth; // only if DDS_HEADER_FLAGS_VOLUME is set in dwHeaderFlags
        public uint dwMipMapCount;
        public uint dwReserved1; // [11]
        public DDS_PIXELFORMAT PixelFormat; // ddspf
        public uint dwSurfaceFlags;
        public uint dwCubemapFlags;
        public uint dwReserved2; // [3]

        public static uint GetSize()
        {
            // 9 uint + DDS_PIXELFORMAT uints + 2 uint arrays (dwReserved1 and dwReserved2) with 14 uints total
            // each uint 4 bytes each
            return (9 * 4) + DDS_PIXELFORMAT.GetSize() + (14 * 4);
        }

        public void Write(System.IO.BinaryWriter bw)
        {
            bw.Write(dwSize);
            bw.Write(dwHeaderFlags);
            bw.Write(dwHeight);
            bw.Write(dwWidth);
            bw.Write(dwPitchOrLinearSize);
            bw.Write(dwDepth);
            bw.Write(dwMipMapCount);

            // Just write it multiple times, since it's never assigned a value anyway
            for (int i = 0; i < 11; i++)
                bw.Write(dwReserved1);

            // DDS_PIXELFORMAT
            bw.Write(PixelFormat.dwSize);
            bw.Write(PixelFormat.dwFlags);
            bw.Write(PixelFormat.dwFourCC);
            bw.Write(PixelFormat.dwRGBBitCount);
            bw.Write(PixelFormat.dwRBitMask);
            bw.Write(PixelFormat.dwGBitMask);
            bw.Write(PixelFormat.dwBBitMask);
            bw.Write(PixelFormat.dwABitMask);

            bw.Write(dwSurfaceFlags);
            bw.Write(dwCubemapFlags);

            // Just write it multiple times, since it's never assigned a value anyway
            for (int i = 0; i < 3; i++)
                bw.Write(dwReserved2);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DDS_HEADER_DXT10
    {
        public uint dxgiFormat;
        public uint resourceDimension;
        public uint miscFlag;
        public uint arraySize;
        public uint miscFlags2;

        public void Write(System.IO.BinaryWriter bw)
        {
            bw.Write(dxgiFormat);
            bw.Write(resourceDimension);
            bw.Write(miscFlag);
            bw.Write(arraySize);
            bw.Write(miscFlags2);
        }

        public static uint GetSize()
        {
            // 5 uints, each 4 bytes each
            return 5 * 4;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct DDS_PIXELFORMAT
    {
        public uint dwSize;
        public uint dwFlags;
        public uint dwFourCC;
        public uint dwRGBBitCount;
        public uint dwRBitMask;
        public uint dwGBitMask;
        public uint dwBBitMask;
        public uint dwABitMask;

        public DDS_PIXELFORMAT(uint size, uint flags, uint fourCC, uint rgbBitCount, uint rBitMask, uint gBitMask, uint bBitMask, uint aBitMask)
        {
            dwSize = size;
            dwFlags = flags;
            dwFourCC = fourCC;
            dwRGBBitCount = rgbBitCount;
            dwRBitMask = rBitMask;
            dwGBitMask = gBitMask;
            dwBBitMask = bBitMask;
            dwABitMask = aBitMask;
        }

        public static uint GetSize()
        {
            // 8 uints, each 4 bytes each
            return 8 * 4;
        }
    }
}
