using BSA_Browser.Classes;
using BSA_Browser.Dialogs;
using BSA_Browser.Extensions;
using BSA_Browser.Properties;
using SharpBSABA2;
using SharpBSABA2.BA2Util;
using SharpBSABA2.Enums;
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

namespace BSA_Browser
{
    public enum ArchiveFileSortOrder
    {
        FilePath,
        FileSize,
        Extra
    }

    public enum SystemErrorCodes : int
    {
        ERROR_CANCELLED = 0x4C7,
        ERROR_NO_ASSOCIATION = 0x483
    }

    public partial class BSABrowser : Form
    {
        private const string UpdateMarker = "(!) ";

        OpenFolderDialog _openFolderDialog = new OpenFolderDialog();
        List<ArchiveEntry> _files = new List<ArchiveEntry>();
        ArchiveFileSorter _filesSorter = new ArchiveFileSorter();
        CompareForm _compareForm;

        /// <summary>
        /// Gets the selected <see cref="ArchiveNode"/>.
        /// </summary>
        private ArchiveNode SelectedArchiveNode
        {
            get
            {
                if (tvFolders.SelectedNode == null)
                    return null;

                return this.GetRootNode(tvFolders.SelectedNode) as ArchiveNode;
            }
        }

        public BSABrowser()
        {
            InitializeComponent();
            // Set it here otherwise DPI scaling will not work correctly, for some reason
            this.Menu = mainMenu1;

            // Show application version in title
            this.Text += $" ({Program.GetVersion()})";

            var archiveNode = new ArchiveNode("All", null)
            {
                Files = new ArchiveEntry[0],
                AllFiles = new List<ArchiveEntry>()
            };
            tvFolders.Nodes.Add(archiveNode);

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
            lvFiles.EnableVisualStyles();

            // Set TextBox cue
            txtSearch.SetCue("Search term...");

#if DEBUG
            this.SetupDebugTools();
#endif

            filesImageList.Images.Add("NoAssoc", SystemIcons.FilesNoAssoc);

            foldersImageList.Images.Add(new System.Drawing.Bitmap(16, 16));
            foldersImageList.Images.Add(SystemIcons.FolderSmall);
            foldersImageList.Images.Add(SystemIcons.Files);
            foldersImageList.Images.Add((System.Drawing.Icon)this.Icon.Clone());

            if (Settings.Default.Icons.HasFlag(Enums.Icons.FileList))
            {
                lvFiles.SmallImageList = filesImageList;
            }

            if (Settings.Default.Icons.HasFlag(Enums.Icons.FolderTree))
            {
                tvFolders.ImageList = foldersImageList;
            }
        }

#if DEBUG
        #region Debug Tools

        private static Stopwatch _debugStopwatch = new Stopwatch();

        private void SetupDebugTools()
        {
            lvFiles.Columns.Add("Extra", 200);

            MenuItem debugMenuItem = new MenuItem("DEBUG");
            mainMenu1.MenuItems.Add(debugMenuItem);

            debugMenuItem.MenuItems.Add("Average opening speed of archive", OpeningSpeedAverage_Click);
            debugMenuItem.MenuItems.Add("Average extraction speed of selected item", ExtractionSpeedAverage_Click);
            debugMenuItem.MenuItems.Add("Average extraction speed of selected archive", ExtractionSpeedAverageArchive_Click);
            debugMenuItem.MenuItems.Add("Check if all textures formats are supported", CheckTextureFormats_Click);
        }

        private void OpeningSpeedAverage_Click(object sender, EventArgs e)
        {
            if (OpenArchiveDialog.ShowDialog(this) != DialogResult.OK)
                return;

            var skipped = 0;
            var path = OpenArchiveDialog.FileNames[0];

            var sw = new Stopwatch();
            var count = 0;
            var results = new List<long>();

            while (count < 100)
            {
                Console.WriteLine(count);
                sw.Restart();

                try
                {
                    var extension = Path.GetExtension(path);
                    var encoding = Encoding.GetEncoding(Settings.Default.EncodingCodePage);

                    switch (extension.ToLower())
                    {
                        case ".bsa":
                        case ".dat":
                            if (SharpBSABA2.BSAUtil.BSA.IsSupportedVersion(path) == false)
                            {
                                goto default;
                            }

                            new SharpBSABA2.BSAUtil.BSA(path, encoding, Settings.Default.RetrieveRealSize);
                            break;
                        case ".ba2":
                            new SharpBSABA2.BA2Util.BA2(path, encoding, Settings.Default.RetrieveRealSize);
                            break;
                        default:
                            skipped++;
                            break;
                    }
                }
                catch
                {
                    skipped++;
                }

                sw.Stop();
                results.Add(sw.ElapsedMilliseconds);
                count++;
            }

            MessageBox.Show(this, $"Average: {results.Sum() / results.Count}ms. {skipped} skipped.");
        }

