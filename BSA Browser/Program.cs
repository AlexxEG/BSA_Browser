using BSA_Browser.Classes;
using BSA_Browser.Properties;
using Microsoft.VisualBasic.Devices;
using SharpBSABA2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    enum ExtractDestinations
    {
        Directory,
        Here
    }

    class App : MsVB.WindowsFormsApplicationBase
    {
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
                var parsed = new ParsedArguments(this.CommandLineArgs);

                if (parsed.Extract)
                {
                    this.Extract(parsed.ExtractFile, parsed.ExtractDestination, true);
                }
                else
                {
                    this.MainForm = new BSABrowser(parsed.Files.ToArray());
                }
            }
        }

        protected override void OnStartupNextInstance(MsVB.StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);

            var parsed = new ParsedArguments(eventArgs.CommandLine);

            if (parsed.Extract)
            {
                this.Extract(parsed.ExtractFile, parsed.ExtractDestination, false);
            }
            else
            {
                eventArgs.BringToForeground = true;
                if (eventArgs.CommandLine.Count > 0)
                    (this.MainForm as BSABrowser).OpenArchive(eventArgs.CommandLine[0], true);
            }
        }

        private void Extract(string file, ExtractDestinations destination, bool exitOnComplete)
        {
            Archive archive = Common.OpenArchive(file, null);

            ProgressForm progressForm = Common.CreateProgressForm(archive.Files.Count);
            progressForm.StartPosition = FormStartPosition.CenterScreen;

            if (exitOnComplete)
                progressForm.FormClosed += (sender, e) => { Application.Exit(); };

            string folder = Path.GetDirectoryName(file);
            if (destination == ExtractDestinations.Directory)
                folder = Path.Combine(folder, Path.GetFileNameWithoutExtension(file));

            BSABrowser.ExtractFiles(null, folder, true, true, archive.Files, progressForm);

            if (exitOnComplete)
                this.MainForm = progressForm;
        }
    }

    class ParsedArguments
    {
        public bool Extract { get; private set; } = false;
        public string ExtractFile { get; private set; }
        public ExtractDestinations ExtractDestination { get; private set; } = ExtractDestinations.Here;
        public ReadOnlyCollection<string> Files { get; private set; }

        public ParsedArguments(IList<string> arguments)
        {
            var files = new List<string>();

            for (int i = 0; i < arguments.Count; i++)
            {
                switch (arguments[i].ToLower())
                {
                    case "/extract":
                        this.Extract = true;
                        this.ExtractFile = arguments[++i];
                        break;
                    case "/d":
                        this.ExtractDestination = ExtractDestinations.Directory;
                        break;
                    case "/h":
                        this.ExtractDestination = ExtractDestinations.Here;
                        break;
                    default: // Assume rest are files
                        files.Add(arguments[i]);
                        break;
                }
            }

            this.Files = files.AsReadOnly();
        }

        public override string ToString()
        {
            return $"{nameof(Extract)}: {Extract}\n" +
                $"{nameof(ExtractFile)}: {ExtractFile}\n" +
                $"{nameof(ExtractDestination)}: {ExtractDestination}\n" +
                $"{nameof(Files)}: {string.Join("\n", Files)}";
        }
    }
}