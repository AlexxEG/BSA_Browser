using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BSA_Browser
{
    static class Program
    {
        public static readonly string tmpPath = Path.Combine(Path.GetTempPath(), "bsa_browser");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);

            App myApp = new App();
            myApp.Run(args);
        }

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
            throw new fommException("Could not create temp folder because directory is full");
        }

        public static string GetVersion()
        {
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format("{0}.{1}.{0}", v.Major, v.Minor, v.Build);
        }
    }

    /// <summary>
    ///  We inherit from WindowsFormApplicationBase which contains the logic for the application model, including
    ///  the single-instance functionality.
    /// </summary>
    class App : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        public App()
        {
            this.IsSingleInstance = true; // makes this a single-instance app
            this.EnableVisualStyles = true; // C# windowsForms apps typically turn this on.  We'll do the same thing here.
            this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses; // the vb app model supports two different shutdown styles.  We'll use this one for the sample.
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
        protected override void OnStartupNextInstance(Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            eventArgs.BringToForeground = true;
            if (eventArgs.CommandLine.Count > 0)
                (this.MainForm as BSABrowser).OpenArchive(eventArgs.CommandLine[0], true);
        }
    }

    public class fommException : Exception { public fommException(string msg) : base(msg) { } }
}