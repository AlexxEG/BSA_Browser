using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpBSABA2;
using SharpBSABA2.Enums;

namespace BSA_Browser
{
    public struct CompareItem
    {
        public string FullPath;
        public CompareType Type;

        public CompareItem(string fullPath, CompareType type)
        {
            this.FullPath = fullPath;
            this.Type = type;
        }
    }

    public enum CompareType
    {
        Added,
        Removed,
        Changed,
        Identical
    }

    public partial class CompareForm : Form
    {
        string CompareTextTemplate = string.Empty;

        public List<Archive> Archives { get; private set; } = new List<Archive>();
        public List<CompareItem> Files { get; private set; } = new List<CompareItem>();

        public CompareForm()
        {
            InitializeComponent();

            CompareTextTemplate = this.lComparison.Text;
        }

        public CompareForm(ICollection<Archive> archives)
            : this()
        {
            if (archives?.Count > 0)
                this.Archives.AddRange(archives);
        }

        private void CompareForm_Load(object sender, EventArgs e)
        {
            foreach (var archive in this.Archives)
            {
                cbArchiveA.Items.Add(archive.FileName);
                cbArchiveB.Items.Add(archive.FileName);
            }

            this.lComparison.Text = string.Format(CompareTextTemplate, 0, 0, 0, 0, 0);
        }

        private void cbArchives_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;

            var controls = new Dictionary<string, Control>()
            {
                { "type", sender == cbArchiveA ? lTypeA : lTypeB },
                { "version", sender == cbArchiveA ? lVersionA : lVersionB },
                { "fileCount", sender == cbArchiveA ? lFileCountA : lFileCountB },
                { "chunks", sender == cbArchiveA ? lChunksA : lChunksB },
                { "chunksLabel", sender == cbArchiveA ? lChunksAA : lChunksBB },
                { "missingNameTable", sender == cbArchiveA ? lMissingNameTableA : lMissingNameTableB }
            };

            if (comboBox.SelectedIndex < 0)
            {
                controls["type"].Text = "-";
                controls["version"].Text = "-";
                controls["fileCount"].Text = "-";
                controls["chunks"].Text = "-";
                controls["chunks"].Visible = controls["chunksLabel"].Visible = false;
                controls["missingNameTable"].Visible = false;
            }
            else
            {
                var archive = this.Archives[comboBox.SelectedIndex];
                controls["type"].Text = this.FormatType(archive.Type);
                controls["version"].Text = archive.VersionString;
                controls["fileCount"].Text = archive.FileCount.ToString();
                controls["chunks"].Text = archive.Chunks.ToString();
                controls["chunks"].Visible = controls["chunksLabel"].Visible = archive.Chunks > 0;
                controls["missingNameTable"].Visible = !archive.HasNameTable;
            }

            this.Compare();
        }

        private void lvArchive_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (this.Files.Count <= e.ItemIndex)
                return;

            var file = this.Files[e.ItemIndex];
            ListViewItem newItem;

            switch (file.Type)
            {
                case CompareType.Added:
                    newItem = new ListViewItem
                    {
                        UseItemStyleForSubItems = false
                    };
                    newItem.SubItems.Add(new ListViewItem.ListViewSubItem(newItem, file.FullPath)
                    {
                        ForeColor = Color.Green
                    });
                    break;
                case CompareType.Removed:
                    newItem = new ListViewItem(file.FullPath)
                    {
                        ForeColor = Color.Red
                    };
                    newItem.SubItems.Add(new ListViewItem.ListViewSubItem());
                    break;
                case CompareType.Changed:
                    newItem = new ListViewItem(file.FullPath)
                    {
                        ForeColor = Color.Blue
                    };
                    newItem.SubItems.Add(new ListViewItem.ListViewSubItem(newItem, file.FullPath));
                    break;
                case CompareType.Identical:
                    newItem = new ListViewItem(file.FullPath)
                    {
                        ForeColor = Color.Gray
                    };
                    newItem.SubItems.Add(new ListViewItem.ListViewSubItem(newItem, file.FullPath));
                    break;
                default:
                    throw new Exception("Unknown CompareType");
            }

            newItem.ToolTipText = file.FullPath;

