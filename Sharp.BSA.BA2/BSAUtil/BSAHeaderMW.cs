using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public struct BSAHeaderMW
    {
        public uint Version;
        public uint HashOffset;
        public uint FileCount;

        public const uint Size = 12;

        public BSAHeaderMW(BinaryReader br, uint version)
        {
            this.Version = version;
            this.HashOffset = br.ReadUInt32();
            this.FileCount = br.ReadUInt32();
        }
    }
}
