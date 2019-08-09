using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BSA_Browser.Classes;
using BSA_Browser.Extensions;
using BSA_Browser.Packing.Hashing;
using BSA_Browser.Properties;
using BSA_Browser.Sorting;
using Ionic.Zlib;

namespace BSA_Browser.Packing
{
    public partial class PackerForm : Form
    {
        public const int DDS_HEADER_SIZE = 124;
        public const int DDS_HEADER_DXT10_SIZE = 144;
        public const uint FOURCC_DXT10 = 0x35545844;

        public class FileRecord
        {
            public uint NameHash { get; set; }
            public char[] Extension { get; set; }
            public uint DirHash { get; set; }
            public uint Flags { get; set; } = 0x00100100u;
            public ulong Offset { get; set; }
            public uint FileSize { get; set; }
            public uint RealSize { get; set; }
            public uint Align { get; set; } = 0xBAADF00Du;

            public string Name { get; private set; }
            public string FilePath { get; private set; }

            public FileRecord(string name, string filePath)
            {
                this.Name = name;
                this.FilePath = filePath;
                this.NameHash = CRC32.Compute(Path.GetFileNameWithoutExtension(FilePath).ToLower());

                string ext = Path.GetExtension(FilePath).Remove(0, 1);
                if (ext.Length > 4) throw new Exception("Extension is too long.");
                var extChars = new List<char>(ext.ToCharArray());
                while (extChars.Count < 4) extChars.Add('\0');

                this.Extension = extChars.ToArray();
                this.DirHash = CRC32.Compute(Path.GetDirectoryName(FilePath).ToLower());
                this.RealSize = (uint)new FileInfo(FilePath).Length;
            }
        }

        public class TextureRecord
        {
            public uint nameHash { get; set; }
            public string FullPath { get; set; }
            public char[] extension { get; set; }
            public uint dirHash { get; set; }
            public byte unk8 { get; set; } = 0;
            public byte numChunks { get; set; }
            public ushort chunkHdrLen { get; set; } = 24;
            public ushort height { get; set; }
            public ushort width { get; set; }
            public byte numMips { get; set; }
            public byte format { get; set; }
            // ToDo: Figure out when this should be 2049. Currently only a few textures in Shared/CubeMaps use 2049
            public ushort unk16 { get; set; } = 2048;

            public class Chunk
            {
                public ulong offset;
                public uint packSz;
                public uint fullSz;
                public ushort startMip;
                public ushort endMip;
                public uint align = 0xBAADF00D;
            }

            public Chunk[] chunks { get; set; }
        }

        public CompressionLevel SelectedCompression
        {
            get
            {
                switch (cbCompression.SelectedIndex)
                {
                    case 0:
                        return CompressionLevel.None;
                    case 1:
                        return CompressionLevel.Default;
                    case 2:
                        return CompressionLevel.BestCompression;
                    case 3:
                        return CompressionLevel.BestSpeed;
                    default:
                        throw new Exception("Invalid selected compression value.");
                }
            }
        }

        public PackerForm()
        {
            InitializeComponent();

            this.SetupContextMenu();

            cbCompression.SelectedIndex = 1;

            lvFiles.EnableVisualStyles();
        }

        private void PackerForm_Load(object sender, EventArgs e)
        {
        }

        private void PackerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        #region ContextMenu

        MenuItem excludeFromPath;
        MenuItem removeMenuItem;
        MenuItem selectAllMenuItem;

        private void SetupContextMenu()
        {
            excludeFromPath = new MenuItem("Exclude From Path", excludeFromPath_Click);
            excludeFromPath.Popup += excludeFromPath_Popup;
            removeMenuItem = new MenuItem("Remove", removeMenuItem_Click);
            removeMenuItem.Shortcut = Shortcut.Del;
            selectAllMenuItem = new MenuItem("Select All", selectAllMenuItem_Click);
            selectAllMenuItem.Shortcut = Shortcut.CtrlA;

            lvFiles.ContextMenu = new ContextMenu(new MenuItem[] { excludeFromPath, removeMenuItem, selectAllMenuItem });
            lvFiles.ContextMenu.Popup += ContextMenu_Popup;
        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            excludeFromPath.Enabled = removeMenuItem.Enabled = selectAllMenuItem.Enabled = lvFiles.SelectedItems.Count > 0;
            excludeFromPath.MenuItems.Clear();
            excludeFromPath.MenuItems.Add("-");
        }

