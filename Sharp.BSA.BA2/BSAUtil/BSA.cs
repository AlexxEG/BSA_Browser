using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public bool Compressed { get; private set; }
        public bool ContainsFileNameBlobs { get; private set; }

        public int Version { get; set; }

        public BSA(string filePath) : base(filePath)
        {
        }

        protected override void Open(string filePath)
        {
            try
            {
                uint magic = this.BinaryReader.ReadUInt32();

                if (magic == MW_HEADER_MAGIC) // Morrowind uses this as version
                {

                    var header = new BSAHeaderMW(this.BinaryReader, magic);

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

                        string name = this.ReadStringTo(this.BinaryReader, '\0');

                        this.Files.Add(new BSAFileEntry(this, i, name, offset, size));
                    }
                }
                else if (magic == BSA_HEADER_MAGIC)
                {
                    this.Version = this.BinaryReader.ReadInt32();

                    if (this.Version == SSE_HEADER_VERSION)
                    {
                        // ToDo: Merge these two methods together, with version checks instead.
                        // This is just a lazy lazy implementation for now.
                        this.OpenSSE();
                        return;
                    }

                    var header = new BSAHeader(this.BinaryReader);

                    int[] numfiles = new int[header.FolderCount];
                    this.Compressed = ((header.ArchiveFlags & OB_BSAARCHIVE_COMPRESSFILES) > 0);
                    this.ContainsFileNameBlobs = ((header.ArchiveFlags & F3_BSAARCHIVE_PREFIXFULLFILENAMES) > 0 && this.Version == F3_HEADER_VERSION);

                    for (int i = 0; i < header.FolderCount; i++)
                    {
                        // Skip hash
                        this.BinaryReader.BaseStream.Position += 8;
                        // Read fileCount
                        numfiles[i] = this.BinaryReader.ReadInt32();
                        // Skip offset
                        this.BinaryReader.BaseStream.Position += 4;
                    }

                    for (int i = 0; i < header.FolderCount; i++)
                    {
                        int k = this.BinaryReader.ReadByte();
                        string folder = this.BinaryReader.ReadString(k - 1);
                        this.BinaryReader.BaseStream.Position++;

                        for (int j = 0; j < numfiles[i]; j++)
                        {
                            // Skip hash
                            this.BinaryReader.BaseStream.Position += 8;
                            uint size = this.BinaryReader.ReadUInt32();
                            bool comp = this.Compressed;

                            if ((size & (1 << 30)) != 0)
                            {
                                comp = !comp;
                                size ^= 1 << 30;
                            }
                            this.Files.Add(new BSAFileEntry(this, i,
                                comp, folder, this.BinaryReader.ReadUInt32(), size));
                        }
                    }

                    for (int i = 0; i < header.FileCount; i++)
                    {
                        string name = this.ReadStringTo(this.BinaryReader, '\0');
                        this.Files[i].FullPath = Path.Combine(this.Files[i].FullPath, name);
                    }
                }
                else
                {
                    //Might be a fallout 2 dat
                    this.BinaryReader.BaseStream.Position = this.BinaryReader.BaseStream.Length - 8;
                    uint TreeSize = this.BinaryReader.ReadUInt32();
                    uint DataSize = this.BinaryReader.ReadUInt32();

                    if (DataSize != this.BinaryReader.BaseStream.Length)
                    {
                        this.BinaryReader.Close();
                        throw new ArgumentException("File is not a valid bsa archive.", nameof(filePath));
                    }

                    this.BinaryReader.BaseStream.Position = DataSize - TreeSize - 8;
                    int FileCount = this.BinaryReader.ReadInt32();

                    for (int i = 0; i < FileCount; i++)
                    {
                        int fileLen = this.BinaryReader.ReadInt32();
                        string path = this.BinaryReader.ReadString(fileLen);

                        byte comp = this.BinaryReader.ReadByte();
                        uint realSize = this.BinaryReader.ReadUInt32();
                        uint compSize = this.BinaryReader.ReadUInt32();
                        uint offset = this.BinaryReader.ReadUInt32();

                        if (path.StartsWith("\\"))
                            path.Remove(0, 1);

                        this.Files.Add(new BSAFileEntry(this, i,
                            path, offset, compSize, comp == 0 ? 0 : realSize));
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.BinaryReader != null)
                    this.BinaryReader.Close();

                throw new Exception("An error occured trying to open the archive.", ex);
            }
        }

        private string ReadStringTo(BinaryReader reader, char endChar)
        {
            var sb = new StringBuilder();
            char c;
            while ((c = reader.ReadChar()) != endChar)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        private void OpenSSE()
        {
            // Read header
            BSAHeader header = new BSAHeader(this.BinaryReader);

            this.Compressed = (header.ArchiveFlags & OB_BSAARCHIVE_COMPRESSFILES) > 0;
            this.ContainsFileNameBlobs = (header.ArchiveFlags & F3_BSAARCHIVE_PREFIXFULLFILENAMES) > 0;

            var folderInfos = new List<BSAFolderInfo>();
            for (int i = 0; i < header.FolderCount; i++)
            {
                folderInfos.Add(new BSAFolderInfo(this.BinaryReader, this.Version));
            }

            for (int i = 0; i < folderInfos.Count; i++)
            {
                int len = this.BinaryReader.ReadByte();
                folderInfos[i].FolderName = this.BinaryReader.ReadString(len - 1);
                this.BinaryReader.ReadChar(); // Skip '\0' char

                var fileInfos = new List<BSAFileInfo>();
                while (fileInfos.Count < folderInfos[i].FileCount)
                {
                    fileInfos.Add(new BSAFileInfo(this.BinaryReader));
                }

                for (int j = 0; j < fileInfos.Count; j++)
                {
                    this.Files.Add(new BSAFileEntry(this, i,
                        this.Compressed,
                        folderInfos[i].FolderName,
                        fileInfos[j].Offset,
                        fileInfos[j].SizeFlags));
                }

                folderInfos[i].Files.AddRange(fileInfos);
            }

            for (int i = 0; i < header.FileCount; i++)
            {
                this.Files[i].FullPath = Path.Combine(
                    this.Files[i].FullPath,
                    this.ReadStringTo(this.BinaryReader, '\0'));
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
