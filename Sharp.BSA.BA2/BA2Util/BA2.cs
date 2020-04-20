using SharpBSABA2.Enums;
using SharpBSABA2.Extensions;
using System;
using System.IO;
using System.Text;

namespace SharpBSABA2.BA2Util
{
    public class BA2 : Archive
    {
        public BA2Header Header { get; set; }

        public override int FileCount => (int)Header.NumFiles;
        public override bool HasNameTable => Header.NameTableOffset > 0;
        public override string VersionString => Header.Version.ToString();

        public BA2(string filePath) : base(filePath) { }
        public BA2(string filePath, Encoding encoding) : base(filePath, encoding) { }

        protected override void Open(string filePath)
        {
            this.Header = new BA2Header(BinaryReader);
            // Set more detailed archive type, used for comparing
            this.Type = this.ConvertType(this.Header.Type);

            for (int i = 0; i < this.FileCount; i++)
                switch (this.Header.Type)
                {
                    case BA2HeaderType.GNRL: Files.Add(new BA2FileEntry(this)); break;
                    case BA2HeaderType.DX10: Files.Add(new BA2TextureEntry(this)); break;
                    case BA2HeaderType.GNMF: Files.Add(new BA2GNFEntry(this)); break;
                    default:
                        throw new Exception($"Unknown {nameof(BA2HeaderType)} value: " + this.Header.Type);
                }

            if (this.HasNameTable)
            {
                // Seek to name table
                BinaryReader.BaseStream.Seek((long)Header.NameTableOffset, SeekOrigin.Begin);

                // Assign full names to each file
                for (int i = 0; i < this.FileCount; i++)
                    try
                    {
                        this.Files[i].FullPath = BinaryReader.ReadString(BinaryReader.ReadInt16());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error reading name table. Try different encoding.", ex);
                    }
            }
        }

        /// <summary>
        /// Converts <see cref="BA2HeaderType"/> to <see cref="ArchiveTypes"/> for more detailed information.
        /// </summary>
        private ArchiveTypes ConvertType(BA2HeaderType type)
        {
            if (Enum.TryParse("BA2_" + type, out ArchiveTypes typeConverted))
                return typeConverted;
            else
                throw new Exception($"Unable to convert value '{type}' to {nameof(ArchiveTypes)}");
        }
    }
}
