using System;
using System.Collections.Generic;
using BSA_Browser.Enums;
using SharpBSABA2;
using SharpBSABA2.BA2Util;

namespace BSA_Browser.Sorting
{
    public class ArchiveFileSorter : Comparer<ArchiveEntry>
    {
        internal static ArchiveFileSortOrder order = 0;
        internal static bool desc = true;

        public static void SetSorter(ArchiveFileSortOrder sortOrder, bool sortDesc)
        {
            order = sortOrder;
            desc = sortDesc;
        }

        public override int Compare(ArchiveEntry a, ArchiveEntry b)
        {
            switch (order)
            {
                case ArchiveFileSortOrder.FilePath:
                    if (a.Archive.HasNameTable)
                        return desc ? string.CompareOrdinal(a.LowerPath, b.LowerPath) :
                                      string.CompareOrdinal(b.LowerPath, a.LowerPath);
                    else
                        return desc ? a.Index.CompareTo(b.Index) :
                                      b.Index.CompareTo(a.Index);

                case ArchiveFileSortOrder.FileSize:
                    return desc ? a.DisplaySize.CompareTo(b.DisplaySize) :
                                  b.DisplaySize.CompareTo(a.DisplaySize);

                case ArchiveFileSortOrder.Extra:
                    if (a is BA2TextureEntry && b is BA2TextureEntry)
                    {
                        string af = Enum.GetName(typeof(DXGI_FORMAT), (a as BA2TextureEntry).format);
                        string bf = Enum.GetName(typeof(DXGI_FORMAT), (b as BA2TextureEntry).format);
                        return desc ? string.CompareOrdinal(af, bf) :
                                      string.CompareOrdinal(bf, af);
                    }
                    else
                    {
                        // Sort by file path since Extra will be empty
                        return desc ? string.CompareOrdinal(a.LowerPath, b.LowerPath) :
                                      string.CompareOrdinal(b.LowerPath, a.LowerPath);
                    }

                default:
                    return 0;
            }
        }
    }
}