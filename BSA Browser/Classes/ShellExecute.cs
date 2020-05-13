using System;
using System.Runtime.InteropServices;

namespace BSA_Browser.Classes
{
    public static class ShellExecute
    {
        [Serializable]
        private struct ShellExecuteInfo
        {
            public int Size;
            public uint Mask;
            public IntPtr hwnd;
            public string Verb;
            public string File;
            public string Parameters;
            public string Directory;
            public uint Show;
            public IntPtr InstApp;
            public IntPtr IDList;
            public string Class;
            public IntPtr hkeyClass;
            public uint HotKey;
            public IntPtr Icon;
            public IntPtr Monitor;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private extern static bool ShellExecuteEx(ref ShellExecuteInfo lpExecInfo);

        private const uint SW_NORMAL = 1;

        public static void OpenWith(string file)
        {
            ShellExecuteInfo sei = new ShellExecuteInfo();
            sei.Size = Marshal.SizeOf(sei);
            sei.Verb = "openas";
            sei.File = file;
            sei.Show = SW_NORMAL;
            if (!ShellExecuteEx(ref sei))
                throw new System.ComponentModel.Win32Exception();
        }
    }
}
