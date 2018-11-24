using System;
using System.IO;
using System.Text;
using SharpBSABA2.Enums;
using SharpBSABA2.Extensions;

namespace SharpBSABA2.BSAUtil
{
    public class BSA : Archive
    {
        public const int MW_HEADER_MAGIC = 0x00000100;  // Magic for Morrowind BSA
        public const int BSA_HEADER_MAGIC = 0x00415342; // Magic for Oblivion BSA, the literal string "BSA\0".

        public const int OB_HEADER_VERSION = 0x67;  // Version number of an Oblivion BSA
        public const int F3_HEADER_VERSION = 0x68;  // Version number of an Fallout 3 BSA
        public const int SSE_HEADER_VERSION = 0x69; // Version number of an Skyrim Special Edition BSA

        public const int OB_BSAARCHIVE_COMPRESSFILES = 0x4;
        public const int F3_BSAARCHIVE_PREFIXFULLFILENAMES = 0x100;

        public bool Compressed
        {
            get
            {
                switch (this.Magic)
                {
                    case BSA_HEADER_MAGIC:
                        return ((Header as BSAHeader).ArchiveFlags & OB_BSAARCHIVE_COMPRESSFILES) > 0;
                    case MW_HEADER_MAGIC:
                    default:
                        return false;
                }
            }
        }
        public bool ContainsFileNameBlobs
        {
            get
            {
                switch (this.Magic)
                {
                    case BSA_HEADER_MAGIC:
                        BSAHeader header = this.Header as BSAHeader;

                        if (header.Version == F3_HEADER_VERSION || header.Version == SSE_HEADER_VERSION)
                            return (header.ArchiveFlags & F3_BSAARCHIVE_PREFIXFULLFILENAMES) > 0;
                        else
                            return false;
                    case MW_HEADER_MAGIC:
                    default:
                        return false;
                }
            }
        }

        public uint Magic { get; private set; }
        public object Header { get; private set; }

        public new bool HasNameTable
        {
            get
            {
                switch (Magic)
                {
                    case BSA_HEADER_MAGIC:
                        BSAHeader header = Header as BSAHeader;
                        return (header.ArchiveFlags & 0x1) > 0 && (header.ArchiveFlags & 0x2) > 0; // Should always be true
                    case MW_HEADER_MAGIC:
                    default:
                        return true;
                }
            }
        }
        public new string VersionString
        {
            get
            {
                switch (this.Magic)
                {
                    case MW_HEADER_MAGIC:
                        return (Header as BSAHeaderMW).Version.ToString("X");
                    case BSA_HEADER_MAGIC:
                        return (Header as BSAHeader).Version.ToString();
                    default:
                        return "None";
                }
            }
        }
        public new ArchiveTypes Type
        {
            get
            {
                switch (this.Magic)
                {
                    case MW_HEADER_MAGIC:
                        return ArchiveTypes.BSA_MW;
                    case BSA_HEADER_MAGIC:
                        return (Header as BSAHeader).Version == SSE_HEADER_VERSION ? ArchiveTypes.BSA_SE : ArchiveTypes.BSA;
                    default:
                        return ArchiveTypes.DAT_F2;
                }
            }
        }

        public BSA(string filePath) : base(filePath)
        {
        }

