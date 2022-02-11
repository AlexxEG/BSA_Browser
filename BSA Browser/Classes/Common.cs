using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BSA_Browser.Enums;
using BSA_Browser.Preview;
using BSA_Browser.Properties;
using SharpBSABA2;
using SharpBSABA2.BA2Util;
using SharpBSABA2.BSAUtil;
using SharpBSABA2.Enums;

namespace BSA_Browser.Classes
{
    public class Common
    {
        /// <summary>
        /// Returns true if there are any unsupported textures.
        /// </summary>
        public static bool CheckForUnsupportedTextures(IList<ArchiveEntry> entries)
        {
            return entries.Any(x => (x as BA2TextureEntry)?.IsFormatSupported() == false);
        }

        #region ExtractFiles variables

#if DEBUG
        private static Stopwatch _debugStopwatch = new Stopwatch();
#endif

        private static List<string> ExtractingArchives = new List<string>();

        private static void ExtractOperation_StateChange(ExtractOperation sender, StateChangeEventArgs e)
        {
            sender.ProgressForm.Description = e.FileName + '\n' + Common.FormatTimeRemaining(sender.EstimateTimeRemaining);
            sender.ProgressForm.Footer = $"({e.Count}/{e.FilesTotal}) {Common.FormatBytes(sender.SpeedBytes)}/s";
        }

        private static void ExtractOperation_ProgressPercentageUpdate(ExtractOperation sender, ProgressPercentageUpdateEventArgs e)
        {
            if (sender.ProgressForm.Owner != null && sender.TitleProgress)
                sender.ProgressForm.Owner.Text = $"{e.ProgressPercentage}% - {sender.OriginalTitle}";

            sender.ProgressForm.Progress = e.ProgressPercentage;
            sender.ProgressForm.Description = sender.ProgressForm.Description.Split('\n')[0] + "\n" + Common.FormatTimeRemaining(e.RemainingEstimate);
        }

        private static void ExtractOperation_Completed(ExtractOperation sender, CompletedEventArgs e)
        {
#if DEBUG
            _debugStopwatch.Stop();
            Console.WriteLine($"Extraction complete. {_debugStopwatch.ElapsedMilliseconds}ms elapsed");
#endif

            ExtractingArchives.RemoveAll(x => sender.Archives.Contains(x));

            sender.ProgressForm.BlockClose = false;
            sender.ProgressForm.Close();

            if (sender.ProgressForm.Owner != null && sender.TitleProgress)
                sender.ProgressForm.Owner.Text = sender.OriginalTitle;

            // Save exceptions to _report.txt file in destination path
            if (e?.Exceptions.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{sender.Files[0].Archive.FileName} - RetrieveRealSize: {sender.Files[0].Archive.RetrieveRealSize}");
                sb.AppendLine();

                foreach (var ex in e.Exceptions)
                    sb.AppendLine($"{ex.ArchiveEntry.FullPath}{Environment.NewLine}{ex.Exception}{Environment.NewLine}");

                File.WriteAllText(Path.Combine(sender.Folder, "_report.txt"), sb.ToString());
                MessageBox.Show(sender.ProgressForm.Owner, $"{e.Exceptions.Count} file(s) failed to extract. See report file in destination for details.", "Error");
            }
        }

        #endregion

