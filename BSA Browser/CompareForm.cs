using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpBSABA2;
using SharpBSABA2.Enums;

namespace BSA_Browser
{
    public partial class CompareForm : Form
    {
        string CompareTextTemplate = string.Empty;

        public List<Archive> Archives { get; private set; } = new List<Archive>();

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
            ComboBox comboBox = sender as ComboBox;
            Label label = sender == cbArchiveA ? lTypeA : lTypeB;

            if (comboBox.SelectedIndex < 0)
                label.Text = "-";
            else
                label.Text = this.FormatType(this.Archives[comboBox.SelectedIndex].Type);

            this.Compare();
        }

        private void Compare()
        {
            lvArchiveA.Items.Clear();

            if (cbArchiveA.SelectedIndex < 0 || cbArchiveB.SelectedIndex < 0)
                return;

            if (cbArchiveA.SelectedIndex == cbArchiveB.SelectedIndex)
                return;

            var archA = this.Archives[cbArchiveA.SelectedIndex];
            var archB = this.Archives[cbArchiveB.SelectedIndex];

            lTypeA.ForeColor = archA.Type != archB.Type ? Color.Red : SystemColors.ControlText;
            lTypeB.ForeColor = archA.Type != archB.Type ? Color.Green : SystemColors.ControlText;

            var archAFileList = archA.Files.ToDictionary(x => x.FullPath);
            var archBFileList = archB.Files.ToDictionary(x => x.FullPath);

            // Merge file list, ignore duplicates
            var dict = archAFileList.Keys.ToDictionary(x => x);
            foreach (var file in archBFileList.Keys) dict[file] = file;
            var filelist = dict.Values.ToList();

            var added = new List<ArchiveEntry>();
            var removed = new List<ArchiveEntry>();
            var changed = new List<ArchiveEntry>();
            var identical = new List<ArchiveEntry>();

            foreach (var file in filelist)
            {
                if (archAFileList.ContainsKey(file) && !archBFileList.ContainsKey(file))
                {
                    removed.Add(archAFileList[file]);
                }
                else if (!archAFileList.ContainsKey(file) && archBFileList.ContainsKey(file))
                {
                    added.Add(archBFileList[file]);
                }
                else
                {
                    byte[] a = archAFileList[file].GetRawDataStream().ToArray();
                    byte[] b = archBFileList[file].GetRawDataStream().ToArray();

                    if (UnsafeCompare(a, b))
                    {
                        identical.Add(archAFileList[file]);
                    }
                    else
                    {
                        changed.Add(archAFileList[file]);
                    }
                }
            }

            foreach (var entry in added)
            {
                var item = new ListViewItem();
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
                    entry.FullPath,
                    Color.Green,
                    Color.Transparent,
                    lvArchiveA.Font));
                lvArchiveA.Items.Add(item);
            }

            foreach (var entry in removed)
            {
                var item = new ListViewItem(entry.FullPath);
                item.ForeColor = Color.Red;
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
                    string.Empty,
                    Color.Red,
                    Color.Transparent,
                    lvArchiveA.Font));
                lvArchiveA.Items.Add(item);
            }

            foreach (var entry in changed)
            {
                var item = new ListViewItem(entry.FullPath);
                item.ForeColor = Color.Blue;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, entry.FullPath));
                lvArchiveA.Items.Add(item);
            }

            foreach (var entry in identical)
            {
                var item = new ListViewItem(entry.FullPath);
                item.ForeColor = Color.Gray;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, entry.FullPath));
                lvArchiveA.Items.Add(item);
            }

            this.lComparison.Text = string.Format(CompareTextTemplate,
                added.Count,
                removed.Count,
                changed.Count,
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
