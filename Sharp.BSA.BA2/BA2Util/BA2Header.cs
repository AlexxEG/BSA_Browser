using System;
using System.IO;

namespace SharpBSABA2.BA2Util
{
    public enum CompressionFormat
    {
        Zip,
        LZ4
    }

    public struct BA2Header
    {
        public BA2HeaderMagic Magic { get; private set; }
        public uint Version { get; private set; }
        public BA2HeaderType Type { get; private set; }
        public uint NumFiles { get; private set; }
        public ulong NameTableOffset { get; private set; }
        public uint Unknown1 { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Unknown3 { get; private set; }

        public CompressionFormat CompressionFormat { get; private set; }

        public BA2Header(BinaryReader br)
        {
            Magic = ParseMagic(br.ReadChars(4));
            Version = br.ReadUInt32();
            Type = ParseType(br.ReadChars(4));
            NumFiles = br.ReadUInt32();
            NameTableOffset = br.ReadUInt64();

            Unknown1 = 0;
            Unknown2 = 0;
            Unknown3 = 0;

            if (Version == 2)
            {
                Unknown1 = br.ReadUInt32();
                Unknown2 = br.ReadUInt32();
            }

            if (Version == 3)
            {
                Unknown1 = br.ReadUInt32();
                Unknown2 = br.ReadUInt32();
                Unknown3 = br.ReadUInt32();
            }

            // If version is 3, then Unknown1 means which compression format is used. TODO: Consider renaming Unknown1
            if (Version == 3)
            {
                CompressionFormat = Unknown1 == 1 ? CompressionFormat.LZ4 : CompressionFormat.Zip;
            }
            else
            {
                CompressionFormat = CompressionFormat.Zip;
            }
        }

        private static BA2HeaderMagic ParseMagic(char[] chars)
        {
            string magic = new string(chars);
            if (Enum.TryParse(magic, true, out BA2HeaderMagic magicParsed))
                return magicParsed;
            else
                throw new Exception($"Unknown {nameof(BA2Header)}.{nameof(Magic)} value: ${magic}");
        }

        private static BA2HeaderType ParseType(char[] chars)
        {
            string type = new string(chars);
            if (Enum.TryParse(type, true, out BA2HeaderType typeParsed))
                return typeParsed;
            else
                throw new Exception($"Unknown {nameof(BA2Header)}.{nameof(Type)} value: ${type}");
        }

        public override string ToString()
        {
            return $"Magic: {Magic} Version: {Version} Type: {Type} NumFiles: {NumFiles} NameTableOffset: {NameTableOffset}";
        }
    }
}
