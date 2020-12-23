using System;
using System.IO;

namespace SharpBSABA2.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] data = new byte[count];
            int read = stream.Read(data, 0, count);
            byte[] trimmed = new byte[read];

            Array.Copy(data, trimmed, read);

            return trimmed;
        }
    }
}