        private void ExtractionSpeedAverage_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedIndices.Count == 0)
                return;

            if (lvFiles.SelectedIndices.Count > 1)
            {
                MessageBox.Show(this, "Can only test one file at a time", "Error");
                return;
            }

            var fe = _files[lvFiles.SelectedIndices[0]];
            Stopwatch sw = new Stopwatch();
            int count = 0;
            List<long> results = new List<long>();

            while (count < 1000)
            {
                sw.Restart();
                using (var ms = fe.GetDataStream())
                {
                    sw.Stop();
                    results.Add(sw.ElapsedMilliseconds);
                    count++;
                }
            }

            Console.WriteLine($"Average: {results.Sum() / results.Count}ms");
        }

        private void ExtractionSpeedAverageArchive_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode == null)
                return;

            if (tvFolders.SelectedNode.Index == 0)
                return;

            Stopwatch sw = new Stopwatch();
            int count = 0;
            List<long> results = new List<long>();

            while (count < 50)
            {
                sw.Restart();
                using (var ms = new MemoryStream())
                {
                    foreach (var file in SelectedArchiveNode.Archive.Files)
                    {
                        file.GetDataStream().CopyTo(ms);
                    }
                }
                sw.Stop();
                results.Add(sw.ElapsedMilliseconds);
                count++;
                Console.WriteLine($"{count} - Average: {results.Sum() / results.Count}ms");
            }

            MessageBox.Show($"Average: {results.Sum() / results.Count}ms");
        }

        private void CheckTextureFormats_Click(object sender, EventArgs e)
        {
            int checkedTextures = 0;
            int unsupported = 0;
            var sw = new Stopwatch();

            sw.Start();

            foreach (var fe in _files.OfType<BA2TextureEntry>())
            {
                checkedTextures++;

                if (!fe.IsFormatSupported())
                    unsupported++;
            }

            sw.Stop();
            MessageBox.Show($"Checked {checkedTextures} textures in {sw.ElapsedMilliseconds}ms, {unsupported} unsupported textures.");
        }

        #endregion
