using System.IO;
using System.Text;

namespace SharpBSABA2.Extensions
{
    public static class BinaryStreamExtensions
    {
        /// <summary>
        /// Reads given amounts of <see cref="char"/> at current position and returns it as a <see cref="string"/>.
        /// </summary>
        /// <param name="charCount">Amount of <see cref="char"/> to read.</param>
        public static string ReadString(this BinaryReader reader, int charCount)
        {
            return new string(reader.ReadChars(charCount));
        }

        /// <summary>
        /// Reads <see cref="char"/> until hitting <paramref name="endChar"/> then returns them as a <see cref="string"/>.
        /// </summary>
        /// <param name="endChar">The <see cref="char"/> to read to.</param>
        public static string ReadStringTo(this BinaryReader reader, char endChar)
        {
            var sb = new StringBuilder();
            char c;
            while ((c = reader.ReadChar()) != endChar)
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from offset, and seeks back.
        /// </summary>
        public static uint ReadUInt32From(this BinaryReader reader, long offset)
        {
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            uint value = reader.ReadUInt32();
            reader.BaseStream.Position = position;
            return value;
        }

        /// <summary>
        /// Writes <paramref name="value"/> at <paramref name="position"/> then optionally seeks back.
        /// </summary>
        /// <param name="position">The position to write <paramref name="value"/>.</param>
        /// <param name="value">The <see cref="uint"/> to write.</param>
        /// <param name="returnPosition"><see cref="bool">True</see> will return position back to before operation.</param>
        public static void WriteAt(this BinaryWriter writer, long position, uint value, bool returnPosition = true)
        {
            long startPosition = writer.BaseStream.Position;
            writer.BaseStream.Position = position;
            writer.Write(value);
            if (returnPosition)
                writer.BaseStream.Position = startPosition;
        }
    }
}
