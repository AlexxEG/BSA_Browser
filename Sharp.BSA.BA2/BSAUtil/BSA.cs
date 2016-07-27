using System;
using System.IO;
using System.Text;

namespace SharpBSABA2.BSAUtil
{
    public class BSA : Archive
    {
        public const int MW_BSAHEADER_FILEID = 0x00000100; //!< Magic for Morrowind BSA
        public const int OB_BSAHEADER_FILEID = 0x00415342; //!< Magic for Oblivion BSA, the literal string "BSA\0".

        public const int OB_BSAHEADER_VERSION = 0x67; //!< Version number of an Oblivion BSA
        public const int F3_BSAHEADER_VERSION = 0x68; //!< Version number of a Fallout 3 BSA

        public bool Compressed { get; private set; }
        public bool ContainsFileNameBlobs { get; private set; }

        public BSA(string filePath) : base(filePath)
        {
        }

        protected override void Open(string filePath)
        {
            try
            {
                //if(Program.ReadCString(br)!="BSA") throw new fommException("File was not a valid BSA archive");
                uint type = this.BinaryReader.ReadUInt32();
                StringBuilder sb = new StringBuilder(64);
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

                        for (int j = 0; j < fileLen; j++)
                            sb.Append(this.BinaryReader.ReadChar());

                        byte comp = this.BinaryReader.ReadByte();
                        uint realSize = this.BinaryReader.ReadUInt32();
                        uint compSize = this.BinaryReader.ReadUInt32();
                        uint offset = this.BinaryReader.ReadUInt32();

                        if (sb[0] == '\\')
                            sb.Remove(0, 1);

                        this.Files.Add(new BSAFileEntry(this, i)
                            .Initialize(sb.ToString(), offset, compSize, comp == 0 ? 0 : realSize));
                        sb.Length = 0;
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

                        sb.Length = 0;

                        while (true)
                        {
                            char b = this.BinaryReader.ReadChar();
                            if (b == '\0') break;
                            sb.Append(b);
                        }

                        this.Files.Add(new BSAFileEntry(this, i).Initialize(sb.ToString(), offset, size));
                    }
                }
                else
                {
                    int version = this.BinaryReader.ReadInt32();
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
                        numfiles[i] = this.BinaryReader.ReadInt32();
                        this.BinaryReader.BaseStream.Position += 12;
                    }

                    this.BinaryReader.BaseStream.Position -= 8;

                    for (int i = 0; i < FolderCount; i++)
                    {
                        int k = this.BinaryReader.ReadByte();
                        while (--k > 0) sb.Append(this.BinaryReader.ReadChar());
                        this.BinaryReader.BaseStream.Position++;
                        string folder = sb.ToString();

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
                        sb.Length = 0;
                    }

                    for (int i = 0; i < FileCount; i++)
                    {
                        while (true)
                        {
                            char c = this.BinaryReader.ReadChar();

                            if (c == '\0')
                                break;

                            sb.Append(c);
                        }
                        this.Files[i].FullPath = Path.Combine(this.Files[i].FullPath, sb.ToString());
                        sb.Length = 0;
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

        public static bool IsSupportedVersion(string filePath)
        {
            using (var br = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                uint type = br.ReadUInt32();
                int version = br.ReadInt32();

                if (type != OB_BSAHEADER_FILEID)
                    return true; // Only Oblivion/Fallout BSAs needs this version check,
                                 // so if it's neither just always return true.

                if (version != 0x67 && version != 0x68)
                    return false;
            }

            return true;
        }
    }
}
