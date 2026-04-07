using SharpBSABA2.Utils;
using System;
using System.IO;

namespace SharpBSABA2.BSAUtil
{
    public class XNGBSAFileEntry : ArchiveEntry
    {
        public ushort RecordId { get; private set; }

        public override uint DisplaySize => this.RealSize > 0 ? this.RealSize : this.Size;

        public XNGBSAFileEntry(Archive archive,
                               string fullPath,
                               ulong offset,
                               uint size,
                               bool compressed,
                               ushort recordId)
            : base(archive)
        {
            this.FullPath = fullPath;
            this.FullPathOriginal = this.FullPath;
            this.Offset = offset;
            this.Size = size;
            this.Compressed = compressed;
            this.RecordId = recordId;
        }

        public override ulong GetSizeInArchive(SharedExtractParams extractParams)
        {
            return this.Size;
        }

        public override string GetToolTipText()
        {
            return $"Offset:\t\t {Offset}\n" +
                   $"Size:\t\t {Size}\n" +
                   $"Real Size:\t {RealSize}\n" +
                   $"Compressed:\t {Compressed}\n" +
                   $"Record ID:\t {RecordId}";
        }

        protected override void WriteDataToStream(Stream stream, SharedExtractParams extractParams, bool decompress = true)
        {
            var reader = extractParams.Reader;
            reader.BaseStream.Position = (long)this.Offset;
            this.BytesWritten = 0;

            if (!decompress || !this.Compressed)
            {
                StreamUtils.WriteSectionToStream(reader.BaseStream,
                    this.Size,
                    stream,
                    bytesWritten => this.BytesWritten = bytesWritten);
                return;
            }

            if (this.Size > int.MaxValue)
                throw new InvalidOperationException("Compressed XnGine record is too large to extract in memory.");

            byte[] data = reader.ReadBytes((int)this.Size);
            if (data.Length != this.Size)
                throw new EndOfStreamException("Unexpected end of stream while reading compressed XnGine record.");

            byte[] decompressed = CompressionUtils.DecompressBattlespireLzss(data);
            stream.Write(decompressed, 0, decompressed.Length);
            this.RealSize = (uint)decompressed.Length;
            this.BytesWritten = (ulong)decompressed.Length;
        }
    }
}
