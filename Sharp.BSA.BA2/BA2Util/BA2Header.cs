using System;
using System.IO;

namespace SharpBSABA2.BA2Util
{
    public class BA2Header
    {
        public BA2HeaderMagic Magic { get; set; }
        public uint version { get; set; }
        public BA2HeaderType Type { get; set; }
        public uint numFiles { get; set; }
        public ulong nameTableOffset { get; set; }

        public BA2Header(BinaryReader br)
        {
            BA2HeaderMagic magicParsed;
            if (Enum.TryParse(new string(br.ReadChars(4)), true, out magicParsed))
                this.Magic = magicParsed;
            else
                this.Magic = BA2HeaderMagic.Unknown;

            version = br.ReadUInt32();

            BA2HeaderType typeParsed;
            if (Enum.TryParse(new string(br.ReadChars(4)), true, out typeParsed))
                this.Type = typeParsed;
            else
                this.Type = BA2HeaderType.Unknown;

            numFiles = br.ReadUInt32();
            nameTableOffset = br.ReadUInt64();
        }

        public override string ToString()
        {
            return $"magic: {Magic} version: {version} type: {Type} numFiles: {numFiles} nameTableOffset: {nameTableOffset}";
        }
    }
}
