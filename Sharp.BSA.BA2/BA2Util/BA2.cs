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

        public BA2(string filePath) : base(filePath)
        {
        }

        protected override void Open(string filePath)
        {
            this.Header = new BA2Header(BinaryReader);

            // Set archive type
            switch (this.Header.Type)
            {
                case BA2HeaderType.DX10:
                    this.Type = ArchiveTypes.BA2_DX10;
                    break;
                case BA2HeaderType.GNMF:
                    this.Type = ArchiveTypes.BA2_GNMF;
                    break;
                case BA2HeaderType.GNRL:
                    this.Type = ArchiveTypes.BA2_GNRL;
                    break;
                case BA2HeaderType.Unknown:
                    throw new Exception($"Unknown {nameof(BA2HeaderType)} value: {this.Header.Type}");
            }

            for (int i = 0; i < this.Header.numFiles; i++)
                if (this.Header.Type == BA2HeaderType.GNRL)
                    this.Files.Add(new BA2FileEntry(this, i));
                else if (this.Header.Type == BA2HeaderType.DX10)
                    this.Files.Add(new BA2TextureEntry(this, i));
                else if (this.Header.Type == BA2HeaderType.GNMF)
                    this.Files.Add(new BA2GNFEntry(this, i));

            // Seek to name table
            BinaryReader.BaseStream.Seek((long)this.Header.nameTableOffset, SeekOrigin.Begin);

            // Assign full names to each file
            for (int i = 0; i < this.Header.numFiles; i++)
            {
                short length = BinaryReader.ReadInt16();
                this.Files[i].FullPath = this.BinaryReader.ReadString(length);
            }
        }
    }
}
