using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BSA_Browser.Classes;
using BSA_Browser.Controls;
using BSA_Browser.Dialogs;
using BSA_Browser.Extensions;
using BSA_Browser.Properties;
using SharpBSABA2;

namespace BSA_Browser
{
    public enum ArchiveFileSortOrder
    {
        FolderName,
        FileSize,
        FileName,
        FileType
    }

    public partial class BSABrowser : Form
    {
        private const string UpdateMarker = "(!) ";

        string _untouchedTitle;
        OpenFolderDialog _openFolderDialog = new OpenFolderDialog();
        List<ArchiveEntry> _files = new List<ArchiveEntry>();
        ArchiveFileSorter _filesSorter = new ArchiveFileSorter();
        Timer _searchDelayTimer;
        CompareForm _compareForm;

        /// <summary>
        /// Get the selected archive.
        /// </summary>
        private ArchiveNode SelectedArchiveNode
        {
            get
            {
                if (tvFolders.SelectedNode == null)
                    return null;

                return this.GetRootNode(tvFolders.SelectedNode);
            }
        }

        public BSABrowser()
        {
            InitializeComponent();

            // Show application version in title
            this.Text += $" ({Program.GetVersion()})";

            // Store title so it can be restored later,
            // for example when showing the extraction progress in title
            _untouchedTitle = this.Text;

            lvFiles.ContextMenu = contextMenu1;

            if (Settings.Default.UpdateSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                Settings.Default.Save();
            }

            // Restore last path for OpenFolderDialog
            if (!string.IsNullOrEmpty(Settings.Default.LastUnpackPath))
                _openFolderDialog.InitialFolder = Settings.Default.LastUnpackPath;

            // Load Recent Files list
            if (Settings.Default.RecentFiles != null)
            {
                foreach (string item in Settings.Default.RecentFiles)
                    this.AddToRecentFiles(item);
            }

            // Load Quick Extract Paths
            if (Settings.Default.QuickExtractPaths == null)
                Settings.Default.QuickExtractPaths = new QuickExtractPaths();

            this.LoadQuickExtractPaths();

            // Set lvFiles sorter
            ArchiveFileSorter.SetSorter(Settings.Default.SortType, Settings.Default.SortDesc);

            // Enable visual styles
            tvFolders.EnableVisualStyles();
            tvFolders.EnableAutoScroll();

            lvFiles.EnableVisualStyles();
            lvFiles.EnableVisualStylesSelection();
            lvFiles.HideFocusRectangle();

            // Set TextBox cue
            txtSearch.SetCue("Search term...");
        }

        public BSABrowser(string[] args)
            : this()
        {
            this.OpenArchives(true, args);
        }

        private void BSABrowser_Load(object sender, EventArgs e)
        {
            // Initialize WindowStates if null
            if (Settings.Default.WindowStates == null)
            {
                Settings.Default.WindowStates = new WindowStates();
            }

            // Add this form if it doesn't exists
            if (!Settings.Default.WindowStates.Contains(this.Name))
            {
                Settings.Default.WindowStates.Add(this.Name);
            }

            // Restore window state
            Settings.Default.WindowStates[this.Name].RestoreForm(this);

            // Restore sorting preferences
            lvFiles.SetSortIcon(
                (int)Settings.Default.SortType,
                Settings.Default.SortDesc ? SortOrder.Ascending : SortOrder.Descending);

            // Restore Regex preference
            cbRegex.Checked = Settings.Default.SearchUseRegex;

            OpenArchiveDialog.InitialDirectory = Settings.Default.OpenArchiveDialog;

            // Show ! in main menu if update is available
            this.ShowUpdateNotification();
        }

        private void BSABrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tvFolders.GetNodeCount(false) > 0)
                this.CloseArchives();

            this.SaveRecentFiles();

