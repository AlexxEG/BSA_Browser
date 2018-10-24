using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using SharpBSABA2.Enums;

namespace SharpBSABA2
{
    public abstract class Archive
    {
        public bool RetrieveRealSize { get; set; }

        public string FullPath { get; private set; }
        public string FileName => Path.GetFileName(this.FullPath);

        public ArchiveTypes Type { get; protected set; }

        public Inflater Inflater { get; private set; } = new Inflater();
        public List<ArchiveEntry> Files { get; private set; } = new List<ArchiveEntry>();

        public BinaryReader BinaryReader { get; private set; }

        public Archive(string filePath)
        {
            this.FullPath = filePath;
            this.BinaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read), Encoding.Default);

            this.Open(filePath);
        }

        public void Close()
        {
            this.BinaryReader?.Close();
        }

        public void Extract(string filePath, string destination, bool preserveFolder)
        {
            this.FindFile(filePath).Extract(destination, preserveFolder);
        }

        public ArchiveEntry FindFile(string fullpath)
        {
            return this.Files.Find(x => x.FullPath == fullpath);
        }

        public ArchiveEntry[] FindFiles(params string[] files)
        {
            return this.Files.FindAll(x => System.Array.IndexOf(files, x.FullPath) >= 0).ToArray();
        }

        protected abstract void Open(string filePath);
    }
}
