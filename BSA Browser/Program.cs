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

        public static readonly string tmpPath = Path.Combine(Path.GetTempPath(), "bsa_browser");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);

#if (!DEBUG)
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

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

    /// <summary>
    ///  We inherit from WindowsFormApplicationBase which contains the logic for the application model, including
    ///  the single-instance functionality.
    /// </summary>
    class App : MsVB.WindowsFormsApplicationBase
    {
        public App()
        {
            this.IsSingleInstance = true; // makes this a single-instance app
            this.EnableVisualStyles = true; // C# windowsForms apps typically turn this on.  We'll do the same thing here.
            this.ShutdownStyle = MsVB.ShutdownMode.AfterMainFormCloses; // the vb app model supports two different shutdown styles.  We'll use this one for the sample.
        }

        /// <summary>
        /// This is how the application model learns what the main form is
        /// </summary>
        protected override void OnCreateMainForm()
        {
            if (this.CommandLineArgs.Count > 0)
                this.MainForm = new BSABrowser(this.CommandLineArgs.ToArray());
            else
                this.MainForm = new BSABrowser();
        }

        /// <summary>
        /// Gets called when subsequent application launches occur.  The subsequent app launch will result in this function getting called
        /// and then the subsequent instances will just exit.  You might use this method to open the requested doc, or whatever 
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnStartupNextInstance(MsVB.StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            eventArgs.BringToForeground = true;
            if (eventArgs.CommandLine.Count > 0)
                (this.MainForm as BSABrowser).OpenArchive(eventArgs.CommandLine[0], true);
        }
    }
}