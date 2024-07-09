using BSA_Browser.Enums;
using SharpBSABA2;
using SharpBSABA2.BA2Util;
using System;
using System.Collections.Generic;

namespace BSA_Browser.Sorting
{
    public class ArchiveFileSorter : Comparer<ArchiveEntry>
    {
        public static SortingConfig SortingConfig { get; set; } = new SortingConfig(true, 0);

        public static void SetSorter(ArchiveFileSortOrder sortOrder, bool sortDesc)
        {
            SortingConfig = new SortingConfig(sortDesc, sortOrder);
        }

        public override int Compare(ArchiveEntry a, ArchiveEntry b)
        {
            switch (SortingConfig.Order)
            {
                case ArchiveFileSortOrder.FilePath:
                    if (a.Archive.HasNameTable || a.HadHashTranslated && b.HadHashTranslated)
                    {
                        return SortingConfig.Descending
                            ? string.Compare(a.FullPath, b.FullPath, StringComparison.OrdinalIgnoreCase)
                            : string.Compare(b.FullPath, a.FullPath, StringComparison.OrdinalIgnoreCase);
                    }
                    else if (a.HadHashTranslated != b.HadHashTranslated)
                    {
                        // Only one was translated, sort hash last always
                        return a.HadHashTranslated ? -1 : 1;
                    }
                    else
                    {
                        return SortingConfig.Descending
                            ? a.Index.CompareTo(b.Index)
                            : b.Index.CompareTo(a.Index);
                    }

                case ArchiveFileSortOrder.FileSize:
                    return SortingConfig.Descending
                        ? a.DisplaySize.CompareTo(b.DisplaySize)
                        : b.DisplaySize.CompareTo(a.DisplaySize);

                case ArchiveFileSortOrder.Archive:
                    return SortingConfig.Descending
                        ? string.Compare(a.Archive.FileName, b.Archive.FileName, StringComparison.OrdinalIgnoreCase)
                        : string.Compare(b.Archive.FileName, a.Archive.FileName, StringComparison.OrdinalIgnoreCase);

                case ArchiveFileSortOrder.Extra:
                    if (a is BA2TextureEntry && b is BA2TextureEntry)
                    {
                        string af = Enum.GetName(typeof(DXGI_FORMAT), (a as BA2TextureEntry).format);
                        string bf = Enum.GetName(typeof(DXGI_FORMAT), (b as BA2TextureEntry).format);

                        return SortingConfig.Descending
                            ? string.CompareOrdinal(af, bf)
                            : string.CompareOrdinal(bf, af);
                    }
                    else
                    {
                        // Sort by file path since Extra will be empty
                        return SortingConfig.Descending
                            ? string.Compare(a.FullPath, b.FullPath, StringComparison.OrdinalIgnoreCase)
                            : string.Compare(b.FullPath, a.FullPath, StringComparison.OrdinalIgnoreCase);
                    }

                default:
                    return 0;
            }
        }
    }
}