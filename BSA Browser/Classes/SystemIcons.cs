using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BSA_Browser.Classes
{
    // From https://stackoverflow.com/a/59129804
    public static class SystemIcons
    {
        private static Dictionary<string, Icon> _cache = new Dictionary<string, Icon>();

        public static Icon Files => GetCachedIcon(nameof(Files), SHSTOCKICONID.SIID_DRIVEFIXED, SHGSI_SMALLICON);
        public static Icon FilesNoAssoc => GetCachedIcon(nameof(FilesNoAssoc), SHSTOCKICONID.SIID_DOCNOASSOC, SHGSI_SMALLICON);
        public static Icon FolderLarge => GetCachedIcon(nameof(FolderLarge), SHSTOCKICONID.SIID_FOLDER, SHGSI_LARGEICON);
        public static Icon FolderSmall => GetCachedIcon(nameof(FolderSmall), SHSTOCKICONID.SIID_FOLDER, SHGSI_SMALLICON);
        public static Icon FolderLargeOpen => GetCachedIcon(nameof(FolderLargeOpen), SHSTOCKICONID.SIID_FOLDEROPEN, SHGSI_LARGEICON);
        public static Icon FolderSmallOpen => GetCachedIcon(nameof(FolderSmallOpen), SHSTOCKICONID.SIID_FOLDEROPEN, SHGSI_SMALLICON);

        private static Icon GetCachedIcon(string name, SHSTOCKICONID type, uint size)
        {
            if (!_cache.ContainsKey(name))
                _cache.Add(name, GetStockIcon(type, size));
            return _cache[name];
        }

        private static Icon GetStockIcon(SHSTOCKICONID type, uint size)
        {
            var info = new SHSTOCKICONINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);

            SHGetStockIconInfo((uint)type, SHGSI_ICON | size, ref info);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
            DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak

            return icon;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        // https://docs.microsoft.com/nb-no/windows/win32/api/shellapi/ne-shellapi-shstockiconid
        private enum SHSTOCKICONID : uint
        {
            SIID_DOCNOASSOC = 0x0,
            SIID_FOLDER = 0x3,
            SIID_FOLDEROPEN = 0x4,
            SIID_DRIVEFIXED = 0x8
        }

        private const uint SHGSI_ICON = 0x100;
        private const uint SHGSI_LARGEICON = 0x0;
        private const uint SHGSI_SMALLICON = 0x1;
    }
}