#endif

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

            if (Settings.Default.CheckForUpdates)
            {
                // Show ! in main menu if update is available
                this.ShowUpdateNotification();
            }
        }

        private void BSABrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tvFolders.GetNodeCount(false) > 1)
                this.CloseArchives();

            this.SaveRecentFiles();

            Settings.Default.WindowStates[this.Name].SaveForm(this);
            Settings.Default.LastUnpackPath = _openFolderDialog.Folder;
            Settings.Default.Save();
        }

        private void BSABrowser_Shown(object sender, EventArgs e)
        {
            // Show notification if Settings was reset this session
            if (Program.SettingsReset)
                MessageBox.Show(this,
                    "Settings was reset to default.",
                    "Settings Reset",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (this.SelectedArchiveNode != null)
            {
                this.ExtractFilesTo(false, true, () => _files);
            }
        }

        private void btnExtractFolders_Click(object sender, EventArgs e)
        {
            if (this.SelectedArchiveNode != null)
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

        private void lvFiles_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_files.Count <= e.ItemIndex)
                return;

            var file = _files[e.ItemIndex];
            var fullpath = Path.Combine(file.Folder, file.FileName);

            // Prepend 1-based indexing if there is no name table
            if (!file.Archive.HasNameTable)
                fullpath = $"({file.Index + 1}) {fullpath}";

            var lvi = new ListViewItem(fullpath, GetFileIconIndex(fullpath));

            lvi.SubItems.Add(Common.FormatBytes(file.DisplaySize));
            lvi.SubItems.Add(file.Archive.FileName);
            lvi.ToolTipText = file.GetToolTipText();
            lvi.Tag = file;

#if DEBUG
            if (file is BA2TextureEntry)
                lvi.SubItems.Add(Enum.GetName(typeof(DXGI_FORMAT), (file as BA2TextureEntry).format));
            else
                lvi.SubItems.Add(string.Empty);
#endif

            e.Item = lvi;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LimitedAction.RunAfter(1, 500, this.DoSearch);
        }

        private void cbRegex_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SearchUseRegex = cbRegex.Checked;
            this.DoSearch();
        }

        private void tvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var rootNode = this.GetRootNode(e.Node) as ArchiveNode;

            // Check if node structure has already been built
            if (rootNode.AllFiles != null)
                return;

            e.Node.Nodes.Clear();
            var nodes = new Dictionary<string, TreeNode>();
            rootNode.AllFiles = new List<ArchiveEntry>(rootNode.Files);
            rootNode.AllFiles.Sort(_filesSorter);

            // Keep track of directories with files directly under them
            var directoriesWithFiles = new List<string>();

            // Build the whole node structure
            foreach (var lvi in rootNode.AllFiles)
            {
                string path = Path.GetDirectoryName(lvi.FullPath);

                if (!directoriesWithFiles.Contains(path.ToLower()))
                    directoriesWithFiles.Add(path.ToLower());

                // Ignore files under root node and already processed nodes
                if (path == string.Empty || nodes.ContainsKey(path.ToLower()))
                    continue;

                string[] dirs = path.Split('\\');

                for (int i = 0; i < dirs.Length; i++)
                {
                    string newpath = string.Join("\\", dirs, 0, i + 1);

                    if (!nodes.ContainsKey(newpath.ToLower()))
                    {
                        var tn = new TreeNode(dirs[i], 1, 1);
                        tn.Tag = newpath;

                        if (i == 0) // Root node
                            e.Node.Nodes.Add(tn);
                        else // Sub nodes
                            nodes[path.ToLower()].Nodes.Add(tn);

                        nodes.Add(newpath.ToLower(), tn);
                    }
                    path = newpath;
                }
            }

            // Insert <Files> nodes under each directory
            foreach (var node in nodes)
            {
                // Only add if there are sub nodes. Node without sub nodes already behaves the same
                if (node.Value.Nodes.Count > 0 && directoriesWithFiles.Contains(node.Key.ToLower()))
                    node.Value.Nodes.Insert(0, new TreeNode("<Files>", 2, 2) { Tag = node.Key });
            }

            if (Settings.Default.SortArchiveDirectories)
            {
                this.SortNodes(e.Node);
            }
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var rootNode = this.GetRootNode(e.Node) as ArchiveNode;

            // If AllFiles is null, trigger event which will populate it
            if (rootNode.AllFiles == null)
                tvFolders_BeforeExpand(null, new TreeViewCancelEventArgs(e.Node, false, TreeViewAction.Unknown));

            if (rootNode.Index == 0) // 'All' node is selected, show all files in all archives
            {
                var lvis = new List<ArchiveEntry>();

                for (int i = 1; i < tvFolders.Nodes.Count; i++)
                {
                    lvis.AddRange((tvFolders.Nodes[i] as ArchiveNode).AllFiles);
                }

                rootNode.Files = lvis.ToArray();
            }
            else if (e.Node.Tag == null) // Root node is selected, so show all files
                rootNode.Files = rootNode.AllFiles?.ToArray();
            else
            {
                // Ignore casing
                string lowerPath = ((string)e.Node.Tag).ToLower();

                // Only show files under selected node
                var lvis = new List<ArchiveEntry>(rootNode.AllFiles.Count);

                foreach (var lvi in rootNode.AllFiles)
                {
                    if (e.Node.Text == "<Files>")
                    {
                        string selectedPath = Path.GetDirectoryName(lvi.FullPath.Replace('/', '\\'));

                        // Show files in current node, not including sub nodes
                        if (selectedPath.ToLower() == lowerPath) lvis.Add(lvi);
                    }
                    else
                    {
                        string path = lvi.FullPath.ToLower().Replace('/', '\\');

                        // Show all files under current node, including sub nodes.
                        // Add \ to exclude folders with similar names on same level, for example 'folder' & 'folder2'
                        if (path.StartsWith(lowerPath + '\\')) lvis.Add(lvi);
                    }
                }

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
            extractArchivesMenuItem.Enabled = tvFolders.Nodes.Count > 1;
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
            var archives = new List<Archive>(tvFolders.Nodes.Cast<ArchiveNode>().Skip(1).Select(x => x.Archive));
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
            bool replaceGNFExt = Settings.Default.ReplaceGNFExt;

            using (var of = new OptionsForm())
            {
                if (of.ShowDialog(this) == DialogResult.OK)
                {
                    of.SaveChanges();
                    Settings.Default.Save();

                    // Sync changes to UI
                    this.LoadQuickExtractPaths();

                    this.RefreshIcons();

                    // Add 2 to include the permanent menu items
                    while (recentFilesMenuItem.MenuItems.Count > (Settings.Default.RecentFiles_MaxFiles + 2))
                    {
                        recentFilesMenuItem.MenuItems.RemoveAt(recentFilesMenuItem.MenuItems.Count - 1);
                    }

                    // Sync changes to archives already opened
                    for (int i = 1; i < tvFolders.Nodes.Count; i++)
                    {
                        var archiveNode = (ArchiveNode)tvFolders.Nodes[i];
                        archiveNode.Archive.MatchLastWriteTime = Settings.Default.MatchLastWriteTime;
                    }

                    if (Settings.Default.ReplaceGNFExt != replaceGNFExt)
                    {
                        lvFiles.BeginUpdate();
                        foreach (var archive in tvFolders.Nodes
                                                .Cast<ArchiveNode>().Skip(1)
                                                .Where(x => x.Archive.Type == ArchiveTypes.BA2_GNMF))
                        {
                            Common.ReplaceGNFExtensions(archive.Files.OfType<BA2GNFEntry>(), Settings.Default.ReplaceGNFExt);
                        }
                        lvFiles.EndUpdate();
                    }
                }
            }
        }

        private void recentFilesMenuItem_Popup(object sender, EventArgs e)
        {
            emptyListMenuItem.Enabled = recentFilesMenuItem.MenuItems.Count > 2;
        }

        private void emptyListMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this,
                                "Are you sure?",
                                "Confirmation",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning) == DialogResult.No)
                return;

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

            for (int i = 1; i < tvFolders.Nodes.Count; i++)
                archives.Add((tvFolders.Nodes[i] as ArchiveNode).Archive);

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
                        Process.Start(Program.SkyrimSENexus);
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

        private void githubMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Program.GitHub);
        }

        private void fallout4NexusPageMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Program.Fallout4Nexus);
        }

        private void skyrimSENexusPageMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Program.SkyrimSENexus);
        }

        private void discordMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Program.Discord);
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
            using (var of = new OptionsForm(OptionsForm.QuickExtractIndex))
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

            ExtractFiles(this, path.Path, path.UseFolderPath, true, files, titleProgress: true);
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
            TreeNode selectedNode = tvFolders.GetNodeAt(tvFolders.PointToClient(Cursor.Position));

            // Ignore if the 'All' node is selected
            if (selectedNode?.Index == 0)
                selectedNode = null;

            extractAllFilesMenuItem.Enabled = extractAllFoldersMenuItem.Enabled = closeMenuItem.Enabled = selectedNode != null;
            archiveContextMenu.Tag = selectedNode;
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

            if (tvFolders.Nodes.Count == 1)
                this.ClearList();
            else
                this.DoSearch();
        }

        #endregion

        /// <summary>
        /// Opens the given archive, adding it to the <see cref="TreeView"/> and making it browsable.
        /// </summary>
        /// <param name="path">The archive file path.</param>
        /// <param name="addToRecentFiles">True if archive should be added to recent files list.</param>
        public void OpenArchive(string path, bool addToRecentFiles = false)
        {
            // Check if archive is already opened
            for (int i = 1; i < tvFolders.Nodes.Count; i++)
            {
                ArchiveNode node = (ArchiveNode)tvFolders.Nodes[i];
                if (node.Archive.FullPath.ToLower() == path.ToLower())
                {
                    tvFolders.SelectedNode = node;
                    return;
                }
            }

            Archive archive = Common.OpenArchive(path, this);

            var newNode = new ArchiveNode(
                Path.GetFileNameWithoutExtension(path) + this.DetectGame(path),
                archive);

            newNode.ImageIndex = newNode.SelectedImageIndex = 3;
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

            // Add 2 to include the permanent menu items
            while (recentFilesMenuItem.MenuItems.Count > (Settings.Default.RecentFiles_MaxFiles + 2))
            {
                recentFilesMenuItem.MenuItems.RemoveAt(recentFilesMenuItem.MenuItems.Count - 1);
            }
        }

        /// <summary>
        /// Clears the virtual <see cref="ListView"/>.
        /// </summary>
        private void ClearList()
        {
            lvFiles.BeginUpdate();
            _files.Clear();
            lvFiles.VirtualListSize = 0;
            lvFiles.EndUpdate();
        }

        /// <summary>
        /// Closes the given archive, removing it from the <see cref="TreeView"/>.
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

            if (tvFolders.GetNodeCount(false) == 1)
            {
                btnPreview.Enabled = false;
                btnExtractAllFolders.Enabled = false;
                btnExtractAll.Enabled = false;
            }
        }

        /// <summary>
        /// Closes all open archives, clearing the <see cref="TreeView"/>.
        /// </summary>
        private void CloseArchives()
        {
            this.ClearList();

            for (int i = tvFolders.Nodes.Count - 1; i > 0; i--)
            {
                ArchiveNode node = (ArchiveNode)tvFolders.Nodes[i];
                node.Archive.Close();
                _compareForm?.RemoveArchive(node.Archive);
                tvFolders.Nodes.RemoveAt(i);
            }

            GC.Collect();

            // Disable buttons
            btnPreview.Enabled = btnExtractAllFolders.Enabled = btnExtractAll.Enabled = false;
        }

        /// <summary>
        /// Returns <see cref="string"/> with game name if given path is a Fallout 3 or Fallout New Vegas file
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
            LimitedAction.Stop(1);

            if (tvFolders.SelectedNode == null)
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
        /// <param name="gui">True to show a <see cref="ProgressForm"/>.</param>
        /// <param name="files">The files in the selected archive to extract.</param>
        public static void ExtractFiles(Form owner, string folder, bool useFolderPath, bool gui, IList<ArchiveEntry> files, ProgressForm progressForm = null, bool titleProgress = false)
        {
            // Check for unsupported textures and prompt the user what to do if there is
            if (Common.CheckForUnsupportedTextures(files))
            {
                DialogResult result = MessageBox.Show(owner,
                    "There are unsupported textures about to be extracted. These are missing DDS headers that can't (currently) be generated.\n\n" +
                    "Do you want to extract the raw data without DDS header? Selecting 'No' will skip these textures.",
                    "Unsupported Textures", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.No)
                {
                    // Remove unsupported textures
                    for (int i = files.Count; i-- > 0;)
                    {
                        if ((files[i] as BA2TextureEntry)?.IsFormatSupported() == false)
                            files.RemoveAt(i);
                    }
                }
                else if (result == DialogResult.Yes)
                {
                    foreach (var fe in files.OfType<BA2TextureEntry>().Where(x => x.IsFormatSupported() == false))
                    {
                        fe.GenerateTextureHeader = false;
                    }
                }
            }

            if (gui)
            {
                var operation = new ExtractOperation(folder, files, useFolderPath)
                {
                    TitleProgress = titleProgress
                };
                operation.StateChange += ExtractOperation_StateChange;
                operation.ProgressPercentageUpdate += ExtractOperation_ProgressPercentageUpdate;
                operation.Completed += ExtractOperation_Completed;

                _progressForm = progressForm;
                if (_progressForm == null)
                {
                    _progressForm = Common.CreateProgressForm(files.Count);
                }
                _progressForm.Canceled += delegate { operation.Cancel(); };

#if DEBUG
                // Track extraction speed
                _debugStopwatch.Restart();
#endif

                _owner = owner;
                _originalTitle = owner?.Text;

                operation.Start();
                _progressForm.ShowDialog(owner);
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
        /// Opens <see cref="OpenFolderDialog"/> to select where to extract file(s).
        /// </summary>
        /// <param name="useFolderPath">True to use full folder path for files, false to extract straight to path.</param>
        /// <param name="gui">True to show a progression dialog.</param>
        /// <param name="selector">The files in the selected archive to extract.</param>
        private void ExtractFilesTo(bool useFolderPath, bool gui, Func<IEnumerable<ArchiveEntry>> selector)
        {
            var archive = this.SelectedArchiveNode.Archive?.FullPath;

            _openFolderDialog.DefaultFolder = Path.GetDirectoryName(archive);

            if (_openFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                BSABrowser.ExtractFiles(this,
                    _openFolderDialog.Folder,
                    useFolderPath,
                    gui,
                    selector.Invoke().ToList(),
                    titleProgress: true);
            }
        }

        #region ExtractFiles variables

        static ProgressForm _progressForm;
        static Form _owner;
        static string _originalTitle;

        private static void ExtractOperation_StateChange(ExtractOperation sender, StateChangeEventArgs e)
        {
            _progressForm.Description = e.FileName + '\n' + Common.FormatTimeRemaining(sender.EstimateTimeRemaining);
            _progressForm.Footer = $"({e.Count}/{e.FilesTotal}) {Common.FormatBytes(sender.SpeedBytes)}/s";
        }

        private static void ExtractOperation_ProgressPercentageUpdate(ExtractOperation sender, ProgressPercentageUpdateEventArgs e)
        {
            if (_owner != null && sender.TitleProgress)
                _owner.Text = $"{e.ProgressPercentage}% - {_originalTitle}";

            _progressForm.Progress = e.ProgressPercentage;
            _progressForm.Description = _progressForm.Description.Split('\n')[0] + "\n" + Common.FormatTimeRemaining(e.RemainingEstimate);
        }

        private static void ExtractOperation_Completed(ExtractOperation sender, CompletedEventArgs e)
        {
#if DEBUG
            _debugStopwatch.Stop();
            Console.WriteLine($"Extraction complete. {_debugStopwatch.ElapsedMilliseconds}ms elapsed");
#endif

            _progressForm.BlockClose = false;
            _progressForm.Close();

            if (_owner != null && sender.TitleProgress)
                _owner.Text = _originalTitle;

            // Save exceptions to _report.txt file in destination path
            if (e?.Exceptions.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{sender.Files[0].Archive.FileName} - RetrieveRealSize: {sender.Files[0].Archive.RetrieveRealSize}");
                sb.AppendLine();

                foreach (var ex in e.Exceptions)
                    sb.AppendLine($"{ex.ArchiveEntry.FullPath}{Environment.NewLine}{ex.Exception}{Environment.NewLine}");

                File.WriteAllText(Path.Combine(sender.Folder, "_report.txt"), sb.ToString());
                MessageBox.Show(_owner, $"{e.Exceptions.Count} file(s) failed to extract. See report file in destination for details.", "Error");
            }
        }

        #endregion

        /// <summary>
        /// Returns index for file icon in <see cref="filesImageList"/>.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private int GetFileIconIndex(string filepath)
        {
            if (lvFiles.SmallImageList == null)
                return -1;

            string ext = Path.GetExtension(filepath).TrimStart('.');

            // Return icon for no association if no ext
            if (string.IsNullOrEmpty(ext))
                return 0;

            // Add missing icons
            if (!filesImageList.Images.ContainsKey(ext))
                filesImageList.Images.Add(ext, SystemIcons.GetFileIcon(filepath));

            return filesImageList.Images.IndexOfKey(ext);
        }

        /// <summary>
        /// Returns the root node of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> to get root node from.</param>
        private TreeNode GetRootNode(TreeNode node)
        {
            if (node == null)
                return null;

            var rootNode = node;
            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;
            return rootNode;
        }

        /// <summary>
        /// Returns true if file is supported by this program. False otherwise.
        /// </summary>
        private bool IsSupportedFile(string file)
        {
            switch (Path.GetExtension(file).ToLower())
            {
                case ".bsa":
                case ".ba2":
                case ".dat":
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

            if (lvFiles.SelectedIndices.Count > 1)
            {
                MessageBox.Show(this, "Can only preview one file at a time", "Error");
                return;
            }

            var fe = _files[lvFiles.SelectedIndices[0]];
            string fileName = fe.FileName;
            var extension = Path.GetExtension(fe.LowerPath);

            switch (extension)
            {
                case ".dds":
                case ".bmp":
                case ".png":
                case ".jpg":
                    if ((fe as BA2TextureEntry)?.IsFormatSupported() == false)
                    {
                        MessageBox.Show(this, "Unsupported DDS texture.");
                        return;
                    }

                    if (!Settings.Default.BuiltInPreviewing.Contains(extension))
                        goto default;

                    if (fe is BA2GNFEntry)
                    {
                        if (Settings.Default.ReplaceGNFExt)
                        {
                            fileName = Path.GetFileNameWithoutExtension(fileName) + ".gnf";
                        }

                        goto default;
                    }

                    try
                    {
                        DDSViewer.ShowDialog(this, fe.FileName, fe.GetDataStream());
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

                    new TextViewer(fe).Show(this);
                    break;
                default:
                    string dest = Program.CreateTempDirectory();
                    string file = Path.Combine(dest, fileName);
                    fe.Extract(dest, false, fileName);

                    try
                    {
                        Process.Start(new ProcessStartInfo(file));
                    }
                    catch (Win32Exception ex)
                    {
                        if (ex.NativeErrorCode == (int)SystemErrorCodes.ERROR_NO_ASSOCIATION)
                            ShellExecute.OpenWith(file);
                        else if (ex.NativeErrorCode != (int)SystemErrorCodes.ERROR_CANCELLED)
                            MessageBox.Show(this, ex.Message, "Preview Error");
                    }
                    break;
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
        /// Returns the given file's <see cref="MenuItem"/>.
        /// </summary>
        /// <param name="file">The file to get <see cref="MenuItem"/> from.</param>
        private MenuItem RecentListGetItemByString(string file)
        {
            return recentFilesMenuItem.MenuItems
                .Cast<MenuItem>()
                .First(x => x.Tag != null && x.Tag.ToString() == file);
        }

        private void RefreshIcons()
        {
            bool requiresReloadFiles = (!Settings.Default.Icons.HasFlag(Enums.Icons.FileList) && lvFiles.SmallImageList != null) ||
                                       (Settings.Default.Icons == Enums.Icons.None && lvFiles.SmallImageList != null);
            bool requiresReloadFolders = (!Settings.Default.Icons.HasFlag(Enums.Icons.FolderTree) && tvFolders.ImageList != null) ||
                                         (Settings.Default.Icons == Enums.Icons.None && tvFolders.ImageList != null);

            if (requiresReloadFiles)
            {
                lvFiles.SmallImageList = null;
                // Need to call this to fix icon spacing persisting after disabling
                lvFiles.GetType().GetMethod(
                    "RecreateHandle",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(lvFiles, null);
                lvFiles.EnableVisualStyles();
            }

            if (requiresReloadFolders)
            {
                tvFolders.ImageList = null;
            }

            if (Settings.Default.Icons.HasFlag(Enums.Icons.FileList))
            {
                lvFiles.SmallImageList = filesImageList;
            }

            if (Settings.Default.Icons.HasFlag(Enums.Icons.FolderTree))
            {
                tvFolders.ImageList = foldersImageList;
            }
        }

        /// <summary>
        /// Saves the recent files list to <see cref="Settings"/>.
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
        /// Adds update marker (UpdateMarker constant) to Help and Check for update menu items if there is an update available.
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
        /// Sorts all nodes in given <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="rootNode">The <see cref="TreeNode"/> whose children is to be sorted.</param>
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
                case ArchiveFileSortOrder.FilePath:
                    if (a.Archive.HasNameTable)
                        return desc ? string.CompareOrdinal(a.LowerPath, b.LowerPath) :
                                      string.CompareOrdinal(b.LowerPath, a.LowerPath);
                    else
                        return desc ? a.Index.CompareTo(b.Index) :
                                      b.Index.CompareTo(a.Index);

                case ArchiveFileSortOrder.FileSize:
                    return desc ? a.DisplaySize.CompareTo(b.DisplaySize) :
                                  b.DisplaySize.CompareTo(a.DisplaySize);

                case ArchiveFileSortOrder.Extra:
                    if (a is BA2TextureEntry && b is BA2TextureEntry)
                    {
                        string af = Enum.GetName(typeof(DXGI_FORMAT), (a as BA2TextureEntry).format);
                        string bf = Enum.GetName(typeof(DXGI_FORMAT), (b as BA2TextureEntry).format);
                        return desc ? string.CompareOrdinal(af, bf) :
                                      string.CompareOrdinal(bf, af);
                    }
                    else
                    {
                        // Sort by file path since Extra will be empty
                        return desc ? string.CompareOrdinal(a.LowerPath, b.LowerPath) :
                                      string.CompareOrdinal(b.LowerPath, a.LowerPath);
                    }

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
                // Sort in alphabetical order, except for "<Files>" node
                return b == null ? 1 : a.Text == "<Files>" ? 0 : a.Text.CompareTo(b.Text);
            }
        }
    }
}