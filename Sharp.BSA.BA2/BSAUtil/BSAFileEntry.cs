using SharpBSABA2.Enums;
using SharpBSABA2.Utils;
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
        public ulong mwNameHash;

        public new BSA Archive => base.Archive as BSA;
        public override uint DisplaySize
        {
            get
            {
                return this.RealSize > 0 ? this.RealSize : this.Size;
            }
        }

        public BSAFileVersion Version { get; private set; }

        public override ulong GetSizeInArchive(SharedExtractParams extractParams)
        {
            if (this.Archive.Type == ArchiveTypes.BSA_SE)
            {
                var reader = extractParams.Reader;
                ulong filesz = this.Size & 0x3fffffff;
                reader.BaseStream.Position = (long)Offset;

                if (this.Archive.ContainsFileNameBlobs)
                {
                    int len = reader.ReadByte();
                    filesz -= (ulong)len + 1;
                }

                if (this.Size > 0 && this.Compressed)
                {
                    filesz -= 4;
                }

                return filesz;
            }
            else
            {
                return this.Size;
            }
        }

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

        public override string GetToolTipText()
        {
            return $"Version:\t\t {Version}\n" +
                $"Offset:\t\t {Offset}\n" +
                $"Size:\t\t {Size}\n" +
                $"Real Size:\t {RealSize}\n" +
                $"Compressed:\t {Compressed}";
        }

        protected override void WriteDataToStream(Stream stream, SharedExtractParams extractParams, bool decompress)
        {
            var reader = extractParams.Reader;
            decompress = decompress && this.Compressed;
            reader.BaseStream.Position = (long)Offset;
            // Reset at start since value might still be in used for a bit after
            this.BytesWritten = 0;

            if (this.Archive.Type == ArchiveTypes.BSA_SE)
            {
                // Separate Skyrim Special Edition extraction
                ulong filesz = this.Size & 0x3fffffff;
                if (this.Archive.ContainsFileNameBlobs)
                {
                    int len = reader.ReadByte();
                    filesz -= (ulong)len + 1;
                    reader.BaseStream.Seek((long)this.Offset + 1 + len, SeekOrigin.Begin);
                }

                uint filesize = (uint)filesz;
                if (this.Size > 0 && this.Compressed)
                {
                    filesize = reader.ReadUInt32();
                    filesz -= 4;
                }

                if (!decompress)
                {
                    StreamUtils.WriteSectionToStream(reader.BaseStream,
                        filesz,
                        stream,
                        bytesWritten => this.BytesWritten = bytesWritten);
                }
                else
                {
                    CompressionUtils.DecompressLZ4(reader.BaseStream,
                        (uint)filesz,
                        stream,
                        bytesWritten => this.BytesWritten = bytesWritten);
                }
            }
            else
            {
                // Skip ahead
                if (this.Archive.ContainsFileNameBlobs)
                    reader.BaseStream.Position += reader.ReadByte() + 1;

                if (!decompress)
                {
                    StreamUtils.WriteSectionToStream(reader.BaseStream,
                        this.Size,
                        stream,
                        bytesWritten => this.BytesWritten = bytesWritten);
                }
                else
                {
                    if (this.Compressed)
                        reader.ReadUInt32(); // Skip

                    CompressionUtils.Decompress(reader.BaseStream,
                        this.Size - 4,
                        stream,
                        bytesWriten => this.BytesWritten = bytesWriten,
                        extractParams);
                }
            }
        }
    }
}