        private void excludeFromPath_Click(object sender, EventArgs e)
        {

        }

        private void excludeFromPath_Popup(object sender, EventArgs e)
        {
            excludeFromPath.MenuItems.Clear();

            var paths = lvFiles.SelectedItems.Cast<ListViewItem>()
                .Select(i => Path.GetDirectoryName(i.Text))
                .Distinct().ToArray();

            foreach (string path in this.SplitPath(this.GetCommonPath(paths)))
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                excludeFromPath.MenuItems.Add(path, excludeFromPathItem_Click);
            }

            if (excludeFromPath.MenuItems.Count == 0)
                excludeFromPath.MenuItems.Add(new MenuItem("None") { Enabled = false });
        }

        private void excludeFromPathItem_Click(object sender, EventArgs e)
        {
            string path = (sender as MenuItem).Text + Path.DirectorySeparatorChar;

            foreach (ListViewItem item in lvFiles.SelectedItems)
            {
                item.Text = item.Text.Replace(path, "");
            }
        }

        private void removeMenuItem_Click(object sender, EventArgs e)
        {
            lvFiles.BeginUpdate();

            for (int i = lvFiles.SelectedItems.Count - 1; i >= 0; i--)
            {
                lvFiles.Items.RemoveAt(lvFiles.SelectedItems[i].Index);
            }

            lvFiles.EndUpdate();
        }

