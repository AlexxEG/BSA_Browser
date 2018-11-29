using System;
using System.IO;
using SharpBSABA2.Enums;
using SharpBSABA2.Extensions;

namespace SharpBSABA2.BA2Util
{
    public class BA2 : Archive
    {
        public BA2Header Header { get; set; }

        public bool UseATIFourCC { get; set; } = false;

        public new int FileCount => (int)Header.numFiles;
        public new bool HasNameTable => Header.nameTableOffset > 0;
        public new string VersionString => Header.version.ToString();

        public BA2(string filePath) : base(filePath)
        {
        }

        protected override void Open(string filePath)
        {
            this.Header = new BA2Header(BinaryReader);

            // Set archive type
            if (Enum.TryParse("BA2_" + this.Header.Type, out ArchiveTypes type))
                this.Type = type;
            else
                throw new Exception($"Unknown {nameof(BA2HeaderType)} value: {this.Header.Type}");

            for (int i = 0; i < this.FileCount; i++)
                if (this.Header.Type == BA2HeaderType.GNRL)
                    this.Files.Add(new BA2FileEntry(this));
                else if (this.Header.Type == BA2HeaderType.DX10)
                    this.Files.Add(new BA2TextureEntry(this));
                else if (this.Header.Type == BA2HeaderType.GNMF)
                    this.Files.Add(new BA2GNFEntry(this));

            if (this.HasNameTable)
            {
                // Seek to name table
                BinaryReader.BaseStream.Seek((long)Header.nameTableOffset, SeekOrigin.Begin);
            }

            // Assign full names to each file
            for (int i = 0; i < this.FileCount; i++)
            {
                if (!this.HasNameTable)
                {
                    this.Files[i].FullPath = this.Files[i].nameHash.ToString("X");
                }
                else
                {
                    short length = BinaryReader.ReadInt16();
                    this.Files[i].FullPath = this.BinaryReader.ReadString(length);
                }
            }
        }
    }
}
