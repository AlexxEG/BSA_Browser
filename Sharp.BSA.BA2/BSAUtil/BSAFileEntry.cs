using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class BSAFileEntry : ArchiveEntry
    {
        public BSAFileEntry(Archive archive, int index)
            : base(archive, index)
        {

        }

        public override void Extract(string destination, bool preserveFolder)
        {
            string path = preserveFolder ? this.FullPath : this.FileName;

            if (!string.IsNullOrEmpty(destination))
                path = Path.Combine(destination, path);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            this.BinaryReader.BaseStream.Position = (long)Offset;

            // Skip ahead
            if ((this.Archive as BSA).ContainsFileNameBlobs)
                this.BinaryReader.BaseStream.Position += this.BinaryReader.ReadByte() + 1;

            using (var fs = File.Create(path))
            {
                if (!Compressed)
                {
                    byte[] bytes = new byte[Size];
                    this.BinaryReader.Read(bytes, 0, (int)Size);
                    fs.Write(bytes, 0, (int)Size);
                }
                else
                {
                    byte[] uncompressed;
                    if (RealSize == 0)
                        uncompressed = new byte[this.BinaryReader.ReadUInt32()];
                    else
                        uncompressed = new byte[RealSize];
                    byte[] compressed = new byte[Size - 4];
                    this.BinaryReader.Read(compressed, 0, (int)(Size - 4));
                    this.Archive.Inflater.Reset();
                    this.Archive.Inflater.SetInput(compressed);
                    this.Archive.Inflater.Inflate(uncompressed);
                    fs.Write(uncompressed, 0, uncompressed.Length);
                }
            }
        }

        public BSAFileEntry Initialize(bool compressed, string folder, uint offset, uint size)
        {
            Compressed = compressed;
            this.FullPath = folder;
            Offset = offset;
            Size = size;
            return this;
        }

        public BSAFileEntry Initialize(string path, uint offset, uint size)
        {
            this.FullPath = path;
            Offset = offset;
            Size = size;
            return this;
        }

        public BSAFileEntry Initialize(string path, uint offset, uint size, uint realSize)
        {
            this.FullPath = path;
            Offset = offset;
            Size = size;
            RealSize = realSize;
            Compressed = realSize != 0;
            return this;
        }
    }
}
