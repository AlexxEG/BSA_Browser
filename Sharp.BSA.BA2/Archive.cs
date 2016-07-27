using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace SharpBSABA2
{
    public abstract class Archive
    {
        public string FullPath { get; private set; }

        public Inflater Inflater { get; private set; } = new Inflater();
        public List<ArchiveEntry> Files { get; private set; } = new List<ArchiveEntry>();

        public BinaryReader BinaryReader { get; private set; }

        public Archive(string filePath)
        {
            this.FullPath = filePath;
            this.BinaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));

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
