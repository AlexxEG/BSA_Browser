using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public struct BSAHeader
    {
        public uint FolderRecordOffset;
        public uint ArchiveFlags;
        public uint FolderCount;
        public uint FileCount;
        public uint FolderNameLength;
        public uint FileNameLength;
        public uint FileFlags;

        public BSAHeader(BinaryReader reader)
        {
            FolderRecordOffset = reader.ReadUInt32();
            ArchiveFlags = reader.ReadUInt32();
            FolderCount = reader.ReadUInt32();
            FileCount = reader.ReadUInt32();
            FolderNameLength = reader.ReadUInt32();
            FileNameLength = reader.ReadUInt32();
            FileFlags = reader.ReadUInt32();
        }
    }
}