        /// <summary>
        /// Extracts the given file(s) to the given path.
        /// </summary>
        /// <param name="folder">The path to extract files to.</param>
        /// <param name="useFolderPath">True to use full folder path for files, false to extract straight to path.</param>
        /// <param name="gui">True to show a <see cref="ProgressForm"/>.</param>
        /// <param name="files">The files in the selected archive to extract.</param>
        public static void ExtractFiles(Form owner,
                                        string folder,
                                        bool useFolderPath,
                                        bool gui,
                                        List<ArchiveEntry> files,
                                        ProgressForm progressForm = null,
                                        bool titleProgress = false)
        {
            // Store all unique archives to prevent extracting same archive across multiple operations at the same time
            var archives = files.Select(x => x.Archive.FullPath.ToLower()).Distinct();

            if (ExtractingArchives.Any(x => archives.Contains(x)))
            {
                MessageBox.Show(owner,
                    "One or more archives are already being extracted from, try again later.",
                    "BSA Browser",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Check for unsupported textures and prompt the user what to do if there is any
            if (CheckAndHandleUnsupportedTextures(owner, files) == DialogResult.Cancel)
                return;

            if (gui)
            {
                progressForm = progressForm ?? new ProgressForm(files.Count);

                var operation = new ExtractOperation(folder, files, useFolderPath)
                {
                    Archives = archives,
                    ProgressForm = progressForm,
                    TitleProgress = titleProgress,
                    OriginalTitle = owner?.Text
                };
                operation.StateChange += ExtractOperation_StateChange;
                operation.ProgressPercentageUpdate += ExtractOperation_ProgressPercentageUpdate;
                operation.Completed += ExtractOperation_Completed;

                progressForm.Owner = owner;
                progressForm.Canceled += delegate { operation.Cancel(); };

#if DEBUG
                // Track extraction speed
                _debugStopwatch.Restart();
#endif

                operation.Start();
                progressForm.Show(owner);
                ExtractingArchives.AddRange(archives);
            }
            else
            {
                try
                {
                    foreach (var fe in files)
                        fe.Extract(folder, useFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(owner, ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Formats file size into a human readable <see cref="string"/>.
        /// </summary>
        /// <param name="bytes">The file size to format.</param>
        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        /// <summary>
        /// Formats <see cref="TimeSpan"/> into a human readable <see cref="string"/>.
        /// </summary>
        /// <param name="time">The <see cref="TimeSpan"/> to format.</param>
        public static string FormatTimeRemaining(TimeSpan time)
        {
            List<string> ss = new List<string>();

            if (time.Hours > 0) ss.Add(time.Hours == 1 ? "1 hour" : $"{time.Hours} hours");
            if (time.Minutes > 0) ss.Add(time.Minutes == 1 ? "1 minute" : $"{time.Minutes} minutes");
            if (time.Seconds > 0) ss.Add(time.Seconds == 1 ? "1 second" : $"{time.Seconds} seconds");

            switch (ss.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return $"About {ss[0]} remaining";
                case 2:
                    return $"About {ss[0]} and {ss[1]} remaining";
                case 3:
                    return $"About {ss[0]}, {ss[1]} and {ss[2]} remaining";
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Opens and return <see cref="Archive"/> of <paramref name="file"/>.
        /// </summary>
        /// <param name="file">Archive file to open.</param>
        /// <param name="owner">Used with <see cref="MessageBox.Show(IWin32Window, string)"/>.</param>
        public static Archive OpenArchive(string file, IWin32Window owner = null)
        {
            Archive archive;

            try
            {
                string extension = Path.GetExtension(file);
                Encoding encoding = Encoding.GetEncoding(Settings.Default.EncodingCodePage);

                // ToDo: Read file header to find archive type, not just extension
                switch (extension.ToLower())
                {
                    case ".bsa":
                    case ".dat":
                        if (BSA.IsSupportedVersion(file, encoding) == false)
                        {
                            if (Common.ShowMessageBoxInvoke(owner,
                                    "Archive has an unknown version number.\n" + "Attempt to open anyway?",
                                    "Warning",
                                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return null;
                        }

                        archive = new BSA(file, encoding, Settings.Default.RetrieveRealSize)
                        {
                            MatchLastWriteTime = Settings.Default.MatchLastWriteTime
                        };
                        break;
                    case ".ba2":
                        archive = new BA2(file, encoding, Settings.Default.RetrieveRealSize)
                        {
                            MatchLastWriteTime = Settings.Default.MatchLastWriteTime
                        };

                        if (archive.Type == ArchiveTypes.BA2_GNMF)
                        {
                            // Check if extensions for GNF textures should be replaced
                            Common.ReplaceGNFExtensions(archive.Files.OfType<BA2GNFEntry>(), Settings.Default.ReplaceGNFExt);
                        }
                        break;
                    default:
                        throw new Exception($"Unrecognized archive file type ({extension}).");
                }
            }
            catch (Exception ex)
            {
                Common.ShowMessageBoxInvoke(owner,
                    "An error occured trying to open the archive. Changing the Encoding in Options can help, please try before reporting.\n\n" + ex.ToStringInvariant(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }

            return archive;
        }

        public static void PreviewTexture(IWin32Window owner, ArchiveEntry entry)
        {
            if (entry == null)
                throw new ArgumentException("Parameter 'entry' can't be null.", nameof(entry));

            string fileName = entry.FileName;
            var extension = Path.GetExtension(entry.LowerPath);

            switch (extension)
            {
                case ".dds":
                case ".bmp":
                case ".png":
                case ".jpg":
                    if ((entry as BA2TextureEntry)?.IsFormatSupported() == false)
                    {
                        MessageBox.Show(owner, "Unsupported DDS texture.");
                        return;
                    }

                    if (!Settings.Default.BuiltInPreviewing.Contains(extension))
                        goto default;

                    if (entry is BA2GNFEntry)
                    {
                        if (Settings.Default.ReplaceGNFExt)
                        {
                            fileName = Path.GetFileNameWithoutExtension(fileName) + ".gnf";
                        }

                        goto default;
                    }

                    try
                    {
                        DDSViewer.Show(owner, entry.FileName, entry.GetDataStream());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        goto default;
                    }
                    break;
                case ".txt":
                case ".bat":
                case ".xml":
                case ".lst":
                case ".psc":
                case ".json":
                    if (!Settings.Default.BuiltInPreviewing.Contains(extension))
                        goto default;

                    new TextViewer(entry).Show(owner);
                    break;
                default:
                    string dest = Program.CreateTempDirectory();
                    string file = Path.Combine(dest, fileName);
                    entry.Extract(dest, false, fileName);

                    try
                    {
                        Process.Start(new ProcessStartInfo(file));
                    }
                    catch (Win32Exception ex)
                    {
                        if (ex.NativeErrorCode == (int)SystemErrorCodes.ERROR_NO_ASSOCIATION)
                            ShellExecute.OpenWith(file);
                        else if (ex.NativeErrorCode != (int)SystemErrorCodes.ERROR_CANCELLED)
                            MessageBox.Show(owner, ex.Message, "Preview Error");
                    }
                    break;
            }
        }

        /// <summary>
        /// Replaces GNF texture extensions with '.dds' or '.gnf' depending on <paramref name="replaceWithGNF"/>.
        /// </summary>
        public static void ReplaceGNFExtensions(IEnumerable<BA2GNFEntry> files, bool replaceWithGNF)
        {
            foreach (BA2GNFEntry entry in files)
            {
                if (replaceWithGNF && Path.GetExtension(entry.FullPath).ToLower() == ".dds")
                {
                    entry.FullPath = Path.Combine(
                        Path.GetDirectoryName(entry.FullPath),
                        Path.GetFileNameWithoutExtension(entry.FullPath));

                    string orgExt = Path.GetExtension(entry.FullPathOriginal);
                    string newExt = ".gnf";
                    for (int i = 0; i < newExt.Length; i++)
                    {
                        // Match casing in all configurations, for example .DdS -> .GnF
                        entry.FullPath += char.IsUpper(orgExt[i]) ? char.ToUpper(newExt[i]) : newExt[i];
                    }
                }
                else
                {
                    entry.FullPath = entry.FullPathOriginal;
                }
            }
        }

        /// <summary>
        /// Checks and handles user preference on how to handle unsupported textures.
        /// </summary>
        private static DialogResult CheckAndHandleUnsupportedTextures(IWin32Window owner, List<ArchiveEntry> files)
        {
            if (!Common.CheckForUnsupportedTextures(files))
                return DialogResult.No;

            DialogResult result = MessageBox.Show(owner,
                "There are unsupported textures about to be extracted. These are missing DDS headers that can't be generated.\n\n" +
                "Do you want to extract the raw data without DDS header? Selecting 'No' will skip these textures.",
                "Unsupported Textures", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.No)
            {
                // Remove unsupported textures
                files.RemoveAll(x => (x as BA2TextureEntry)?.IsFormatSupported() == false);
            }
            else if (result == DialogResult.Yes)
            {
                foreach (var fe in files.OfType<BA2TextureEntry>().Where(x => x.IsFormatSupported() == false))
                {
                    fe.GenerateTextureHeader = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Shows <see cref="MessageBox"/>, invoking if required by <paramref name="owner"/>.
        /// </summary>
        private static DialogResult ShowMessageBoxInvoke(IWin32Window owner,
                                                         string text,
                                                         string title,
                                                         MessageBoxButtons buttons = MessageBoxButtons.OK,
                                                         MessageBoxIcon icon = MessageBoxIcon.None)
        {
            var msgBoxFunc = new Func<DialogResult>(() => MessageBox.Show(owner, text, title, buttons, icon));

            if (owner is Form form)
                return (DialogResult)form.Invoke(msgBoxFunc);

            // 'owner' is either null or not a Form, so just try
            return msgBoxFunc();
        }
    }

    public static class CommonExtensions
    {
        public static List<List<T>> Split<T>(this List<T> list, int chunkCount)
        {
            var chunks = new List<List<T>>();
            decimal chunkSize = Math.Floor((decimal)list.Count / chunkCount);

            for (int i = 0; i < chunkCount; i++)
            {
                // Skip amount of files already processed then take amount of files based on chunkSize
                chunks.Add(list
                    .Skip(chunks.Select(x => x.Count).Sum())
                    .Take((int)chunkSize)
                    .ToList());
            }

            // Add the rest, if any
            chunks[chunks.Count() - 1].AddRange(list.Skip(chunks.Select(x => x.Count).Sum()));

            return chunks;
        }

        /// <summary>
        /// Returns <see cref="Exception.ToString"/> with <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        public static string ToStringInvariant(this Exception exception)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var cultureUI = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // Get it here so we can switch back Culture as fast as possible, just in case
            string exceptionMsg = exception.ToString();

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUI;

            return exceptionMsg;
        }
    }
}
