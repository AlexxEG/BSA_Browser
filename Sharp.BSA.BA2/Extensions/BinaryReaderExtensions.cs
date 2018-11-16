﻿using System.IO;
using System.Text;

namespace SharpBSABA2.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadString(this BinaryReader reader, int charCount)
        {
            return new string(reader.ReadChars(charCount));
        }

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
    }
}
