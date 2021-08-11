using BSA_Browser.Properties;
using SharpBSABA2;
using SharpBSABA2.BA2Util;
using SharpBSABA2.BSAUtil;
using SharpBSABA2.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        /// Creates <see cref="ProgressForm"/> with default settings.
        /// </summary>
        /// <param name="fileCount">File count in footer.</param>
        public static ProgressForm CreateProgressForm(int fileCount)
        {
            return new ProgressForm("Unpacking archive")
            {
                Header = "Extracting...",
                Footer = $"(0/{fileCount})",
                Cancelable = true,
                Maximum = 100
            };
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
                        if (BSA.IsSupportedVersion(file) == false)
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
                MessageBox.Show(ex.Message);
                return null;
            }

            return archive;
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
}
