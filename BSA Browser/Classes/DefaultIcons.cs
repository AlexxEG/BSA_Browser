using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BSA_Browser.Classes
{
    public static class DefaultIcons
    {
        private static Icon filesIcon;

        private static Icon folderIconLarge;
        private static Icon folderIconSmall;

        private static Icon folderIconLargeOpen;
        private static Icon folderIconSmallOpen;

        public static Icon Files => filesIcon ?? (filesIcon = GetStockIcon(SHSIID_DRIVEFIXED, SHGSI_SMALLICON));

        public static Icon FolderLarge => folderIconLarge ?? (folderIconLarge = GetStockIcon(SHSIID_FOLDER, SHGSI_LARGEICON));
        public static Icon FolderSmall => folderIconSmall ?? (folderIconSmall = GetStockIcon(SHSIID_FOLDER, SHGSI_SMALLICON));

        public static Icon FolderLargeOpen => folderIconLargeOpen ?? (folderIconLargeOpen = GetStockIcon(SHSIID_FOLDEROPEN, SHGSI_LARGEICON));
        public static Icon FolderSmallOpen => folderIconSmallOpen ?? (folderIconSmallOpen = GetStockIcon(SHSIID_FOLDEROPEN, SHGSI_SMALLICON));

        private static Icon GetStockIcon(uint type, uint size)
        {
            var info = new SHSTOCKICONINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);

            SHGetStockIconInfo(type, SHGSI_ICON | size, ref info);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
            DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak

            return icon;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        [DllImport("shell32.dll")]
        public static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);

        // https://docs.microsoft.com/nb-no/windows/win32/api/shellapi/ne-shellapi-shstockiconid
        private const uint SHSIID_FOLDER = 0x3;
        private const uint SHSIID_FOLDEROPEN = 0x4;
        private const uint SHSIID_DRIVEFIXED = 0x8;

        private const uint SHGSI_ICON = 0x100;
        private const uint SHGSI_LARGEICON = 0x0;
        private const uint SHGSI_SMALLICON = 0x1;
    }
}