            Settings.Default.WindowStates[this.Name].SaveForm(this);
            Settings.Default.LastUnpackPath = _openFolderDialog.Folder;
            Settings.Default.Save();
        }

        private void File_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                e.Effect = files.All(this.IsSupportedFile) ? DragDropEffects.Link : DragDropEffects.Scroll;
            }
        }

        private void File_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.All(this.IsSupportedFile))
            {
                this.OpenArchives(true, files.Where(this.IsSupportedFile).ToArray());
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (OpenArchiveDialog.ShowDialog(this) == DialogResult.OK)
                this.OpenArchives(true, OpenArchiveDialog.FileNames);
        }

        private void btnExtractFiles_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode != null)
            {
                this.ExtractFilesTo(false, true, () => _files);
            }
        }

        private void btnExtractFolders_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode != null)
            {
                this.ExtractFilesTo(true, true, () => _files);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            this.PreviewSelected();
        }

        private void lvFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var type = (ArchiveFileSortOrder)e.Column;

            if (Settings.Default.SortType == type)
            {
                // Reverse order
                Settings.Default.SortDesc = !Settings.Default.SortDesc;
            }
            else
            {
                Settings.Default.SortType = type;
                Settings.Default.SortDesc = true;
            }

            lvFiles.SetSortIcon(e.Column,
                Settings.Default.SortDesc ? SortOrder.Ascending : SortOrder.Descending);

            this.SortList();
        }

        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            this.PreviewSelected();
        }

        private void lvFiles_Enter(object sender, EventArgs e)
        {
            lvFiles.HideFocusRectangle();
        }

        private void lvFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!(lvFiles.SelectedIndices.Count >= 1))
                return;

            var obj = new DataObject();
            var sc = new StringCollection();

            foreach (int index in lvFiles.SelectedIndices)
            {
                var fe = _files[index];
                string dest = Program.CreateTempDirectory();

                fe.Extract(dest, false);
                sc.Add(Path.Combine(dest, fe.FileName));
            }

            obj.SetFileDropList(sc);
            lvFiles.DoDragDrop(obj, DragDropEffects.Move);
        }

        private void lvFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                lvFiles.SelectAllItems();
            }
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvFiles.HideFocusRectangle();
        }

        private void lvFiles_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_files.Count <= e.ItemIndex)
                return;

            var file = _files[e.ItemIndex];
            var lvi = new ListViewItem(Path.Combine(file.Folder, file.FileName));

            lvi.SubItems.Add(this.FormatBytes(file.DisplaySize));
            lvi.Tag = file;

            e.Item = lvi;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_searchDelayTimer == null)
            {
                _searchDelayTimer = new Timer();
                _searchDelayTimer.Tick += delegate { DoSearch(); };
                _searchDelayTimer.Interval = 500;
            }

            _searchDelayTimer.Stop();
            _searchDelayTimer.Start();
        }

        private void cbRegex_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SearchUseRegex = cbRegex.Checked;
            this.DoSearch();
        }

        private void tvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var rootNode = this.GetRootNode(e.Node);

            // This event only needs to run once, so return if AllFiles is NOT null
            if (rootNode.AllFiles != null)
                return;

            e.Node.Nodes.Clear();
            var nodes = new Dictionary<string, TreeNode>();
            rootNode.AllFiles = new List<ArchiveEntry>(rootNode.Files);
            rootNode.AllFiles.Sort(_filesSorter);

            // This builds the all TreeNodes
            foreach (var lvi in rootNode.AllFiles)
            {
                string path = Path.GetDirectoryName(lvi.FullPath);

                if (path == string.Empty || nodes.ContainsKey(path))
                    continue;

                string[] dirs = path.Split('\\');

                for (int i = 0; i < dirs.Length; i++)
                {
                    string newpath = string.Join("\\", dirs, 0, i + 1);

                    if (!nodes.ContainsKey(newpath))
                    {
                        var tn = new TreeNode(dirs[i]);
                        tn.Tag = newpath;

                        if (i == 0)
                            e.Node.Nodes.Add(tn);
                        else
                            nodes[path].Nodes.Add(tn);

                        nodes.Add(newpath, tn);
                    }
                    path = newpath;
                }
            }

            if (Settings.Default.SortArchiveDirectories)
            {
                this.SortNodes(e.Node);
            }
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var rootNode = this.GetRootNode(e.Node);
            string path = (string)e.Node.Tag;

            // If AllFiles is null, trigger event which will populate it
            if (rootNode.AllFiles == null)
                tvFolders_BeforeExpand(null, new TreeViewCancelEventArgs(e.Node, false, TreeViewAction.Unknown));

            if (path == null) // Root node is selected, so show all files
                rootNode.Files = rootNode.AllFiles.ToArray();
            else
            {
                // Only show files under selected node
                var lvis = new List<ArchiveEntry>(rootNode.AllFiles.Count);

                foreach (var lvi in rootNode.AllFiles)
                    if (lvi.FullPath.StartsWith(path)) lvis.Add(lvi);

                lvis.TrimExcess();
                rootNode.Files = lvis.ToArray();
            }

            lvFiles.ScrollToTop();
            this.DoSearch();
        }

        private void OpenArchiveDialog_FileOk(object sender, CancelEventArgs e)
        {
            OpenArchiveDialog.InitialDirectory = Path.GetDirectoryName(OpenArchiveDialog.FileName);
            Settings.Default.OpenArchiveDialog = OpenArchiveDialog.InitialDirectory;
        }

        #region mainMenu1

        private void fileMenuItem_Popup(object sender, EventArgs e)
        {
            extractArchivesMenuItem.Enabled = tvFolders.Nodes.Count >= 1;
        }

        private void openArchiveMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenArchiveDialog.ShowDialog(this) == DialogResult.OK)
                this.OpenArchives(true, OpenArchiveDialog.FileNames);
        }

        private void closeSelectedArchiveMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SelectedArchiveNode == null)
                return;

            this.CloseArchive(SelectedArchiveNode);
        }

        private void closeAllArchivesMenuItem_Click(object sender, EventArgs e)
        {
            this.CloseArchives();
        }

        private void extractArchivesMenuItem_Click(object sender, EventArgs e)
        {
            var archives = new List<Archive>(tvFolders.Nodes.Cast<ArchiveNode>().Select(x => x.Archive));
            var dialog = ExtractArchivesDialog.ShowDialog(this, archives);

            if (dialog.DialogResult != DialogResult.OK)
                return;

            this.ExtractFilesTo(true, true, () =>
            {
                var files = new List<ArchiveEntry>();

                foreach (Archive archive in dialog.Selected)
                    files.AddRange(archive.Files);

                return files;
            });

            var test = dialog.Selected.Select(x => x.Files);
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (var of = new OptionsForm())
            {
                if (of.ShowDialog(this) == DialogResult.OK)
                {
                    of.SaveChanges();
                    Settings.Default.Save();

                    // Sync changes to UI
                    this.LoadQuickExtractPaths();
                }
            }
        }

        private void recentFilesMenuItem_Popup(object sender, EventArgs e)
        {
            emptyListMenuItem.Enabled = recentFilesMenuItem.MenuItems.Count > 2;
        }

        private void emptyListMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = recentFilesMenuItem.MenuItems.Count - 1; i != 1; i--)
                recentFilesMenuItem.MenuItems.RemoveAt(i);
        }

        private void recentFiles_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            string file = item.Tag.ToString();

            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                this.OpenArchive(file, true);
            }
            else
            {
                if (MessageBox.Show(this,
                        $"\"{file}\" doesn't exist anymore.\n\n" + "Do you want to remove it from the recent files list?",
                        "Lost File",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    recentFilesMenuItem.MenuItems.Remove(item);
                }
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editMenuItem_Popup(object sender, EventArgs e)
        {
            bool hasSelectedItems = lvFiles.SelectedIndices.Count > 0;

            copyMenuItem.Enabled = hasSelectedItems;
        }

        private void copyPathMenuItem_Click(object sender, EventArgs e)
        {
            var builder = new StringBuilder();

            foreach (int index in lvFiles.SelectedIndices)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.AppendLine();

                builder.Append(_files[index].FullPath);
            }

            Clipboard.SetText(builder.ToString());
        }

        private void copyFolderPathMenuItem_Click(object sender, EventArgs e)
        {
            var builder = new StringBuilder();

            foreach (int index in lvFiles.SelectedIndices)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.AppendLine();

                builder.Append(Path.GetDirectoryName(_files[index].FullPath));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void copyFileNameMenuItem_Click(object sender, EventArgs e)
        {
            var builder = new StringBuilder();

            foreach (int index in lvFiles.SelectedIndices)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.AppendLine();

                builder.Append(Path.GetFileName(_files[index].FullPath));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            lvFiles.SelectAllItems();
        }

        private void compareArchivesMenuItem_Click(object sender, EventArgs e)
        {
            var archives = new List<Archive>();

            foreach (ArchiveNode node in tvFolders.Nodes)
                archives.Add(node.Archive);

            if (_compareForm == null || _compareForm.IsDisposed)
                _compareForm = new CompareForm(archives);

            _compareForm.Show();
        }

        private void openFolderMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var path = menuItem.Tag as QuickExtractPath;

            if (!Directory.Exists(path.Path))
            {
                MessageBox.Show(this, $"{path.Name}'s path no longer exists.");
                return;
            }

            Process.Start(path.Path);
        }

        private void helpMenuItem_Popup(object sender, EventArgs e)
        {
            // Remove UpdateMarker from Text
            if (helpMenuItem.Text.StartsWith(UpdateMarker))
                helpMenuItem.Text = helpMenuItem.Text.Remove(0, UpdateMarker.Length);
        }

        private async void checkForUpdateMenuItem_Click(object sender, EventArgs e)
        {
            // Remove UpdateMarker from Text
            if (checkForUpdateMenuItem.Text.StartsWith(UpdateMarker))
                checkForUpdateMenuItem.Text = checkForUpdateMenuItem.Text.Remove(0, UpdateMarker.Length);

            try
            {
                if (await this.IsUpdateAvailable())
                {
                    if (MessageBox.Show(this,
                            "Update available!\n\n" + "Do you want to open the BSA Browser NexusMods page?",
                            "Update available",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start(Program.Website);
                    }
                }
                else
                {
                    MessageBox.Show(this, "You have the latest version.");
                }
            }
            catch (Win32Exception)
            {
                MessageBox.Show(this, "Couldn't open the BSA Browser NexusMods page.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error checking for update.\n\n" + ex.Message);
            }
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            using (var ab = new AboutBox())
            {
                ab.ShowDialog(this);
            }
        }

        #endregion

        #region contextMenu1

        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            bool hasSelectedItems = lvFiles.SelectedIndices.Count > 0;
            bool listIsEmpty = SelectedArchiveNode == null || SelectedArchiveNode.Archive.Files.Count == 0;

            extractMenuItem.Enabled = hasSelectedItems;
            extractFoldersMenuItem.Enabled = hasSelectedItems;

            quickExtractsMenuItem.Enabled = hasSelectedItems;
            copyMenuItem1.Enabled = hasSelectedItems;
        }

        private void extractMenuItem_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedIndices.Count > 0)
            {
                this.ExtractFilesTo(false, true, () => lvFiles.SelectedIndices.Cast<int>().Select(index => _files[index]));
            }
        }

        private void extractFoldersMenuItem_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedIndices.Count > 0)
            {
                this.ExtractFilesTo(true, true, () => lvFiles.SelectedIndices.Cast<int>().Select(index => _files[index]));
            }
        }

        private void previewMenuItem_Click(object sender, EventArgs e)
        {
            this.PreviewSelected();
        }

        private void quickExtractsMenuItem_Click(object sender, EventArgs e)
        {
            if (quickExtractsMenuItem.MenuItems.Count > 0)
                return;

            // Open options with second tab selected
            using (var of = new OptionsForm(1))
            {
                if (of.ShowDialog(this) == DialogResult.OK)
                {
                    of.SaveChanges();
                    Settings.Default.Save();

                    // Sync changes to UI
                    this.LoadQuickExtractPaths();
                }
            }
        }

        private void quickExtractMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var path = menuItem.Tag as QuickExtractPath;

            if (!Directory.Exists(path.Path))
            {
                DialogResult result = MessageBox.Show(this,
                    string.Format("{0} path doesn't exists anymore. Do you want to create it?", path.Name),
                    "Quick Extract",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return;

                Directory.CreateDirectory(path.Path);
            }

            var files = new List<ArchiveEntry>();

            foreach (int index in lvFiles.SelectedIndices)
                files.Add(_files[index]);

            ExtractFiles(path.Path, path.UseFolderPath, true, files.ToArray());
        }

        private void copyPathMenuItem1_Click(object sender, EventArgs e)
        {
            copyPathMenuItem.PerformClick();
        }

        private void copyFolderPathMenuItem1_Click(object sender, EventArgs e)
        {
            copyFolderPathMenuItem.PerformClick();
        }

        private void copyFileNameMenuItem1_Click(object sender, EventArgs e)
        {
            copyFileNameMenuItem.PerformClick();
        }

        private void selectAllMenuItem1_Click(object sender, EventArgs e)
        {
            lvFiles.SelectAllItems();
        }

        #endregion

        #region archiveContextMenu

        private void archiveContextMenu_Popup(object sender, EventArgs e)
        {
            archiveContextMenu.Tag = tvFolders.GetNodeAt(tvFolders.PointToClient(Cursor.Position));
        }

        private void extractAllFilesMenuItem_Click(object sender, EventArgs e)
        {
            this.ExtractFilesTo(false, true, () => (archiveContextMenu.Tag as ArchiveNode).Archive.Files);
        }

        private void extractAllFoldersMenuItem_Click(object sender, EventArgs e)
        {
            this.ExtractFilesTo(true, true, () => (archiveContextMenu.Tag as ArchiveNode).Archive.Files);
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            this.CloseArchive((ArchiveNode)archiveContextMenu.Tag);

            if (tvFolders.Nodes.Count == 0)
                this.ClearList();
            else
                this.DoSearch();
        }

        #endregion

        /// <summary>
        /// Opens the given archive, adding it to the TreeView and making it browsable.
        /// </summary>
        /// <param name="path">The archive file path.</param>
        /// <param name="addToRecentFiles">True if archive should be added to recent files list.</param>
        public void OpenArchive(string path, bool addToRecentFiles = false)
        {
            // Check if archive is already opened
            foreach (ArchiveNode node in tvFolders.Nodes)
            {
                if (node.Archive.FullPath.ToLower() == path.ToLower())
                {
                    tvFolders.SelectedNode = node;
                    return;
                }
            }

            Archive archive = null;

            try
            {
                string extension = Path.GetExtension(path);

                // ToDo: Read file header to find archive type, not just extension
                switch (extension.ToLower())
                {
                    case ".bsa":
                    case ".dat":
                        if (SharpBSABA2.BSAUtil.BSA.IsSupportedVersion(path) == false)
                        {
                            if (MessageBox.Show(this,
                                    "Archive has an unknown version number.\n" + "Attempt to open anyway?",
                                    "Warning",
                                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                        }

                        archive = new SharpBSABA2.BSAUtil.BSA(path);
                        break;
                    case ".ba2":
                        archive = new SharpBSABA2.BA2Util.BA2(path)
                        {
                            UseATIFourCC = Settings.Default.UseATIFourCC
                        };
                        break;
                    default:
                        throw new Exception($"Unrecognized archive file type ({extension}).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            var newNode = new ArchiveNode(
                Path.GetFileNameWithoutExtension(path) + this.DetectGame(path),
                archive);

            newNode.ContextMenu = archiveContextMenu;
            newNode.Files = archive.Files.ToArray();
            newNode.Nodes.Add("empty");
            tvFolders.Nodes.Add(newNode);

            if (newNode.IsExpanded)
                newNode.Collapse();

            btnExtractAllFolders.Enabled = true;
            btnExtractAll.Enabled = true;
            btnPreview.Enabled = true;

            if (addToRecentFiles)
                this.AddToRecentFiles(path);

            tvFolders.SelectedNode = newNode;

            _compareForm?.AddArchive(archive);
        }

        /// <summary>
        /// Opens all given archives.
        /// </summary>
        /// <param name="addToRecentFiles">True if archives should be added to recent files list.</param>
        /// <param name="paths">Array of archive file paths.</param>
        public void OpenArchives(bool addToRecentFiles, params string[] paths)
        {
            foreach (string path in paths)
                this.OpenArchive(path, addToRecentFiles);
        }

        /// <summary>
        /// Adds the given file to the recent files list. If it already exists in the list, it gets bumped up to the top.
        /// </summary>
        /// <param name="file">The file to add.</param>
        private void AddToRecentFiles(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            if (this.RecentListContains(file))
            {
                var item = this.RecentListGetItemByString(file);

                if (item == null)
                    return;

                int index = recentFilesMenuItem.MenuItems.IndexOf(item);
                recentFilesMenuItem.MenuItems.Remove(item);
                recentFilesMenuItem.MenuItems.Add(2, item);
            }
            else
            {
                var item = new MenuItem(file, recentFiles_Click);
                item.Tag = file;
                recentFilesMenuItem.MenuItems.Add(2, item);
            }
        }

        /// <summary>
        /// Clears the virtual ListView.
        /// </summary>
        private void ClearList()
        {
            lvFiles.BeginUpdate();
            _files.Clear();
            lvFiles.VirtualListSize = 0;
            lvFiles.EndUpdate();
        }

        /// <summary>
        /// Closes the given archive, removing it from the TreeView.
        /// </summary>
        /// <param name="archiveNode"></param>
        private void CloseArchive(ArchiveNode archiveNode)
        {
            if (SelectedArchiveNode == archiveNode)
                this.ClearList();

            archiveNode.Archive.Close();
            _compareForm?.RemoveArchive(archiveNode.Archive);

            tvFolders.Nodes.Remove(archiveNode);

            GC.Collect();

            if (tvFolders.GetNodeCount(false) == 0)
            {
                btnPreview.Enabled = false;
                btnExtractAllFolders.Enabled = false;
                btnExtractAll.Enabled = false;
            }
        }

        /// <summary>
        /// Closes all open archives, clearing the TreeView.
        /// </summary>
        private void CloseArchives()
        {
            this.ClearList();

            foreach (ArchiveNode node in tvFolders.Nodes)
            {
                node.Archive.Close();
                _compareForm?.RemoveArchive(node.Archive);
            }

            tvFolders.Nodes.Clear();

            GC.Collect();

            // Disable buttons
            btnPreview.Enabled = btnExtractAllFolders.Enabled = btnExtractAll.Enabled = false;
        }

        /// <summary>
        /// Returns string with game name if given path is a Fallout 3 or Fallout New Vegas file
        /// since these two games share a lot of file names.
        /// </summary>
        private string DetectGame(string path)
        {
            // path is not a original Fallout file, no additional identifier required
            if (!Path.GetFileName(path).ToLower().StartsWith("fallout -"))
                return string.Empty;

            var f3 = new Regex(@"^.*(Fallout|F)\s{0,1}(3).*$", RegexOptions.IgnoreCase);
            var fnv = new Regex(@"^.*(Fallout|F)\s{0,1}(NV|New\s{0,1}Vegas).*$", RegexOptions.IgnoreCase);

            if (f3.IsMatch(path))
                return " (F3)";
            else if (fnv.IsMatch(path))
                return " (NV)";

            return string.Empty;
        }

        /// <summary>
        /// Searches files list, filtering out not-matching files.
        /// </summary>
        private void DoSearch()
        {
            _searchDelayTimer?.Stop();

            if (tvFolders.GetNodeCount(false) < 1 || tvFolders.SelectedNode == null)
                return;

            string str = txtSearch.Text;

            // Reset text color
            txtSearch.ForeColor = System.Drawing.SystemColors.WindowText;

            _files.Clear();

            if (str.Length == 0)
                _files.AddRange(this.SelectedArchiveNode.Files);
            else if (cbRegex.Checked)
            {
                Regex regex;

                try
                {
                    regex = new Regex(str, RegexOptions.Compiled | RegexOptions.Singleline);
                }
                catch
                {
                    // Set text color to red to indicate an error with the search pattern
                    txtSearch.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                for (int i = 0; i < this.SelectedArchiveNode.Files.Length; i++)
                {
                    var file = this.SelectedArchiveNode.Files[i];

                    if (regex.IsMatch(file.FullPath))
                        _files.Add(file);
                }
            }
            else
            {
                // Escape special characters, then unescape wild card characters again
                str = WildcardPattern.Escape(str).Replace("`*", "*");
                var pattern = new WildcardPattern($"*{str}*", WildcardOptions.Compiled | WildcardOptions.IgnoreCase);

                try
                {
                    for (int i = 0; i < this.SelectedArchiveNode.Files.Length; i++)
                    {
                        var file = this.SelectedArchiveNode.Files[i];

                        if (pattern.IsMatch(file.FullPath))
                            _files.Add(file);
                    }
                }
                catch
                {
                    // Set text color to red to indicate an error with the search term
                    txtSearch.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            // Refresh list items
            lvFiles.BeginUpdate();
            lvFiles.VirtualListSize = _files.Count;
            lvFiles.Invalidate();
            lvFiles.EndUpdate();

            lFileCount.Text = string.Format("{0:n0} files", _files.Count);
        }

        /// <summary>
        /// Extracts the given file(s) to the given path.
        /// </summary>
        /// <param name="folder">The path to extract files to.</param>
        /// <param name="useFolderPath">True to use full folder path for files, false to extract straight to path.</param>
        /// <param name="gui">True to show a progression dialog.</param>
        /// <param name="files">The files in the selected archive to extract.</param>
        private void ExtractFiles(string folder, bool useFolderPath, bool gui, params ArchiveEntry[] files)
        {
            if (gui)
            {
                pf = new ProgressForm("Unpacking archive");
                pf.EnableCancel();
                pf.SetProgressRange(100);
                pf.Canceled += delegate { bw.CancelAsync(); };
                pf.Show(this);

                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += bw_DoWork;
                bw.ProgressChanged += bw_ProgressChanged;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.RunWorkerAsync(new ExtractFilesArguments()
                {
                    UseFolderPath = useFolderPath,
                    Folder = folder,
                    Files = files
                });
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
                    MessageBox.Show(this, ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Opens folder browser to select where to extract file(s).
        /// </summary>
        /// <param name="useFolderPath">True to use full folder path for files, false to extract straight to path.</param>
        /// <param name="gui">True to show a progression dialog.</param>
        /// <param name="selector">The files in the selected archive to extract.</param>
        private void ExtractFilesTo(bool useFolderPath, bool gui, Func<IEnumerable<ArchiveEntry>> selector)
        {
            var archive = this.SelectedArchiveNode.Archive.FullPath;

            _openFolderDialog.DefaultFolder = Path.GetDirectoryName(archive);

            if (_openFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.ExtractFiles(_openFolderDialog.Folder, useFolderPath, gui, selector.Invoke().ToArray());
            }
        }

        #region ExtractFiles variables

        BackgroundWorker bw;
        ProgressForm pf;

        private class ExtractFilesArguments
        {
            public bool UseFolderPath { get; set; }
            public string Folder { get; set; }
            public ArchiveEntry[] Files { get; set; }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as ExtractFilesArguments;
            var extracted = new Dictionary<string, int>();

            try
            {
                int progress = 0;
                int prevProgress = 0;
                int count = 0;

                foreach (var fe in arguments.Files)
                {
                    if (bw.CancellationPending)
                    {
                        e.Result = false;
                        break;
                    }

                    // Update ProgressForm's current file
                    bw.ReportProgress(-1, fe.FileName);

                    if (arguments.UseFolderPath)
                    {
                        fe.Extract(arguments.Folder, arguments.UseFolderPath);
                    }
                    else
                    {
                        if (extracted.ContainsKey(fe.FileName))
                        {
                            string filename = Path.GetFileNameWithoutExtension(fe.FileName);
                            string extension = Path.GetExtension(fe.FileName);

                            fe.Extract(arguments.Folder,
                                arguments.UseFolderPath,
                                $"{filename} ({++extracted[fe.FileName]}){extension}");
                        }
                        else
                        {
                            fe.Extract(arguments.Folder, arguments.UseFolderPath);
                            extracted.Add(fe.FileName, 0);
                        }
                    }

                    count++;
                    progress = (int)Math.Round(((double)count / arguments.Files.Length) * 100);
                    if (progress > prevProgress)
                    {
                        prevProgress = progress;
                        bw.ReportProgress(progress);
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == -1)
            {
                pf.SetCurrentFile(e.UserState as string);
            }
            else
            {
                pf.UpdateProgress(e.ProgressPercentage);
                this.Text = $"{pf.GetProgressPercentage()}% - {_untouchedTitle}";
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pf.Unblock();
            pf.Close();
            pf.Dispose();
            pf = null;

            bw.Dispose();
            bw = null;

            this.Text = _untouchedTitle;

            if (e.Result is Exception)
            {
                MessageBox.Show(this, (e.Result as Exception).Message, "Error");
            }
        }

        #endregion

        /// <summary>
        /// Formats the given file size to a more readable string.
        /// </summary>
        /// <param name="bytes">The file size to format.</param>
        private string FormatBytes(long bytes)
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
        /// Returns the root node of the given TreeNode.
        /// </summary>
        /// <param name="node">The TreeNode to get root node from.</param>
        private ArchiveNode GetRootNode(TreeNode node)
        {
            if (node == null)
                return null;

            var rootNode = node;
            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;
            return rootNode as ArchiveNode;
        }

        /// <summary>
        /// Returns true if file is supported by this program. False otherwise.
        /// </summary>
        private bool IsSupportedFile(string file)
        {
            switch (Path.GetExtension(file))
            {
                case ".bsa":
                case ".ba2":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if update is available online.
        /// </summary>
        private async Task<bool> IsUpdateAvailable()
        {
            using (var wc = new WebClient())
            {
                // Add tick count to disable caching.
                var onlineVersion = new Version(await wc.DownloadStringTaskAsync(Program.VersionUrl + $"?nocache={Environment.TickCount}"));
                var localVersion = new Version(Application.ProductVersion);

                return localVersion < onlineVersion;
            }
        }

        /// <summary>
        /// Loads quick extract paths into Quick Extract menu item.
        /// </summary>
        private void LoadQuickExtractPaths()
        {
            openFoldersMenuItem.MenuItems.Clear();
            quickExtractsMenuItem.MenuItems.Clear();

            foreach (QuickExtractPath path in Settings.Default.QuickExtractPaths)
            {
                openFoldersMenuItem.MenuItems.Add(
                    new MenuItem(path.Name, openFolderMenuItem_Click)
                    {
                        Tag = path
                    });
                quickExtractsMenuItem.MenuItems.Add(
                    new MenuItem(path.Name, quickExtractMenuItem_Click)
                    {
                        Tag = path
                    });
            }
        }

        /// <summary>
        /// Previews selected file in default program or built-in tool if supported.
        /// </summary>
        private void PreviewSelected()
        {
            if (lvFiles.SelectedIndices.Count == 0)
                return;

            if (lvFiles.SelectedIndices.Count == 1)
            {
                var fe = _files[lvFiles.SelectedIndices[0]];
                var extension = Path.GetExtension(fe.LowerPath);

                switch (extension)
                {
                    /*case ".nif":
                        MessageBox.Show("Viewing of nif's disabled as their format differs from oblivion");
                        return;
                    case ".tga":
                        System.Diagnostics.Process.Start("obmm\\NifViewer.exe", fe.LowerName);
                        break;*/
                    case ".dds":
                    case ".bmp":
                    case ".png":
                    case ".jpg":
                        if (fe is SharpBSABA2.BA2Util.BA2GNFEntry)
                        {
                            MessageBox.Show(this, "Can't preview GNF .dds files.");
                            return;
                        }

                        try
                        {
                            DDSViewer.ShowDialog(this, fe);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this, ex.Message);
                        }
                        break;
                    case ".txt":
                    case ".xml":
                    case ".lst":
                    case ".psc":
                        string dest = Program.CreateTempDirectory();

                        fe.Extract(dest, false);
                        Process.Start(Path.Combine(dest, fe.FileName));
                        break;
                    default:
                        MessageBox.Show(this,
                            "Filetype not supported.\n" +
                            "Currently only .txt, .xml, .lst, .psc, .dds, .bmp, .png and .jpg files can be previewed.",
                            "Error");
                        break;
                }
            }
            else
            {
                MessageBox.Show(this, "Can only preview one file at a time", "Error");
            }
        }

        /// <summary>
        /// Returns true if recent files list contains the given file, false otherwise.
        /// </summary>
        /// <param name="file">The file to check.</param>
        private bool RecentListContains(string file)
        {
            return recentFilesMenuItem.MenuItems
                .Cast<MenuItem>()
                .Any(x => x.Tag != null && x.Tag.ToString() == file);
        }

        /// <summary>
        /// Returns the given file's MenuItem.
        /// </summary>
        /// <param name="file">The file to get MenuItem from.</param>
        private MenuItem RecentListGetItemByString(string file)
        {
            return recentFilesMenuItem.MenuItems
                .Cast<MenuItem>()
                .First(x => x.Tag != null && x.Tag.ToString() == file);
        }

        /// <summary>
        /// Saves the recent files list to Settings.
        /// </summary>
        private void SaveRecentFiles()
        {
            if (Settings.Default.RecentFiles == null)
                Settings.Default.RecentFiles = new StringCollection();
            else
                Settings.Default.RecentFiles.Clear();

            // Go bottom-to-top, stopping before clear list and separator items
            for (int i = recentFilesMenuItem.MenuItems.Count - 1; i != 1; i--)
                Settings.Default.RecentFiles.Add(recentFilesMenuItem.MenuItems[i].Tag.ToString());
        }

        /// <summary>
        /// Adds update marker (UpdateMarker constant) to Help & Check for update menu items if there is an update available.
        /// </summary>
        private async void ShowUpdateNotification()
        {
            try
            {
                if (await this.IsUpdateAvailable())
                {
                    helpMenuItem.Text = UpdateMarker + helpMenuItem.Text;
                    checkForUpdateMenuItem.Text = UpdateMarker + checkForUpdateMenuItem.Text;
                }
            }
            catch
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Sorts all items in list according to user selection.
        /// </summary>
        private void SortList()
        {
            ArchiveFileSorter.SetSorter(Settings.Default.SortType, Settings.Default.SortDesc);
            lvFiles.BeginUpdate();

            // Sort the archive so it only needs to be done once
            this.SelectedArchiveNode?.AllFiles.Sort(_filesSorter);

            // Repopulate 'SelectedArchiveNode.Files' with sorted list by triggering this event
            if (this.SelectedArchiveNode != null)
                tvFolders_AfterSelect(null, new TreeViewEventArgs(tvFolders.SelectedNode, TreeViewAction.Unknown));

            lvFiles.EndUpdate();
        }

        /// <summary>
        /// Sorts all nodes in given TreeNode.
        /// </summary>
        /// <param name="rootNode">The TreeNode whose children is to be sorted.</param>
        private void SortNodes(TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                var nodes = new TreeNode[node.Nodes.Count];

                node.Nodes.CopyTo(nodes, 0);

                Array.Sort(nodes, new TreeNodeSorter());

                node.Nodes.Clear();
                node.Nodes.AddRange(nodes);

                this.SortNodes(node);
            }
        }
    }

    public class ArchiveFileSorter : Comparer<ArchiveEntry>
    {
        internal static ArchiveFileSortOrder order = 0;
        internal static bool desc = true;

        public static void SetSorter(ArchiveFileSortOrder sortOrder, bool sortDesc)
        {
            order = sortOrder;
            desc = sortDesc;
        }

        public override int Compare(ArchiveEntry a, ArchiveEntry b)
        {
            switch (order)
            {
                case ArchiveFileSortOrder.FolderName:
                    return (desc) ? string.CompareOrdinal(a.LowerPath, b.LowerPath) : string.CompareOrdinal(b.LowerPath, a.LowerPath);
                case ArchiveFileSortOrder.FileName:
                    return (desc) ? string.CompareOrdinal(a.FileName, b.FileName) : string.CompareOrdinal(b.FileName, a.FileName);
                case ArchiveFileSortOrder.FileSize:
                    return (desc) ? a.DisplaySize.CompareTo(b.DisplaySize) : b.DisplaySize.CompareTo(a.DisplaySize);
                case ArchiveFileSortOrder.FileType:
                    return (desc) ? string.CompareOrdinal(Path.GetExtension(a.FileName), Path.GetExtension(b.FileName)) :
                                    string.CompareOrdinal(Path.GetExtension(b.FileName), Path.GetExtension(a.FileName));
                default:
                    return 0;
            }
        }
    }

    public class TreeNodeSorter : Comparer<TreeNode>
    {
        public override int Compare(TreeNode a, TreeNode b)
        {
            if (a == null)
            {
                return b == null ? 0 : -1;
            }
            else
            {
                return b == null ? 1 : a.Text.CompareTo(b.Text);
            }
        }
    }
}