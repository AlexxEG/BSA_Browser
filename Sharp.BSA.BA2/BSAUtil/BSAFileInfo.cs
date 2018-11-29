using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public struct BSAFileInfo
    {
        public ulong Hash { get; private set; }
        public uint SizeFlags { get; private set; }
        public uint Offset { get; private set; }

        public BSAFileInfo(BinaryReader reader)
        {
            this.Hash = reader.ReadUInt64();
            this.SizeFlags = reader.ReadUInt32();
            this.Offset = reader.ReadUInt32();
        }
    }
}
