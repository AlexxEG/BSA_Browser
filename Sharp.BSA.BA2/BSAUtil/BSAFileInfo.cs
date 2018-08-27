using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public struct BSAFileInfo
    {
        public ulong Hash;

        public uint SizeFlags;
        public uint Offset;

        public BSAFileInfo(BinaryReader reader)
        {
            this.Hash = reader.ReadUInt64();
            this.SizeFlags = reader.ReadUInt32();
            this.Offset = reader.ReadUInt32();
        }

        public static int SizeOf()
        {
            return 16;
        }
    }
}
