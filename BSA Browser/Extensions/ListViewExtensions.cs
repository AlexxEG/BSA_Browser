using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BSA_Browser.Extensions
{
    public static class ListViewExtensions
    {
        private const int LVM_FIRST = 0x1000;
        private const int LVM_SETITEMSTATE = LVM_FIRST + 43;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        };

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageLVItem(HandleRef hWnd, int msg, int wParam, ref LVITEM lvi);

        /// <summary>
        /// Select all items in the ListView.
        /// </summary>
        public static void SelectAllItems(this ListView listView)
        {
            ListViewExtensions.SetItemState(listView, -1, 2, 2);
        }

        /// <summary>
        /// Set the item state on the given item
        /// </summary>
        /// <param name="list">The listview whose item's state is to be changed</param>
        /// <param name="itemIndex">The index of the item to be changed</param>
        /// <param name="mask">Which bits of the value are to be set?</param>
        /// <param name="value">The value to be set</param>
        private static void SetItemState(ListView listView, int itemIndex, int mask, int value)
        {
            LVITEM lvItem = new LVITEM();
            lvItem.stateMask = mask;
            lvItem.state = value;
            SendMessageLVItem(new HandleRef(listView, listView.Handle), LVM_SETITEMSTATE, itemIndex, ref lvItem);
        }

        public static void ScrollToTop(this ListView listView)
        {
            if (listView.Items.Count == 0)
                return;

            try
            {
                listView.TopItem = listView.Items[0];
            }
            catch
            {
                // Setting TopItem in virtualization mode is apparently prone to throwing errors
            }
        }
    }
}
