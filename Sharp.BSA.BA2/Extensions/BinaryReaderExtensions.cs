using System.IO;

namespace SharpBSABA2.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadString(this BinaryReader reader, int charCount)
        {
            return new string(reader.ReadChars(charCount));
        }
    }
}
