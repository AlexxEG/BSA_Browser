using SharpBSABA2.Enums;
using SharpBSABA2.Extensions;
using System;
using System.IO;
using System.Text;

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

        private bool hasNameTableMW = true;
        public override bool HasNameTable
        {
            get
            {
                switch (Magic)
                {
                    case BSA_HEADER_MAGIC:
                        BSAHeader header = Header as BSAHeader;
                        return (header.ArchiveFlags & 0x1) > 0 && (header.ArchiveFlags & 0x2) > 0; // Should always be true
                    case MW_HEADER_MAGIC:
                        return hasNameTableMW;
                    default:
                        return true;
                }
            }
        }
        public override string VersionString
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
        public override ArchiveTypes Type
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

        public BSA(string filePath) : base(filePath) { }
        public BSA(string filePath, Encoding encoding) : base(filePath, encoding) { }
        public BSA(string filePath, Encoding encoding, bool retrieveRealSize) : base(filePath, encoding, retrieveRealSize) { }

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
                    uint dataOffset = 12 + header.HashOffset + header.FileCount * 8;

                    // Store file sizes and offsets
                    for (int i = 0; i < header.FileCount; i++)
                    {
                        uint size = this.BinaryReader.ReadUInt32();
                        uint offset = this.BinaryReader.ReadUInt32() + dataOffset;

                        this.Files.Add(new BSAFileEntry(this, offset.ToString(), offset, size));
                        this.Files[i].Index = i;
                    }

                    // Check if archive has name offset and name table, for example for Xbox
                    this.BinaryReader.BaseStream.Position = 12 + header.FileCount * 8; // Skip header and entries

                    // First name offset should be 0 if there is one, otherwise it doesn't have one
                    hasNameTableMW = this.BinaryReader.ReadUInt32() == 0;

                    if (hasNameTableMW)
                        this.BinaryReader.BaseStream.Position = 12 + header.FileCount * 12; // Seek to name table
                    else
                        this.BinaryReader.BaseStream.Position -= 4; // Go Back

                    for (int i = 0; i < header.FileCount; i++)
                    {
                        if (hasNameTableMW)
                        {
                            this.Files[i].FullPath = this.BinaryReader.ReadStringTo('\0');
                            this.Files[i].FullPathOriginal = this.Files[i].FullPath;
                        }
                        else
                        {
                            this.Files[i].FullPath = string.Format("{0:X}", this.BinaryReader.ReadUInt64());
                            this.Files[i].FullPathOriginal = this.Files[i].FullPath;
                        }
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
                            this.Files[j].Index = j;
                        }
                    }

                    // Grab the uncompressed file size before each data block
                    if (this.RetrieveRealSize)
                    {
                        // Save the position so we can go back after to the name table
                        long pos = this.BinaryReader.BaseStream.Position;

                        for (int i = 0; i < header.FileCount; i++)
                        {
                            var entry = this.Files[i] as BSAFileEntry;

                            if (!entry.Compressed)
                                continue;

                            this.BinaryReader.BaseStream.Position = (long)entry.Offset;

                            if (this.ContainsFileNameBlobs)
                                this.BinaryReader.BaseStream.Position += this.BinaryReader.ReadByte() + 1;

                            entry.RealSize = this.BinaryReader.ReadUInt32();
                        }

                        this.BinaryReader.BaseStream.Position = pos;
                    }

                    // Read name table
                    for (int i = 0; i < header.FileCount; i++)
                    {
                        this.Files[i].FullPath = Path.Combine(
                            this.Files[i].FullPath,
                            this.BinaryReader.ReadStringTo('\0'));
                        this.Files[i].FullPathOriginal = this.Files[i].FullPath;
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
                        this.Files[i].Index = i;
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
