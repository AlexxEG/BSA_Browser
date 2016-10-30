using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class BSAFileInfo
    {
        public ulong Hash { get; private set; }
        public uint SizeFlags { get; private set; }
        public uint Offset { get; private set; }

        public bool NamePrefix { get; set; }
        public bool Compressed { get; set; }

        public string Name { get; set; }

        private BinaryReader _reader;

        public BSAFileInfo(BinaryReader reader)
        {
            _reader = reader;

            this.Hash = _reader.ReadUInt64();
            this.SizeFlags = _reader.ReadUInt32();
            this.Offset = _reader.ReadUInt32();
        }

        public static int SizeOf()
        {
            return 16;
        }

        public static BSAFileInfo ReadFrom(BinaryReader reader)
        {
            return new BSAFileInfo(reader);
        }
    }
}
