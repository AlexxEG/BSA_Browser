using System.IO;
using SharpBSABA2.Extensions;

namespace SharpBSABA2.BSAUtil
{
    public struct DAT2FileEntry
    {
        public int FilenameSize;
        public string Filename;
        public bool Compressed;
        public uint RealSize;
        public uint PackedSize;
        public uint Offset;

        public DAT2FileEntry(BinaryReader reader)
        {
            this.FilenameSize = reader.ReadInt32();
            this.Filename = reader.ReadString(this.FilenameSize).TrimStart('\\');
            this.Compressed = reader.ReadByte() == 1;
            this.RealSize = reader.ReadUInt32();
            this.PackedSize = reader.ReadUInt32();
            this.Offset = reader.ReadUInt32();
        }
    }
}
