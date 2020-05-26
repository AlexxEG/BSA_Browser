using SharpBSABA2.Enums;
using SharpBSABA2.Extensions;
using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public enum BSAFileVersion
    {
        BSA,
        Morrowind,
        Fallout2
    }

    public class BSAFileEntry : ArchiveEntry
    {
        public new BSA Archive => base.Archive as BSA;
        public override uint DisplaySize
        {
            get
            {
                return this.RealSize > 0 ? this.RealSize : this.Size;
            }
        }

        public BSAFileVersion Version { get; private set; }

        public BSAFileEntry(Archive archive, bool compressed, string folder, uint offset, uint size)
            : base(archive)
        {
            this.Version = BSAFileVersion.BSA;

            this.Compressed = compressed;
            this.FullPath = folder;
            this.FullPathOriginal = this.FullPath;
            this.Offset = offset;
            this.Size = size;
        }

        public BSAFileEntry(Archive archive, string path, uint offset, uint size)
            : base(archive)
        {
            this.Version = BSAFileVersion.Morrowind;

            this.FullPath = path;
            this.FullPathOriginal = this.FullPath;
            this.Offset = offset;
            this.Size = size;
        }

        public BSAFileEntry(Archive archive, DAT2FileEntry entry)
            : base(archive)
        {
            this.Version = BSAFileVersion.Fallout2;

            this.FullPath = entry.Filename;
            this.FullPathOriginal = this.FullPath;
            this.Offset = entry.Offset;
            this.Size = entry.PackedSize;
            this.RealSize = entry.RealSize;
            this.Compressed = entry.Compressed;
        }

        public override MemoryStream GetRawDataStream()
        {
            var ms = new MemoryStream();

            this.WriteDataToStream(ms, false);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public override string GetToolTipText()
        {
            return $"{nameof(Version)}: {Version}\n" +
                $"{nameof(FullPath)}: {FullPath}\n" +
                $"{nameof(Offset)}: {Offset}\n" +
                $"{nameof(Size)}: {Size}\n" +
                $"{nameof(RealSize)}: {RealSize}\n" +
                $"{nameof(Compressed)}: {Compressed}";
        }

        protected override void WriteDataToStream(Stream stream)
        {
            this.WriteDataToStream(stream, true);
        }

        protected void WriteDataToStream(Stream stream, bool decompress)
        {
            decompress = decompress && this.Compressed;
            this.BinaryReader.BaseStream.Position = (long)Offset;

            if (this.Archive.Type == ArchiveTypes.BSA_SE)
            {
                // Separate Skyrim Special Edition extraction
                ulong filesz = this.Size & 0x3fffffff;
                if (this.Archive.ContainsFileNameBlobs)
                {
                    int len = this.BinaryReader.ReadByte();
                    filesz -= (ulong)len + 1;
                    this.BinaryReader.BaseStream.Seek((long)this.Offset + 1 + len, SeekOrigin.Begin);
                }

                uint filesize = (uint)filesz;
                if (this.Size > 0 && this.Compressed)
                {
                    filesize = this.BinaryReader.ReadUInt32();
                    filesz -= 4;
                }

                if (!decompress)
                {
                    Archive.WriteSectionToStream(BinaryReader.BaseStream,
                                                 filesz,
                                                 stream,
                                                 bytesWritten => this.BytesWritten = bytesWritten);
                }
                else
                {
                    this.Archive.DecompressLZ4(BinaryReader.BaseStream,
                                               (uint)filesz,
                                               stream,
                                               bytesWritten => this.BytesWritten = bytesWritten);
                }
            }
            else
            {
                // Skip ahead
                if (this.Archive.ContainsFileNameBlobs)
                    this.BinaryReader.BaseStream.Position += this.BinaryReader.ReadByte() + 1;

                if (!decompress)
                {
                    Archive.WriteSectionToStream(BinaryReader.BaseStream,
                                                 this.Size,
                                                 stream,
                                                 bytesWritten => this.BytesWritten = bytesWritten);
                }
                else
                {
                    if (this.Compressed)
                        BinaryReader.ReadUInt32(); // Skip

                    Archive.Decompress(BinaryReader.BaseStream,
                                       this.Size - 4,
                                       stream,
                                       bytesWriten => this.BytesWritten = bytesWriten);
                }
            }
        }
    }
}