            e.Item = newItem;
        }

        private void Compare()
        {
            if (cbArchiveA.SelectedIndex < 0 || cbArchiveB.SelectedIndex < 0)
                return;

            if (cbArchiveA.SelectedIndex == cbArchiveB.SelectedIndex)
                return;

            var archA = this.Archives[cbArchiveA.SelectedIndex];
            var archB = this.Archives[cbArchiveB.SelectedIndex];

            // Type
            lTypeA.ForeColor = archA.Type != archB.Type ? Color.Red : SystemColors.ControlText;
            lTypeB.ForeColor = archA.Type != archB.Type ? Color.Green : SystemColors.ControlText;

            // Version
            lVersionA.ForeColor = archA.VersionString != archB.VersionString ? Color.Red : SystemColors.ControlText;
            lVersionB.ForeColor = archB.VersionString != archB.VersionString ? Color.Green : SystemColors.ControlText;

            // File Count
            lFileCountA.ForeColor = archA.FileCount != archB.FileCount ? Color.Red : SystemColors.ControlText;
            lFileCountB.ForeColor = archB.FileCount != archB.FileCount ? Color.Green : SystemColors.ControlText;

            // Chunks
            lChunksA.ForeColor = archA.Chunks != archB.Chunks ? Color.Red : SystemColors.ControlText;
            lChunksB.ForeColor = archB.Chunks != archB.Chunks ? Color.Green : SystemColors.ControlText;

            var archAFileList = archA.Files.ToDictionary(x => x.FullPath);
            var archBFileList = archB.Files.ToDictionary(x => x.FullPath);

            // Merge file list, ignore duplicates
            var dict = archAFileList.Keys.ToDictionary(x => x);
            foreach (var file in archBFileList.Keys) dict[file] = file;
            var filelist = dict.Values.ToList();

            lvArchive.BeginUpdate();
            this.Files.Clear();

            foreach (var file in filelist)
            {
                if (archAFileList.ContainsKey(file) && !archBFileList.ContainsKey(file))
                {
                    // File appears in left archive only
                    this.Files.Add(new CompareItem(archAFileList[file].FullPath, CompareType.Removed));
                }
                else if (!archAFileList.ContainsKey(file) && archBFileList.ContainsKey(file))
                {
                    // File appears in right archive only
                    this.Files.Add(new CompareItem(archBFileList[file].FullPath, CompareType.Added));
                }
                else
                {
                    byte[] a = archAFileList[file].GetRawDataStream().ToArray();
                    byte[] b = archBFileList[file].GetRawDataStream().ToArray();

                    // Compare bytes
                    if (UnsafeCompare(a, b))
                    {
                        // Files are identical
                        this.Files.Add(new CompareItem(archAFileList[file].FullPath, CompareType.Identical));
                    }
                    else
                    {
                        // Files are different
                        this.Files.Add(new CompareItem(archAFileList[file].FullPath, CompareType.Changed));
                    }
                }
            }

            lvArchive.VirtualListSize = this.Files.Count;
            lvArchive.Invalidate();
            lvArchive.EndUpdate();

            this.lComparison.Text = string.Format(CompareTextTemplate,
                this.Files.Count(x => x.Type == CompareType.Added),
                this.Files.Count(x => x.Type == CompareType.Removed),
                this.Files.Count(x => x.Type == CompareType.Changed),
                archAFileList.Count,
                archBFileList.Count);
        }

        public void Compare(Archive archA, Archive archB)
        {
            cbArchiveA.SelectedIndexChanged -= cbArchives_SelectedIndexChanged;

            cbArchiveA.SelectedIndex = this.Archives.IndexOf(archA);
            cbArchiveB.SelectedIndex = this.Archives.IndexOf(archB);

            cbArchiveA.SelectedIndexChanged += cbArchives_SelectedIndexChanged;
        }

        public void AddArchive(Archive archive)
        {
            this.Archives.Add(archive);

            cbArchiveA.Items.Add(archive.FileName);
            cbArchiveB.Items.Add(archive.FileName);
        }

        public void RemoveArchive(Archive archive)
        {
            this.Archives.Remove(archive);

            cbArchiveA.Items.Remove(archive.FileName);
            cbArchiveB.Items.Remove(archive.FileName);
        }

        public string FormatType(ArchiveTypes type)
        {
            switch (type)
            {
                case ArchiveTypes.BA2_DX10: return "BA2 Texture";
                case ArchiveTypes.BA2_GNMF: return "BA2 Texture (GNF)";
                case ArchiveTypes.BA2_GNRL: return "BA2 General";
                case ArchiveTypes.BSA: return "BSA";
                case ArchiveTypes.BSA_MW: return "BSA Morrowind";
                case ArchiveTypes.BSA_SE: return "BSA Special Edition";
                case ArchiveTypes.DAT_F2: return "DAT Fallout 2";
                default: return string.Empty;
            }
        }

        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        public static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == a2) return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }
    }
}
