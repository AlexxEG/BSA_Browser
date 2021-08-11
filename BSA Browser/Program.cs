using BSA_Browser.Classes;
using BSA_Browser.Properties;
using Microsoft.VisualBasic.Devices;
using SharpBSABA2;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MsVB = Microsoft.VisualBasic.ApplicationServices;

namespace BSA_Browser
{
    static class Program
    {
        public const string Fallout4Nexus = "https://www.nexusmods.com/fallout4/mods/17061";
        public const string SkyrimSENexus = "https://www.nexusmods.com/skyrimspecialedition/mods/1756";
        public const string GitHub = "https://github.com/AlexxEG/BSA_Browser";
        public const string Discord = "https://discord.gg/k97ACqK";
        public const string VersionUrl = "https://raw.githubusercontent.com/AlexxEG/BSA_Browser/master/VERSION";

        public static bool SettingsReset = false;

        public static readonly string tmpPath = Path.Combine(Path.GetTempPath(), "bsa_browser");

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.SetCompatibleTextRenderingDefault(false);

#if (!DEBUG)
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            if (ShouldResetSettings())
            {
                Settings.Default.Reset();
                SettingsReset = true;
            }

            new App().Run(args);
        }

#if (!DEBUG)
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Program.SaveException(e.ExceptionObject as Exception);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Program.SaveException(e.Exception);
        }
#endif

        internal static string CreateTempDirectory()
        {
            string tmp;
            for (int i = 0; i < 32000; i++)
            {
                tmp = Path.Combine(tmpPath, i.ToString());
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                    return tmp + Path.DirectorySeparatorChar;
                }
            }
            throw new Exception("Could not create temp folder because directory is full");
        }

        public static string GetVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;

            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        private static bool ShouldResetSettings()
        {
            var kb = new Keyboard();
            return kb.AltKeyDown && kb.CtrlKeyDown && kb.ShiftKeyDown;
        }

#if (!DEBUG)
        private static bool HasWriteAccess(string folderPath)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static void SaveException(Exception exception)
        {
            string dir = Path.Combine(Application.StartupPath, "stack traces");

            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch (UnauthorizedAccessException)
            {
                dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BSA Browser", "stack traces");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                File.WriteAllText(Path.Combine(dir, DateTime.Now.ToString("yyyy.MM.dd-HH-mm-ss-fff")) + ".log", exception.ToString());
            }
        }
#endif
    }

    class App : MsVB.WindowsFormsApplicationBase
    {
        ProgressForm _progressForm;

        public App()
        {
            this.IsSingleInstance = true;
            this.EnableVisualStyles = true;
            this.ShutdownStyle = MsVB.ShutdownMode.AfterMainFormCloses;
        }

        protected override void OnCreateMainForm()
        {
            if (this.CommandLineArgs.Count == 0)
                this.MainForm = new BSABrowser();
            else
            {
                if (this.CommandLineArgs[0].ToLower() == "/extract")
                {
                    this.Extract();
                }
                else
                {
                    this.MainForm = new BSABrowser(this.CommandLineArgs.ToArray());
                }
            }
        }

        protected override void OnStartupNextInstance(MsVB.StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            eventArgs.BringToForeground = true;
            if (eventArgs.CommandLine.Count > 0)
                (this.MainForm as BSABrowser).OpenArchive(eventArgs.CommandLine[0], true);
        }

        private void Extract()
        {
            string path = this.CommandLineArgs[1];
            Archive archive = Common.OpenArchive(path, null);

            _progressForm = Common.CreateProgressForm(archive.Files.Count);
            _progressForm.StartPosition = FormStartPosition.CenterScreen;
            _progressForm.FormClosed += (sender, e) => { Application.Exit(); };

            BSABrowser.ExtractFiles(null,
                Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)),
                true,
                true,
                archive.Files,
                _progressForm);

            this.MainForm = _progressForm;
        }
    }
}