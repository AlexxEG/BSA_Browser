using System.Collections.Generic;
using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class BSAFolderInfo
    {
        public ulong Hash { get; private set; }
        public uint FileCount { get; private set; }
        public uint Unk1 { get; private set; }
        public ulong Offset { get; private set; }

        public string FolderName { get; set; }

        public List<BSAFileInfo> Files { get; private set; } = new List<BSAFileInfo>();

        public BSAFolderInfo(BinaryReader reader, int version)
        {
            this.Hash = reader.ReadUInt64();
            this.FileCount = reader.ReadUInt32();

            if (version == BSA.SSE_HEADER_VERSION)
                this.Unk1 = reader.ReadUInt32();

            this.Offset = version == BSA.SSE_HEADER_VERSION ? reader.ReadUInt64() : reader.ReadUInt32();
        }
    }
}