        protected override void Open(string filePath)
        {
            try
            {
                this.Magic = this.BinaryReader.ReadUInt32();

                if (this.Magic == MW_HEADER_MAGIC) // Morrowind uses this as version
                {
                    var header = new BSAHeaderMW(this.BinaryReader, this.Magic);

                    this.Header = header;
                    this.FileCount = (int)header.FileCount;

                    uint dataoffset = 12 + header.HashOffset + header.FileCount * 8;
                    uint fnameOffset1 = 12 + header.FileCount * 8;
                    uint fnameOffset2 = 12 + header.FileCount * 12;

                    for (int i = 0; i < header.FileCount; i++)
                    {
                        this.BinaryReader.BaseStream.Position = 12 + i * 8;
                        uint size = this.BinaryReader.ReadUInt32();
                        uint offset = this.BinaryReader.ReadUInt32() + dataoffset;
                        this.BinaryReader.BaseStream.Position = fnameOffset1 + i * 4;
                        this.BinaryReader.BaseStream.Position = this.BinaryReader.ReadInt32() + fnameOffset2;

                        string name = this.BinaryReader.ReadStringTo('\0');

                        this.Files.Add(new BSAFileEntry(this, name, offset, size));
                    }
                }
                else if (this.Magic == BSA_HEADER_MAGIC)
                {
                    var header = new BSAHeader(this.BinaryReader);

                    this.Header = header;
                    this.FileCount = (int)header.FileCount;

                    int[] numfiles = new int[header.FolderCount];
                    for (int i = 0; i < header.FolderCount; i++)
                    {
                        // Skip hash
                        this.BinaryReader.BaseStream.Position += 8;
                        // Read fileCount
                        numfiles[i] = this.BinaryReader.ReadInt32();
                        // Skip Unk1 + offset (4 + 8 bytes) if SSE. Otherwise only offset (4 bytes)
                        this.BinaryReader.BaseStream.Position += header.Version == SSE_HEADER_VERSION ? 12 : 4;
                    }

                    for (int i = 0; i < header.FolderCount; i++)
                    {
                        string folder = BinaryReader.ReadString(BinaryReader.ReadByte() - 1);
                        this.BinaryReader.BaseStream.Position++;

                        for (int j = 0; j < numfiles[i]; j++)
                        {
                            // Skip hash
                            this.BinaryReader.BaseStream.Position += 8;
                            uint size = this.BinaryReader.ReadUInt32();
                            uint offset = this.BinaryReader.ReadUInt32();
                            bool comp = this.Compressed;

                            if ((size & (1 << 30)) != 0)
                            {
                                comp = !comp;
                                size ^= 1 << 30;
                            }

                            this.Files.Add(new BSAFileEntry(this, comp, folder, offset, size));
                        }
                    }

                    // Read name table
                    for (int i = 0; i < header.FileCount; i++)
                    {
                        this.Files[i].FullPath = Path.Combine(
                            this.Files[i].FullPath,
                            this.BinaryReader.ReadStringTo('\0'));
                    }
                }
                else
                {
                    // Assume it's a Fallout 2 DAT
                    this.BinaryReader.BaseStream.Position = this.BinaryReader.BaseStream.Length - 8;
                    uint treeSize = this.BinaryReader.ReadUInt32();
                    uint dataSize = this.BinaryReader.ReadUInt32();

                    if (dataSize != this.BinaryReader.BaseStream.Length)
                    {
                        this.BinaryReader.Close();
                        throw new ArgumentException("File is not a valid bsa archive.", nameof(filePath));
                    }

                    this.BinaryReader.BaseStream.Position = dataSize - treeSize - 8;

                    this.FileCount = this.BinaryReader.ReadInt32();

                    for (int i = 0; i < this.FileCount; i++)
                    {
                        var entry = new DAT2FileEntry(this.BinaryReader);

                        this.Files.Add(new BSAFileEntry(this, entry));
                    }
                }
            }
            catch (Exception ex)
            {
                this.BinaryReader?.Close();

                throw new Exception("An error occured trying to open the archive.", ex);
            }
        }

        public static bool IsSupportedVersion(string filePath)
        {
            using (var br = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                uint type = br.ReadUInt32();
                int version = br.ReadInt32();

                if (type != BSA_HEADER_MAGIC)
                    return true; // Only Oblivion/Fallout BSAs needs this version check,
                                 // so if it's neither just always return true.

                if (version != OB_HEADER_VERSION &&
                    version != F3_HEADER_VERSION &&
                    version != SSE_HEADER_VERSION)
                    return false;
            }

            return true;
        }
    }
}
