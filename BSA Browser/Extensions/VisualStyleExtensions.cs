using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BSA_Browser.Extensions
{
    public static class VisualStyleExtensions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport("uxtheme", CharSet = CharSet.Unicode)]
        public extern static Int32 SetWindowTheme(IntPtr hWnd, String textSubAppName, String textSubIdList);

        private const int EM_SETCUEBANNER = 0x1500 + 1;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1000 + 54;
        private const int LVS_EX_DOUBLEBUFFER = 0x00010000;
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVS_EX_AUTOHSCROLL = 0x0020;
        private const int UISF_HIDEFOCUS = 0x10001;
        private const int WM_CHANGEUISTATE = 0x0127;

        private static void HideFocusRectangle(this ListView lv)
        {
            SendMessage(lv.Handle, WM_CHANGEUISTATE, UISF_HIDEFOCUS, 0);
        }

        public static void EnableVisualStyles(this ListView lv)
        {
            // Enable Visual Styles
            SetWindowTheme(lv.Handle, "explorer", null);

            // Visual Style Selection
            SendMessage(lv.Handle,
                LVM_SETEXTENDEDLISTVIEWSTYLE,
                LVS_EX_DOUBLEBUFFER,
                LVS_EX_DOUBLEBUFFER);

            lv.HideFocusRectangle();

            // Re-hide focus rectangle after certain events
            lv.Enter += delegate { lv.HideFocusRectangle(); };
            lv.SelectedIndexChanged += delegate { lv.HideFocusRectangle(); };
        }

        public static void SetCue(this TextBox txt, string cue)
        {
            SendMessage(txt.Handle, EM_SETCUEBANNER, IntPtr.Zero.ToInt32(), cue);
        }

        public static void EnableVisualStyles(this TreeView tv)
        {
            // Enable Visual Styles
            SetWindowTheme(tv.Handle, "explorer", null);

            // Enable Auto Scroll
            SendMessage(tv.Handle,
                TVM_SETEXTENDEDSTYLE,
                TVS_EX_AUTOHSCROLL,
                TVS_EX_AUTOHSCROLL);
        }
    }
}
