using System.IO;
using SharpBSABA2.Utils;

namespace SharpBSABA2.BA2Util
{
    public class BA2FileEntry : ArchiveEntry
    {
        public uint flags { get; set; }
        public uint align { get; set; }

        public override bool Compressed
        {
            get { return this.Size != 0; }
        }
        public override uint DisplaySize
        {
            get
            {
                return this.RealSize;
            }
        }

        public override ulong GetSizeInArchive(SharedExtractParams extractParams) => this.Compressed ? this.Size : this.RealSize;

        public BA2FileEntry(Archive ba2) : base(ba2)
        {
            nameHash = ba2.BinaryReader.ReadUInt32();
            Extension = new string(ba2.BinaryReader.ReadChars(4));
            dirHash = ba2.BinaryReader.ReadUInt32();

            FullPath = dirHash > 0 ? $"{dirHash:X}_" : string.Empty;
            FullPath += $"{nameHash:X}.{Extension.TrimEnd('\0')}";
            FullPathOriginal = FullPath;

            flags = ba2.BinaryReader.ReadUInt32();
            Offset = ba2.BinaryReader.ReadUInt64();
            Size = ba2.BinaryReader.ReadUInt32();
            RealSize = ba2.BinaryReader.ReadUInt32();
            align = ba2.BinaryReader.ReadUInt32();
        }

        public override string GetToolTipText()
        {
            return $"Name hash:\t {nameHash:X}\n" +
                $"Directory hash:\t {dirHash:X}\n" +
                $"Flags:\t\t {flags:X}\n" +
                $"Offset:\t\t {Offset}\n" +
                $"Size:\t\t {Size}\n" +
                $"Real Size:\t {RealSize}\n" +
                $"Align:\t\t {align:X}";
        }

        protected override void WriteDataToStream(Stream stream, SharedExtractParams extractParams, bool decompress)
        {
            var reader = extractParams.Reader;
            uint len = this.Compressed ? this.Size : this.RealSize;
            reader.BaseStream.Seek((long)this.Offset, SeekOrigin.Begin);
            // Reset at start since value might still be in used for a bit after
            this.BytesWritten = 0;

            if (!decompress || !this.Compressed)
            {
                StreamUtils.WriteSectionToStream(reader.BaseStream,
                    len,
                    stream,
                    bytesWritten => this.BytesWritten = bytesWritten);
            }
            else
            {
                CompressionUtils.Decompress(reader.BaseStream,
                    len,
                    stream,
                    bytesWritten => this.BytesWritten = bytesWritten,
                    extractParams);
            }
        }
    }
}
