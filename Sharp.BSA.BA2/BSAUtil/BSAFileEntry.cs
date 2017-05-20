using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class BSAFileEntry : ArchiveEntry
    {
        public override uint DisplaySize
        {
            get
            {
                // ToDo: Incorrect with compressed files. RealSize isn't set until Extract is called
                return this.Size;
            }
        }

        public BSAFileInfo fileInfo;

        public BSAFileEntry(Archive archive, int index)
            : base(archive, index)
        {

        }

        public override void Extract(string destination, bool preserveFolder)
        {
            this.Extract(destination, preserveFolder, this.FileName);
        }

        public override void Extract(string destination, bool preserveFolder, string newName)
        {
            string path = preserveFolder ? this.Folder : string.Empty;

            path = Path.Combine(path, newName);

            if (!string.IsNullOrEmpty(destination))
                path = Path.Combine(destination, path);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            if ((this.Archive as BSA).Version == BSA.SSE_BSAHEADER_VERSION)
            {
                // Separate Skyrim Special Edition extraction
                this.BinaryReader.BaseStream.Seek(fileInfo.Offset, SeekOrigin.Begin);
                ulong filesz = fileInfo.SizeFlags & 0x3fffffff;
                if (fileInfo.NamePrefix)
                {
                    int len = this.BinaryReader.ReadByte();
                    filesz -= (ulong)len + 1;
                    this.BinaryReader.BaseStream.Seek(fileInfo.Offset + 1 + len, SeekOrigin.Begin);
                }

                uint filesize = (uint)filesz;
                if (fileInfo.SizeFlags > 0 && fileInfo.Compressed)
                {
                    filesize = this.BinaryReader.ReadUInt32();
                    filesz -= 4;
                }

                byte[] content = this.BinaryReader.ReadBytes((int)filesz);

                using (var fs = File.Create(path))
                {
                    if (fileInfo.Compressed == false)
                    {
                        fs.Write(content, 0, content.Length);
                    }
                    else
                    {
                        using (var ms = new MemoryStream(content, false))
                        using (var stream = lz4.LZ4Stream.CreateDecompressor(ms, lz4.LZ4StreamMode.Read))
                        {
                            stream.CopyTo(fs);
                        }
                    }
                }
            }
            else
            {
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
        }

        public override MemoryStream GetDataStream()
        {
            var result_ms = new MemoryStream();

            if ((this.Archive as BSA).Version == BSA.SSE_BSAHEADER_VERSION)
            {
                // Separate Skyrim Special Edition extraction
                this.BinaryReader.BaseStream.Seek(fileInfo.Offset, SeekOrigin.Begin);
                ulong filesz = fileInfo.SizeFlags & 0x3fffffff;
                if (fileInfo.NamePrefix)
                {
                    int len = this.BinaryReader.ReadByte();
                    filesz -= (ulong)len + 1;
                    this.BinaryReader.BaseStream.Seek(fileInfo.Offset + 1 + len, SeekOrigin.Begin);
                }

                uint filesize = (uint)filesz;
                if (fileInfo.SizeFlags > 0 && fileInfo.Compressed)
                {
                    filesize = this.BinaryReader.ReadUInt32();
                    filesz -= 4;
                }

                byte[] content = this.BinaryReader.ReadBytes((int)filesz);

                if (fileInfo.Compressed == false)
                {
                    result_ms.Write(content, 0, content.Length);
                }
                else
                {
                    using (var ms = new MemoryStream(content, false))
                    using (var stream = lz4.LZ4Stream.CreateDecompressor(ms, lz4.LZ4StreamMode.Read))
                    {
                        stream.CopyTo(result_ms);
                    }
                }
            }
            else
            {
                this.BinaryReader.BaseStream.Position = (long)Offset;

                // Skip ahead
                if ((this.Archive as BSA).ContainsFileNameBlobs)
                    this.BinaryReader.BaseStream.Position += this.BinaryReader.ReadByte() + 1;

                if (!Compressed)
                {
                    byte[] bytes = new byte[Size];
                    this.BinaryReader.Read(bytes, 0, (int)Size);
                    result_ms.Write(bytes, 0, (int)Size);
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
                    result_ms.Write(uncompressed, 0, uncompressed.Length);
                }
            }

            result_ms.Seek(0, SeekOrigin.Begin);
            return result_ms;
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
