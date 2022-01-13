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
using BSA_Browser.Dialogs;
using BSA_Browser.Enums;
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
                            if (MessageBox.Show(owner,
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
                MessageBox.Show(owner,
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
                        DDSViewer.ShowDialog(owner, entry.FileName, entry.GetDataStream());
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
