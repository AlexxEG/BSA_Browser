using System.IO;

namespace SharpBSABA2.BA2Util
{
    public class BA2TextureChunk
    {
        public ulong offset;
        public uint packSz;
        public uint fullSz;
        public ushort startMip;
        public ushort endMip;
        public uint align;

        public BA2TextureChunk(BinaryReader br)
        {
            offset = br.ReadUInt64();
            packSz = br.ReadUInt32();
            fullSz = br.ReadUInt32();
            startMip = br.ReadUInt16();
            endMip = br.ReadUInt16();
            align = br.ReadUInt32();
        }
    }
}
