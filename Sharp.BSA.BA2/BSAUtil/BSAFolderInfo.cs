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

        private BinaryReader _reader;

        public BSAFolderInfo(BinaryReader reader, int version)
        {
            _reader = reader;

            this.Hash = _reader.ReadUInt64();
            this.FileCount = _reader.ReadUInt32();

            if (version == BSA.SSE_BSAHEADER_VERSION)
                this.Unk1 = _reader.ReadUInt32();

            this.Offset = _reader.ReadUInt64();
        }

        public static int SizeOf(int version)
        {
            if (version == BSA.SSE_BSAHEADER_VERSION)
                return 24;
            else
                return 20;
        }

        public static BSAFolderInfo ReadFrom(BinaryReader reader, int version)
        {
            return new BSAFolderInfo(reader, version);
        }
    }
}
