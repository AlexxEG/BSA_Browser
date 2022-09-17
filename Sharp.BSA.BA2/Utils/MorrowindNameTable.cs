using SharpBSABA2.Extensions;
using System.Collections.Generic;
using System.IO;

namespace SharpBSABA2.Utils
{
    public static class MorrowindNameTable
    {
        private const string FileName = "mw_hash_table.bin";

        private static Dictionary<ulong, string> NameTable = new Dictionary<ulong, string>();

        static MorrowindNameTable()
        {
            if (!File.Exists(FileName))
                return;

            using (var file = new BinaryReader(File.OpenRead(FileName)))
            {
                file.BaseStream.Seek(12, SeekOrigin.Begin);
                uint count = file.ReadUInt32();

                for (int i = 0; i < count; i++)
                {
                    NameTable.Add(file.ReadUInt64(), file.ReadStringTo('\0'));
                }
            }
        }

        public static bool Contains(ulong hash) => NameTable.ContainsKey(hash);

        public static string Get(ulong hash) => NameTable[hash];
    }
}
