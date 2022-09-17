using SharpBSABA2.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpBSABA2.Utils
{
    public static class MorrowindNameTable
    {
        private const string FileName = "mw_hash_table.bin";

        private static Dictionary<ulong, string> NameTable = new Dictionary<ulong, string>();

        private static Random _Random = new Random();

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
                    if (_Random.NextDouble() > 0.2)
                        continue; // Skip for testing purposes

                    NameTable.Add(file.ReadUInt64(), file.ReadStringTo('\0'));
                }
            }
        }

        public static bool Contains(ulong hash) => NameTable.ContainsKey(hash);

        public static string Get(ulong hash) => NameTable[hash];
    }
}