        private void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvFiles.Items)
                item.Selected = true;
        }

        #endregion

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog() { Filter = "All files|*.*", Multiselect = true })
            {
                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                foreach (var file in openFileDialog.FileNames)
                {
                    lvFiles.Items.AddItemSub(Path.GetFileName(file), Common.FormatBytes(new FileInfo(file).Length), file);
                }
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            using (var openFolderDialog = new OpenFolderDialog())
            {
                if (openFolderDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                foreach (var file in new DirectoryInfo(openFolderDialog.Folder).GetFiles("*.*", SearchOption.AllDirectories))
                {
                    string path = file.FullName.Replace(Path.GetDirectoryName(openFolderDialog.Folder), "").TrimStart(new char[] { '\\', '/' });

                    lvFiles.Items.AddItemSub(path, Common.FormatBytes(file.Length), file.FullName);
                }
            }
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK)
                return;

            var files = new List<FileRecord>(lvFiles.Items
                .Cast<ListViewItem>()
                .Select(x => new FileRecord(x.Text, x.SubItems[2].Text)));

            gbActions.Enabled = gbActions.Visible = false;
            gbProgress.Enabled = gbProgress.Visible = true;
            progressBar1.Maximum = files.Count;

            backgroundWorker1.RunWorkerAsync(new object[] { saveFileDialog1.FileName, files, SelectedCompression });
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var output = (string)(e.Argument as object[])[0];
            var files = (List<FileRecord>)(e.Argument as object[])[1];
            var compression = (CompressionLevel)(e.Argument as object[])[2];

            using (var binaryWriter = new BinaryWriter(new FileStream(output, FileMode.Create)))
            {
                // Write archive header
                binaryWriter.Write("BTDX".ToCharArray()); // magic
                binaryWriter.Write((uint)1); // version
                binaryWriter.Write("GNRL".ToCharArray()); // type
                binaryWriter.Write((uint)files.Count); // numFiles
                binaryWriter.Write((ulong)0); // nameTableOffset

                // Write file record headers
                foreach (FileRecord entry in files)
                {
                    binaryWriter.Write(entry.NameHash); // nameHash
                    binaryWriter.Write(entry.Extension); // extension
                    binaryWriter.Write(CRC32.Compute(Path.GetDirectoryName(entry.Name).ToLower())); // dirHash
                    binaryWriter.Write(entry.Flags); // flags
                    binaryWriter.Write((ulong)0); // offset
                    binaryWriter.Write((uint)0); // size
                    binaryWriter.Write(entry.RealSize); // realSize
                    binaryWriter.Write(entry.Align); // align
                }

                foreach (TextureRecord record in new TextureRecord[] { null, null, null }) // ToDo
                {
                    using (var fs = new BinaryReader(File.OpenRead(record.FullPath)))
                    {
                        fs.BaseStream.Seek(12, SeekOrigin.Begin);
                        record.height = fs.ReadUInt16();
                        record.width = fs.ReadUInt16();
                        fs.BaseStream.Seek(8, SeekOrigin.Current);
                        record.numMips = fs.ReadByte();

                        fs.BaseStream.Seek(84, SeekOrigin.Begin);
                        uint fourCC = fs.ReadUInt32();

                        record.chunks = new TextureRecord.Chunk[1];
                        record.chunks[0] = new TextureRecord.Chunk()
                        {
                            offset = 0,
                            packSz = 0,
                            fullSz = fourCC == FOURCC_DXT10 ?
                                (uint)fs.BaseStream.Length - DDS_HEADER_DXT10_SIZE :
                                (uint)fs.BaseStream.Length - DDS_HEADER_SIZE,
                            startMip = 0,
                            endMip = (ushort)(record.numMips - 1)
                        };
                    }

                    binaryWriter.Write(record.nameHash);
                    binaryWriter.Write(record.extension);
                    binaryWriter.Write(record.dirHash);
                    binaryWriter.Write(record.unk8);
                    binaryWriter.Write((byte)1); // ToDo: Figure out when to use more chunks
                    binaryWriter.Write(record.chunkHdrLen);
                    binaryWriter.Write(record.height);
                    binaryWriter.Write(record.width);
                    binaryWriter.Write(record.numMips);
                    // format: Check DXGI_FORMAT enum for possible values
                    binaryWriter.Write(record.unk16);


                }

                // Compress and write files into archive
                for (int i = 0; i < files.Count; i++)
                {
                    using (var ms = new MemoryStream())
                    {
                        files[i].Offset = (ulong)binaryWriter.BaseStream.Position;

                        using (var fs = File.OpenRead(files[i].FilePath))
                        using (var compressor = new ZlibStream(ms, CompressionMode.Compress, compression, true))
                            fs.CopyTo(compressor);

                        ms.WriteTo(binaryWriter.BaseStream);

                        files[i].FileSize = (uint)ms.Length;
                    }

                    backgroundWorker1.ReportProgress(0);
                }

                // Store name table offset
                long nameTableOffset = binaryWriter.BaseStream.Position;

                foreach (FileRecord entry in files)
                {
                    string name = entry.Name.Replace('\\', '/');
                    byte[] bytes = Encoding.Default.GetBytes(name);
                    binaryWriter.Write((ushort)bytes.Length);
                    binaryWriter.Write(bytes);
                }

                // Go back and write name table offset in header
                binaryWriter.BaseStream.Position = 16;
                binaryWriter.Write((ulong)nameTableOffset);

                // Go to first offset for first file
                binaryWriter.BaseStream.Position += 16;

                for (int i = 0; i < files.Count; i++)
                {
                    binaryWriter.Write(files[i].Offset);
                    binaryWriter.Write(files[i].FileSize);
                    binaryWriter.BaseStream.Position += 24;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                default:
                    progressBar1.PerformStep();
                    break;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gbActions.Enabled = gbActions.Visible = true;
            gbProgress.Enabled = gbProgress.Visible = false;
            progressBar1.Value = 0;
        }

        /// <summary>
        /// Returns only the part that exists in every path in '<paramref name="paths"/>' parameter.
        /// </summary>
        /// <param name="paths">List of paths to compare.</param>
        private string GetCommonPath(string[] paths)
        {
            var matchedWords = new string[paths.Length][];
            for (int i = 0; i < matchedWords.Length; i++)
            {
                var words = paths[i].Split(Path.DirectorySeparatorChar);
                matchedWords[i] = words;
            }

            // Only check the minimum amount of words. Any extra words doesn't match therefore ignored
            int columns = matchedWords.Select(arr => arr.Length).Min();
            var parts = new List<string>();

            for (int col = 0; col < columns; col++)
            {
                string word = matchedWords[0][col];

                if (matchedWords.All(arr => arr[col] == word))
                    parts.Add(word);
                else
                    break; // Stop after first miss
            }

            return string.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }

        /// <summary>
        /// Splits path into every possible variations.
        /// </summary>
        private string[] SplitPath(string path)
        {
            var results = new List<string>();
            var parts = path.Split(Path.DirectorySeparatorChar);

            for (int i = parts.Length - 1; i > -1; i--)
            {
                var s = new string[i + 1];
                Array.Copy(parts, s, i + 1);
                results.Add(string.Join(Path.DirectorySeparatorChar.ToString(), s));
            }

            return results.ToArray();
        }
    }
}
