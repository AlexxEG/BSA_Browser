using ICSharpCode.SharpZipLib.Zip.Compression;
using System.IO;

namespace BSA_Browser
{
    public class BSAFileEntry
    {
        private static readonly Inflater inf = new Inflater();
        internal readonly bool Compressed;
        private string fileName;
        private string lowername;
        internal string FileName
        {
            get { return fileName; }
            set
            {
                if (value == null) return;
                fileName = value;
                //lowername=Folder.ToLower()+"\\"+fileName.ToLower();
                lowername = Path.Combine(Folder.ToLower(), fileName.ToLower());
            }
        }
        internal string FullPath
        {
            get
            {
                return Path.Combine(Folder, FileName);
            }
        }
        internal string LowerName
        {
            get { return lowername; }
        }
        internal readonly string Folder;
        internal readonly uint Offset;
        internal readonly uint Size;
        internal readonly uint RealSize;

        internal BSAFileEntry(bool compressed, string folder, uint offset, uint size)
        {
            Compressed = compressed;
            Folder = folder;
            Offset = offset;
            Size = size;
        }

        internal BSAFileEntry(string path, uint offset, uint size)
        {
            Folder = Path.GetDirectoryName(path);
            FileName = Path.GetFileName(path);
            Offset = offset;
            Size = size;
        }

        internal BSAFileEntry(string path, uint offset, uint size, uint realSize)
        {
            Folder = Path.GetDirectoryName(path);
            FileName = Path.GetFileName(path);
            Offset = offset;
            Size = size;
            RealSize = realSize;
            Compressed = realSize != 0;
        }

        internal void Extract(string path, bool UseFolderPath, BinaryReader br, bool SkipName)
        {
            if (UseFolderPath)
                path += @"\" + Folder + @"\" + FileName;

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            FileStream fs = File.Create(path);
            br.BaseStream.Position = Offset;

            if (SkipName)
                br.BaseStream.Position += br.ReadByte() + 1;

            if (!Compressed)
            {
                byte[] bytes = new byte[Size];
                br.Read(bytes, 0, (int)Size);
                fs.Write(bytes, 0, (int)Size);
            }
            else
            {
                byte[] uncompressed;
                if (RealSize == 0) uncompressed = new byte[br.ReadUInt32()];
                else uncompressed = new byte[RealSize];
                byte[] compressed = new byte[Size - 4];
                br.Read(compressed, 0, (int)(Size - 4));
                inf.Reset();
                inf.SetInput(compressed);
                inf.Inflate(uncompressed);
                fs.Write(uncompressed, 0, uncompressed.Length);
            }
            fs.Close();
        }
    }
}
