using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class BSAHeader
    {
        public uint Version { get; private set; }
        public uint FolderRecordOffset { get; private set; }
        public uint ArchiveFlags { get; private set; }
        public uint FolderCount { get; private set; }
        public uint FileCount { get; private set; }
        public uint FolderNameLength { get; private set; }
        public uint FileNameLength { get; private set; }
        public uint FileFlags { get; private set; }

        public BSAHeader(BinaryReader reader)
        {
            Version = reader.ReadUInt32();
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
