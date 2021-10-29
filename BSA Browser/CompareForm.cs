using BSA_Browser.Classes;
using BSA_Browser.Extensions;
using BSA_Browser.Properties;
using BSA_Browser.Sorting;
using SharpBSABA2;
using SharpBSABA2.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        Removed = 1,
        Added = 2,
        Changed = 3,
        Identical = 4
    }

    public partial class CompareForm : Form
    {
        string LabelAddedTextTemplate = string.Empty;
        string LabelRemovedTextTemplate = string.Empty;
        string LabelChangedTextTemplate = string.Empty;
        string FormTextOriginal;

        public List<Archive> Archives { get; private set; } = new List<Archive>();
        public List<CompareItem> Files { get; private set; } = new List<CompareItem>();
        public List<CompareItem> FilteredFiles { get; private set; } = new List<CompareItem>();

        private NaturalStringComparer NaturalStringComparer = new NaturalStringComparer();

        private CancellationTokenSource cancellationTokenSource;

        public CompareForm()
        {
            InitializeComponent();

            this.Menu = mainMenu1;
            directoryTreeMenuItem.Checked = Settings.Default.CompareDirectoryTree;
            splitContainer1.Panel1Collapsed = !Settings.Default.CompareDirectoryTree;

            lvArchive.ContextMenu = contextMenu1;
            lvArchive.EnableVisualStyles();
            tvDirectories.EnableVisualStyles();

            LabelAddedTextTemplate = lAdded.Text;
            LabelRemovedTextTemplate = lRemoved.Text;
            LabelChangedTextTemplate = lChanged.Text;
            FormTextOriginal = this.Text;

            chbFilterUnique.Checked = Settings.Default.CompareFilterUnique;
            chbFilterChanged.Checked = Settings.Default.CompareFilterDifferent;
            chbFilterIdentical.Checked = Settings.Default.CompareFilterIdentical;
        }

        public CompareForm(ICollection<Archive> archives)
            : this()
        {
            if (archives?.Count > 0)
                this.Archives.AddRange(archives);
        }

        private void CompareForm_Load(object sender, EventArgs e)
        {
            Settings.Default.WindowStates.Restore(this, false, restoreColumns: false);

            foreach (var archive in this.Archives)
            {
                cbArchiveA.Items.Add(archive.FileName);
                cbArchiveB.Items.Add(archive.FileName);
            }

            lAdded.Text = string.Format(LabelAddedTextTemplate, 0);
            lRemoved.Text = string.Format(LabelRemovedTextTemplate, 0);
            lChanged.Text = string.Format(LabelChangedTextTemplate, 0);
        }

        private void CompareForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            Settings.Default.WindowStates.Save(this, saveColumns: false);
        }

        private async void cbArchives_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            var type = sender == cbArchiveA ? lTypeA : lTypeB;
            var version = sender == cbArchiveA ? lVersionA : lVersionB;
            var fileCount = sender == cbArchiveA ? lFileCountA : lFileCountB;
            var chunks = sender == cbArchiveA ? lChunksA : lChunksB;
            var chunksLabel = sender == cbArchiveA ? lChunksAA : lChunksBB;
            var missingNameTable = sender == cbArchiveA ? lNoNameTableA : lNoNameTableB;

            if (comboBox.SelectedIndex < 0)
            {
                // Reset text and visibility
                type.Text = version.Text = fileCount.Text = chunks.Text = "-";
                chunks.Visible = chunksLabel.Visible = missingNameTable.Visible = false;
            }
            else
            {
                var archive = this.Archives[comboBox.SelectedIndex];
                type.Text = this.FormatType(archive.Type);
                version.Text = archive.VersionString;
                fileCount.Text = archive.FileCount.ToString();
                chunks.Text = archive.Chunks.ToString();
                chunks.Visible = chunksLabel.Visible = archive.Chunks > 0;
                missingNameTable.Visible = !archive.HasNameTable;
            }

            // Checks
            if (cbArchiveA.SelectedIndex < 0 || cbArchiveB.SelectedIndex < 0)
                return;

            if (cbArchiveA.SelectedIndex == cbArchiveB.SelectedIndex)
            {
                // Same archive, don't compare anything but still show info and files
                this.CompareSameArchive();
                return;
            }

            var archA = this.Archives[cbArchiveA.SelectedIndex];
            var archB = this.Archives[cbArchiveB.SelectedIndex];

            // Set colors
            this.SetCompareColor(lTypeA, lTypeB, archA.Type != archB.Type);
            this.SetCompareColor(lVersionA, lVersionB, archA.VersionString != archB.VersionString);
            this.SetCompareColor(lFileCountA, lFileCountB, archA.FileCount != archB.FileCount);
            this.SetCompareColor(lChunksA, lChunksB, archA.Chunks != archB.Chunks);

            cbArchiveA.Enabled = cbArchiveB.Enabled = tvDirectories.Enabled = lvArchive.Enabled = false;
            lvArchive.BeginUpdate();

            try
            {
                cancellationTokenSource = new CancellationTokenSource();

                await this.CompareAsync(archA, archB, new Progress<int>(progress =>
                {
                    this.Text = $"{FormTextOriginal} - {progress}%";
                }), cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Should only be canceled during Form closing event, everything underneath will already be disposed
                return;
            }

            this.Text = FormTextOriginal;

            lvArchive.VirtualListSize = this.FilteredFiles.Count;
            lvArchive.Invalidate();
            lvArchive.EndUpdate();

            int added = 0, removed = 0, changed = 0;

            foreach (var ct in this.Files)
                if (ct.Type == CompareType.Added)
                    added++;
                else if (ct.Type == CompareType.Removed)
                    removed++;
                else if (ct.Type == CompareType.Changed)
                    changed++;

            lAdded.Text = string.Format(LabelAddedTextTemplate, added);
            lRemoved.Text = string.Format(LabelRemovedTextTemplate, removed);
            lChanged.Text = string.Format(LabelChangedTextTemplate, changed);

            if (Settings.Default.CompareDirectoryTree)
                this.BuildFolderTreeView(archA.Files.Select(x => x.Folder).Union(archB.Files.Select(x => x.Folder)));

            cbArchiveA.Enabled = cbArchiveB.Enabled = tvDirectories.Enabled = lvArchive.Enabled = true;
        }

        private void tvDirectories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lvArchive.BeginUpdate();
            this.Filter(e.Node.FullPath);
            lvArchive.VirtualListSize = this.FilteredFiles.Count;
            lvArchive.Invalidate();
            lvArchive.EndUpdate();
        }

        private void lvArchive_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvArchive.Columns[e.ColumnIndex].Width;
        }

        private void lvArchive_Resize(object sender, EventArgs e)
        {
            lvArchive.Columns[0].Width = lvArchive.Size.Width / 2 - 12;
            lvArchive.Columns[1].Width = lvArchive.Size.Width / 2 - 12;
        }

        private void lvArchive_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (this.FilteredFiles.Count <= e.ItemIndex)
                return;

            var file = this.FilteredFiles[e.ItemIndex];

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
                    throw new Exception($"Unknown {nameof(CompareType)} value: {(int)file.Type}.");
            }

            newItem.ToolTipText = file.FullPath;

            e.Item = newItem;
        }

        private void lFilters_CheckedChanged(object sender, EventArgs e)
        {
            lvArchive.BeginUpdate();
            this.Filter(tvDirectories.SelectedNode != null ? tvDirectories.SelectedNode.FullPath : string.Empty);
            lvArchive.VirtualListSize = this.FilteredFiles.Count;
            lvArchive.Invalidate();
            lvArchive.EndUpdate();

            Settings.Default.CompareFilterUnique = chbFilterUnique.Checked;
            Settings.Default.CompareFilterDifferent = chbFilterChanged.Checked;
            Settings.Default.CompareFilterIdentical = chbFilterIdentical.Checked;
        }

        #region mainMenu1

        private void directoryTreeMenuItem_Click(object sender, EventArgs e)
        {
            directoryTreeMenuItem.Checked = !directoryTreeMenuItem.Checked;
            splitContainer1.Panel1Collapsed = !directoryTreeMenuItem.Checked;
            Settings.Default.CompareDirectoryTree = directoryTreeMenuItem.Checked;

            if (Settings.Default.CompareDirectoryTree && tvDirectories.Nodes.Count == 0)
            {
                this.BuildFolderTreeView(Files.Select(x => Path.GetDirectoryName(x.FullPath)).Distinct());
            }
            else
            {
                // Need to clear so that if archives are changed while setting is disabled a rebuild will be triggered
                tvDirectories.Nodes.Clear();
            }
        }

        #endregion

        #region contextMenu1

        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            extractLeftMenuItem.Enabled = cbArchiveA.SelectedIndex != -1;
            extractRightMenuItem.Enabled = cbArchiveB.SelectedIndex != -1;

            if (lvArchive.SelectedIndices.Count == 0)
            {
                extractSelectedLeftMenuItem.Enabled =
                    extractSelectedRightMenuItem.Enabled =
                    previewLeftMenuItem.Enabled =
                    previewRightMenuItem.Enabled = false;
                return;
            }

            var compareItem = this.FilteredFiles[lvArchive.SelectedIndices[0]];
            var selectedItems = this.FilteredFiles.Where((x, index) => lvArchive.SelectedIndices.Contains(index));
            bool added = false, removed = false, uniqueOrIdentical = false;

            foreach (var item in selectedItems)
            {
                if (item.Type == CompareType.Added)
                    added = true;
                else if (item.Type == CompareType.Removed)
                    removed = true;
                else if (item.Type == CompareType.Changed || item.Type == CompareType.Identical)
                    uniqueOrIdentical = true;

                if ((added && removed) || uniqueOrIdentical)
                    break;
            }

            extractSelectedLeftMenuItem.Enabled = removed || uniqueOrIdentical;
            previewLeftMenuItem.Enabled = lvArchive.SelectedIndices.Count == 1 && (removed || uniqueOrIdentical);

            extractSelectedRightMenuItem.Enabled = added || uniqueOrIdentical;
            previewRightMenuItem.Enabled = lvArchive.SelectedIndices.Count == 1 && (added || uniqueOrIdentical);
        }

        private void extractLeftMenuItem_Click(object sender, EventArgs e)
        {
            this.ExtractFiles(true, FilteredFiles.Where(x => x.Type != CompareType.Added));
        }

        private void extractRightMenuItem_Click(object sender, EventArgs e)
        {
            this.ExtractFiles(false, FilteredFiles.Where(x => x.Type != CompareType.Removed));
        }

        private void extractSelectedLeftMenuItem_Click(object sender, EventArgs e)
        {
            this.ExtractFiles(true,
                FilteredFiles.Where((x, index) => lvArchive.SelectedIndices.Contains(index) && x.Type != CompareType.Added));
        }

        private void extractSelectedRightMenuItem_Click(object sender, EventArgs e)
        {
            this.ExtractFiles(false,
                FilteredFiles.Where((x, index) => lvArchive.SelectedIndices.Contains(index) && x.Type != CompareType.Removed));
        }

        private void previewLeftMenuItem_Click(object sender, EventArgs e)
        {
            this.PreviewSelected(true);
        }

        private void previewRightMenuItem_Click(object sender, EventArgs e)
        {
            this.PreviewSelected(false);
        }

        #endregion

        private void Filter(string subFolder = "")
        {
            this.FilteredFiles.Clear();

            var types = GetFilteredTypes();

            foreach (var file in Files)
            {
                if (!string.IsNullOrEmpty(subFolder))
                {
                    if (!file.FullPath.StartsWith(subFolder, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                if (types.Contains(file.Type))
                    this.FilteredFiles.Add(file);
            }
        }

        private void BuildFolderTreeView(IEnumerable<string> folders)
        {
            tvDirectories.Nodes.Clear();
            foreach (var path in folders)
            {
                string subPathAgg = string.Empty;
                TreeNode lastNode = null;
                foreach (string subPath in path.Split('\\'))
                {
                    subPathAgg += subPath + '\\';
                    var nodes = tvDirectories.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                        if (lastNode == null)
                            lastNode = tvDirectories.Nodes.Add(subPathAgg, subPath);
                        else
                            lastNode = lastNode.Nodes.Add(subPathAgg, subPath);
                    else
                        lastNode = nodes[0];
                }
            }
        }

        private async Task CompareAsync(Archive archA, Archive archB, IProgress<int> progress, CancellationToken token)
        {
            var archAFileList = archA.Files.ToDictionary(x => x.FullPath.ToLower());
            var archBFileList = archB.Files.ToDictionary(x => x.FullPath.ToLower());

            // Merge file list, ignore duplicates
            var dict = archAFileList.Keys.ToDictionary(x => x);
            foreach (var file in archBFileList.Keys) dict[file] = file;
            var filelist = dict.Values.ToList();

            await Task.Run(() =>
            {
                using (var epA = archA.CreateSharedParams(true, false))
                using (var epB = archB.CreateSharedParams(true, false))
                {
                    int count = 0;
                    int prevPercentage = 0;

                    this.Files.Clear();

                    foreach (var file in filelist)
                    {
                        token.ThrowIfCancellationRequested();

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
                            epA.Reader.BaseStream.Position = (long)archAFileList[file].Offset;
                            epB.Reader.BaseStream.Position = (long)archBFileList[file].Offset;

                            if (archAFileList[file].GetSizeInArchive(epA) == archBFileList[file].GetSizeInArchive(epB)
                                && CompareStreams(epA.Reader.BaseStream, epB.Reader.BaseStream, archAFileList[file].GetSizeInArchive(epA)))
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

                        count++;
                        int newPercentage = (int)Math.Round((double)count / (double)filelist.Count * 100);
                        if (newPercentage != prevPercentage)
                        {
                            progress.Report(prevPercentage = newPercentage);
                        }
                    }
                }

                this.Files.Sort((x, y) =>
                {
                    int comparison = ((int)x.Type).CompareTo((int)y.Type);
                    if (comparison != 0)
                        return comparison;

                    return NaturalStringComparer.Compare(x.FullPath, y.FullPath);
                });

                this.Filter();
            }, token);
        }

        private void CompareSameArchive()
        {
            SetCompareColor(lTypeA, lTypeB, false);
            SetCompareColor(lVersionA, lVersionB, false);
            SetCompareColor(lFileCountA, lFileCountB, false);
            SetCompareColor(lChunksA, lChunksB, false);

            lvArchive.BeginUpdate();
            this.Files.Clear();

            var archive = this.Archives[cbArchiveA.SelectedIndex];
            foreach (var file in archive.Files)
            {
                // Files are identical
                this.Files.Add(new CompareItem(file.FullPath, CompareType.Identical));
            }

            this.Files.Sort((x, y) =>
            {
                return NaturalStringComparer.Compare(x.FullPath, y.FullPath);
            });

            this.Filter();
            lvArchive.VirtualListSize = this.FilteredFiles.Count;
            lvArchive.Invalidate();
            lvArchive.EndUpdate();

            lAdded.Text = string.Format(LabelAddedTextTemplate, this.Files.Count(x => x.Type == CompareType.Added));
            lRemoved.Text = string.Format(LabelRemovedTextTemplate, this.Files.Count(x => x.Type == CompareType.Removed));
            lChanged.Text = string.Format(LabelChangedTextTemplate, this.Files.Count(x => x.Type == CompareType.Changed));

            if (Settings.Default.CompareDirectoryTree)
                this.BuildFolderTreeView(archive.Files.Select(x => x.Folder).Distinct()); // Distinct helps performance
        }

        private bool CompareStreams(Stream a, Stream b, ulong length)
        {
            int Buffer = 1024 * 10;

            byte[] one = new byte[Buffer];
            byte[] two = new byte[Buffer];
            ulong read = 0;

            while (true)
            {
                // Check if read will be more than length
                if (read + (ulong)Buffer > length)
                {
                    // Reduce buffer by the difference
                    var reduce = (read + (ulong)Buffer) - length;
                    Buffer -= (int)reduce;
                }

                int len1 = a.Read(one, 0, Buffer);
                int len2 = b.Read(two, 0, Buffer);
                int index = 0;

                read += (ulong)len1;

                while (index < len1 && index < len2)
                {
                    if (one[index] != two[index]) return false;
                    index++;
                }
                if (read == length) break; // Max length reached
            }
            return true;
        }

        private void ExtractFiles(bool left, IEnumerable<CompareItem> items)
        {
            var archive = this.Archives[(left ? cbArchiveA : cbArchiveB).SelectedIndex];
            var fes = new List<ArchiveEntry>();

            foreach (var item in items)
                fes.Add(archive.Files.Find(x => x.FullPath.ToLower() == item.FullPath.ToLower()));

            using (var ofd = new OpenFolderDialog())
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    BSABrowser.ExtractFiles(this,
                        ofd.Folder,
                        true,
                        true,
                        fes,
                        titleProgress: true);
                }
            }
        }

        private void PreviewSelected(bool left)
        {
            var compareItem = this.FilteredFiles[lvArchive.SelectedIndices[0]];
            var archive = left ? this.Archives[cbArchiveA.SelectedIndex] : this.Archives[cbArchiveB.SelectedIndex];
            var entry = archive.Files.Find(x => x.FullPath.ToLower() == compareItem.FullPath.ToLower());

            if (entry == null)
            {
                MessageBox.Show(this, $"File '{compareItem.FullPath}' couldn't be found.");
                return;
            }

            Common.PreviewTexture(this, entry);
        }

        private void SetCompareColor(Control a, Control b, bool comparison)
        {
            a.ForeColor = comparison ? Color.Red : SystemColors.ControlText;
            b.ForeColor = comparison ? Color.Green : SystemColors.ControlText;
        }

        private string FormatType(ArchiveTypes type)
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

        private CompareType[] GetFilteredTypes()
        {
            var types = new List<CompareType>();
            if (chbFilterUnique.Checked)
            {
                types.Add(CompareType.Added);
                types.Add(CompareType.Removed);
            }
            if (chbFilterChanged.Checked)
                types.Add(CompareType.Changed);
            if (chbFilterIdentical.Checked)
                types.Add(CompareType.Identical);

            return types.ToArray();
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
    }
}
