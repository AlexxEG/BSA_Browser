using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class BSAHeader
    {
        private BinaryReader _reader;

        public uint FolderRecordOffset { get; private set; }
        public uint ArchiveFlags { get; private set; }
        public uint FolderCount { get; private set; }
        public uint FileCount { get; private set; }
        public uint FolderNameLength { get; private set; }
        public uint FileNameLength { get; private set; }
        public uint FileFlags { get; private set; }

        public BSAHeader(BinaryReader reader)
        {
            _reader = reader;

            this.FolderRecordOffset = _reader.ReadUInt32();
            this.ArchiveFlags = _reader.ReadUInt32();
            this.FolderCount = _reader.ReadUInt32();
            this.FileCount = _reader.ReadUInt32();
            this.FolderNameLength = _reader.ReadUInt32();
            this.FileNameLength = _reader.ReadUInt32();
            this.FileFlags = _reader.ReadUInt32();
        }

        public static BSAHeader ReadFrom(BinaryReader reader)
        {
            return new BSAHeader(reader);
        }
    }
}
