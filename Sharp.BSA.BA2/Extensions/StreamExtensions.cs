using System.IO;

namespace SharpBSABA2.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] data = new byte[count];
            stream.Read(data, 0, count);
            return data;
        }
    }
}
