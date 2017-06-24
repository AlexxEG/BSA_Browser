using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpBSABA2.Extensions;

namespace SharpBSABA2.BSAUtil
{
    public class BSA : Archive
    {
        public const int MW_BSAHEADER_FILEID = 0x00000100; //!< Magic for Morrowind BSA
        public const int OB_BSAHEADER_FILEID = 0x00415342; //!< Magic for Oblivion BSA, the literal string "BSA\0".

        public const int OB_BSAHEADER_VERSION = 0x67; //!< Version number of an Oblivion BSA
        public const int F3_BSAHEADER_VERSION = 0x68; //!< Version number of a Fallout 3 BSA
        public const int SSE_BSAHEADER_VERSION = 0x69; //!< Version number of a Skyrim Special Edition BSA

        public const int OB_BSAARCHIVE_COMPRESSFILES = 0x0004;
        public const int F3_BSAARCHIVE_PREFIXFULLFILENAMES = 0x0100;

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
                //if(Program.ReadCString(br)!="BSA") throw new fommException("File was not a valid BSA archive");
                uint type = this.BinaryReader.ReadUInt32();
                if (type != 0x00415342 && type != 0x00000100)
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

                        this.Files.Add(new BSAFileEntry(this, i)
                            .Initialize(path, offset, compSize, comp == 0 ? 0 : realSize));
                    }
                }
                else if (type == 0x0100)
                {
                    uint hashoffset = this.BinaryReader.ReadUInt32();
                    uint FileCount = this.BinaryReader.ReadUInt32();

                    uint dataoffset = 12 + hashoffset + FileCount * 8;
                    uint fnameOffset1 = 12 + FileCount * 8;
                    uint fnameOffset2 = 12 + FileCount * 12;

                    for (int i = 0; i < FileCount; i++)
                    {
                        this.BinaryReader.BaseStream.Position = 12 + i * 8;
                        uint size = this.BinaryReader.ReadUInt32();
                        uint offset = this.BinaryReader.ReadUInt32() + dataoffset;
                        this.BinaryReader.BaseStream.Position = fnameOffset1 + i * 4;
                        this.BinaryReader.BaseStream.Position = this.BinaryReader.ReadInt32() + fnameOffset2;

                        string name = this.ReadStringTo(this.BinaryReader, '\0');

                        this.Files.Add(new BSAFileEntry(this, i).Initialize(name, offset, size));
                    }
                }
                else
                {
                    int version = this.BinaryReader.ReadInt32();
                    this.Version = version;

                    if (version == 0x69)
                    {
                        // ToDo: Merge these two methods together, with version checks instead.
                        // This is just a lazy lazy implementation for now.
                        this.OpenSSE();
                        return;
                    }

                    this.BinaryReader.BaseStream.Position += 4;
                    uint flags = this.BinaryReader.ReadUInt32();
                    this.Compressed = ((flags & 0x004) > 0);
                    this.ContainsFileNameBlobs = ((flags & 0x100) > 0 && version == 0x68);
                    int FolderCount = this.BinaryReader.ReadInt32();
                    int FileCount = this.BinaryReader.ReadInt32();
                    this.BinaryReader.BaseStream.Position += 12;
                    int[] numfiles = new int[FolderCount];
                    this.BinaryReader.BaseStream.Position += 8;

                    for (int i = 0; i < FolderCount; i++)
                    {
                        // Read fileCount
                        numfiles[i] = this.BinaryReader.ReadInt32();
                        // Skip offset, then hash of the next entry
                        this.BinaryReader.BaseStream.Position += 12;
                    }

                    this.BinaryReader.BaseStream.Position -= 8;

                    for (int i = 0; i < FolderCount; i++)
                    {
                        int k = this.BinaryReader.ReadByte();
                        string folder = this.BinaryReader.ReadString(k - 1);
                        this.BinaryReader.BaseStream.Position++;

                        for (int j = 0; j < numfiles[i]; j++)
                        {
                            this.BinaryReader.BaseStream.Position += 8;
                            uint size = this.BinaryReader.ReadUInt32();
                            bool comp = this.Compressed;

                            if ((size & (1 << 30)) != 0)
                            {
                                comp = !comp;
                                size ^= 1 << 30;
                            }
                            this.Files.Add(new BSAFileEntry(this, i)
                                .Initialize(comp, folder, this.BinaryReader.ReadUInt32(), size));
                        }
                    }

                    for (int i = 0; i < FileCount; i++)
                    {
                        string name = this.ReadStringTo(this.BinaryReader, '\0');
                        this.Files[i].FullPath = Path.Combine(this.Files[i].FullPath, name);
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
            // magic and version has been read, at position 8

            // read header
            BSAHeader header = BSAHeader.ReadFrom(this.BinaryReader);

            uint numFiles = header.FileCount;

            this.Compressed = (header.ArchiveFlags & OB_BSAARCHIVE_COMPRESSFILES) > 0;
            this.ContainsFileNameBlobs = (header.ArchiveFlags & F3_BSAARCHIVE_PREFIXFULLFILENAMES) > 0;

            // Seek to start of Folders, then skip to filename table
            this.BinaryReader.BaseStream.Seek(header.FolderRecordOffset
                    + header.FolderNameLength
                    + header.FolderCount * (1 + BSAFolderInfo.SizeOf(Version))
                    + header.FileCount * BSAFileInfo.SizeOf(),
                SeekOrigin.Begin);

            List<string> fileNames = new List<string>();
            for (int i = 0; i < header.FileCount; i++)
            {
                fileNames.Add(this.ReadStringTo(this.BinaryReader, '\0'));
            }

            this.BinaryReader.BaseStream.Seek(header.FolderRecordOffset, SeekOrigin.Begin);

            var folderInfos = new List<BSAFolderInfo>();
            for (int i = 0; i < header.FolderCount; i++)
            {
                var fi = BSAFolderInfo.ReadFrom(this.BinaryReader, this.Version);
                folderInfos.Add(fi);
            }

            int filenameIndex = 0;

            for (int i = 0; i < folderInfos.Count; i++)
            {
                int len = this.BinaryReader.ReadByte();
                folderInfos[i].FolderName = this.BinaryReader.ReadString(len - 1);
                this.BinaryReader.ReadChar(); // Skip '\0' char

                List<BSAFileInfo> fileInfos = new List<BSAFileInfo>();
                while (fileInfos.Count < folderInfos[i].FileCount)
                {
                    var fi = BSAFileInfo.ReadFrom(this.BinaryReader);
                    fi.NamePrefix = this.ContainsFileNameBlobs;
                    fi.Compressed = this.Compressed;
                    fileInfos.Add(fi);
                }

                for (int j = 0; j < fileInfos.Count; j++)
                {
                    fileInfos[j].Name = fileNames[filenameIndex];
                    filenameIndex++;
                    var fe = new BSAFileEntry(this, i)
                        .Initialize(this.Compressed,
                            folderInfos[i].FolderName,
                            fileInfos[j].Offset,
                            fileInfos[j].SizeFlags);
                    fe.FullPath = Path.Combine(fe.FullPath, fileInfos[j].Name);
                    fe.fileInfo = fileInfos[j];
                    this.Files.Add(fe);
                }

                folderInfos[i].Files.AddRange(fileInfos);
            }
        }

        public static bool IsSupportedVersion(string filePath)
        {
            using (var br = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                uint type = br.ReadUInt32();
                int version = br.ReadInt32();

                if (type != OB_BSAHEADER_FILEID)
                    return true; // Only Oblivion/Fallout BSAs needs this version check,
                                 // so if it's neither just always return true.

                if (version != 0x67 && version != 0x68 && version != 0x69)
                    return false;
            }

            return true;
        }
    }
}
