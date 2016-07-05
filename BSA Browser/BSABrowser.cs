using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BSA_Browser.Classes;
using BSA_Browser.Controls;
using BSA_Browser.Extensions;
using BSA_Browser.Properties;

namespace BSA_Browser
{
    public enum BSASortOrder
    {
        FolderName,
        FileName,
        FileSize,
        Offset,
        FileType
    }

    public partial class BSABrowser : Form
    {
        string _untouchedTitle;
        OpenFolderDialog _openFolderDialog = new OpenFolderDialog();
        ColumnHeader[] _extraColumns;
        BSASorter _filesSorter = new BSASorter();
        Timer _timer;

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
            if (!string.IsNullOrEmpty(Settings.Default.LastBSAUnpackPath))
                _openFolderDialog.InitialFolder = Settings.Default.LastBSAUnpackPath;

            // Load Recent Files list
            if (Settings.Default.RecentFiles != null)
            {
                foreach (string item in Settings.Default.RecentFiles)
                    AddToRecentFiles(item);
            }

            // Load Quick Extract Paths
            if (Settings.Default.QuickExtractPaths == null)
                Settings.Default.QuickExtractPaths = new QuickExtractPaths();

            this.LoadQuickExtractPaths();

            // Set lvFiles sorter
            BSASorter.SetSorter(Settings.Default.SortType, Settings.Default.SortDesc);

            // Toggle columns based on setting
            this.UpdateColumns();

            // Enable visual styles
            tvFolders.EnableVisualStyles();
            tvFolders.EnableAutoScroll();

            // TESTING
            lvFiles.VirtualMode = true;
            lvFiles.RetrieveVirtualItem += LvFiles_RetrieveVirtualItem;
            // -

            lvFiles.EnableVisualStyles();
            lvFiles.EnableVisualStylesSelection();
            lvFiles.HideFocusRectangle();

            // Set TextBox cue
            txtSearch.SetCue("Enter a filter");
        }

        public BSABrowser(string[] args)
            : this()
        {
            foreach (string file in args)
                OpenArchive(file, true);
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
            cmbSortOrder.SelectedIndex = (int)Settings.Default.SortType;
            cbDesc.Checked = Settings.Default.SortDesc;

            // Restore Regex preference
            cbRegex.Checked = Settings.Default.SearchUseRegex;
        }

        private void BSABrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tvFolders.GetNodeCount(false) > 0)
                CloseArchives();

            SaveRecentFiles();

            Settings.Default.WindowStates[this.Name].SaveForm(this);
            Settings.Default.LastBSAUnpackPath = _openFolderDialog.Folder;
            Settings.Default.Save();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (OpenBSA.ShowDialog() == DialogResult.OK)
                OpenArchive(OpenBSA.FileName, true);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedIndices.Count == 0)
                return;

            if (_openFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                var files = new List<BSAFileEntry>();

                foreach (int index in lvFiles.SelectedIndices)
                    files.Add(_files[index]);

                bool useFolderPath = Settings.Default.ExtractMaintainFolderStructure;

                ExtractFiles(_openFolderDialog.Folder, useFolderPath, true, files.ToArray());
            }
        }

        private void btnExtractAll_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode == null)
                return;

            BSAFileEntry[] files = (BSAFileEntry[])GetSelectedArchive().Files;

            if (_openFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                ExtractFiles(_openFolderDialog.Folder, true, true, files);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedIndices.Count == 0)
                return;

            if (lvFiles.SelectedIndices.Count == 1)
            {
                var fe = _files[lvFiles.SelectedIndices[0]];

                switch (Path.GetExtension(fe.LowerName))
                {
                    /*case ".nif":
                        MessageBox.Show("Viewing of nif's disabled as their format differs from oblivion");
                        return;
                    case ".dds":
                    case ".tga":
                    case ".bmp":
                    case ".jpg":
                        System.Diagnostics.Process.Start("obmm\\NifViewer.exe", fe.LowerName);
                        break;*/
                    case ".lst":
                    case ".txt":
                    case ".xml":
                        string path = Program.CreateTempDirectory();
                        BSATreeNode root = GetSelectedArchive();

                        fe.Extract(Path.Combine(path, fe.FileName), false, root.BinaryReader, root.ContainsFileNameBlobs);
                        System.Diagnostics.Process.Start(Path.Combine(path, fe.FileName));
                        break;
                    default:
                        MessageBox.Show("Filetype not supported.\n" +
                            "Currently only txt or xml files can be previewed", "Error");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Can only preview one file at a time", "Error");
            }
        }

        private void cmbSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            BSASorter.SetSorter((BSASortOrder)cmbSortOrder.SelectedIndex, cbDesc.Checked);
            lvFiles.BeginUpdate();
            _files.Sort(_filesSorter);
            lvFiles.EndUpdate();
            Settings.Default.SortType = (BSASortOrder)cmbSortOrder.SelectedIndex;
        }

        private void cbDesc_CheckedChanged(object sender, EventArgs e)
        {
            BSASorter.SetSorter((BSASortOrder)cmbSortOrder.SelectedIndex, cbDesc.Checked);
            lvFiles.BeginUpdate();
            _files.Sort(_filesSorter);
            lvFiles.EndUpdate();
            Settings.Default.SortDesc = cbDesc.Checked;
        }

        private void lvFiles_Enter(object sender, EventArgs e)
        {
            lvFiles.HideFocusRectangle();
        }

        private void lvFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!(lvFiles.SelectedIndices.Count >= 1))
                return;

            DataObject obj = new DataObject();
            StringCollection sc = new StringCollection();

            foreach (int index in lvFiles.SelectedIndices)
            {
                var fe = _files[index];
                string path = Path.Combine(Program.CreateTempDirectory(), fe.FileName);
                BSATreeNode root = GetSelectedArchive();

                fe.Extract(path, false, root.BinaryReader, root.ContainsFileNameBlobs);
                sc.Add(path);
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

        private void LvFiles_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var file = _files[e.ItemIndex];
            var lvi = new ListViewItem(Path.Combine(file.Folder, file.FileName));

            lvi.SubItems.Add(FormatBytes(file.Size));
            lvi.SubItems.Add(file.Offset.ToString());
            lvi.SubItems.Add((file.Compressed ? "Compressed" : "Uncompressed"));
            lvi.Tag = file;
            lvi.ToolTipText =
                $"File size: {FormatBytes(file.Size)}\n" +
                $"File offset: {file.Offset} bytes\n" +
                (file.Compressed ? "Compressed" : "Uncompressed");

            e.Item = lvi;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Tick += txtSearch_DoSearch;
                _timer.Interval = 1 * 750; // 1 sec

            }

            _timer.Stop();
            _timer.Start();
        }

        private List<BSAFileEntry> _files = new List<BSAFileEntry>();

        private void txtSearch_DoSearch(object sender, EventArgs e)
        {
            _timer?.Stop();

            if (!(tvFolders.GetNodeCount(false) > 0) || tvFolders.SelectedNode == null)
                return;

            string str = txtSearch.Text;

            txtSearch.ForeColor = System.Drawing.SystemColors.WindowText;

            if (cbRegex.Checked && str.Length > 0)
            {
                Regex regex;

                try
                {
                    regex = new Regex(str, RegexOptions.Compiled | RegexOptions.Singleline);
                }
                catch
                {
                    txtSearch.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                _files.Clear();

                for (int i = 0; i < GetSelectedArchive().Files.Length; i++)
                {
                    var file = GetSelectedArchive().Files[i];

                    if (regex.IsMatch(Path.Combine(file.Folder, file.FileName)))
                        _files.Add(file);
                }
            }
            else
            {
                _files.Clear();

                if (str.Length == 0)
                    _files.AddRange(GetSelectedArchive().Files);
                else
                {
                    // Escape special characters, then unescape wild card characters again
                    str = WildcardPattern.Escape(str).Replace("`*", "*");
                    var pattern = new WildcardPattern($"*{str}*", WildcardOptions.Compiled | WildcardOptions.IgnoreCase);

                    try
                    {
                        for (int i = 0; i < GetSelectedArchive().Files.Length; i++)
                        {
                            var file = GetSelectedArchive().Files[i];

                            if (pattern.IsMatch(Path.Combine(file.Folder, file.FileName)))
                                _files.Add(file);
                        }
                    }
                    catch
                    {
                        txtSearch.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                }
            }

            _files.Sort(_filesSorter);

            lvFiles.BeginUpdate();
            lvFiles.VirtualListSize = _files.Count;
            lvFiles.EndUpdate();

            lFileCount.Text = string.Format("{0:n0} files", _files.Count);
        }

        private void cbRegex_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SearchUseRegex = cbRegex.Checked;
            txtSearch_DoSearch(sender, e);
        }

        private void tvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (GetRootNode(e.Node).AllFiles != null)
                return;

            e.Node.Nodes.Clear();
            Dictionary<string, TreeNode> nodes = new Dictionary<string, TreeNode>();
            GetRootNode(e.Node).AllFiles = (BSAFileEntry[])GetRootNode(e.Node).Files.Clone();

            foreach (BSAFileEntry lvi in GetRootNode(e.Node).AllFiles)
            {
                string path = Path.GetDirectoryName(lvi.Folder);

                if (path == string.Empty || nodes.ContainsKey(path))
                    continue;

                string[] dirs = path.Split('\\');

                for (int i = 0; i < dirs.Length; i++)
                {
                    string newpath = string.Join("\\", dirs, 0, i + 1);

                    if (!nodes.ContainsKey(newpath))
                    {
                        TreeNode tn = new TreeNode(dirs[i]);
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

            if (Settings.Default.SortBSADirectories)
            {
                this.SortNodes(e.Node);
            }
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (GetRootNode(e.Node).AllFiles == null)
                tvFolders_BeforeExpand(null, new TreeViewCancelEventArgs(e.Node, false, TreeViewAction.Unknown));

            string s = (string)e.Node.Tag;

            if (s == null)
                GetRootNode(e.Node).Files = GetRootNode(e.Node).AllFiles;
            else
            {
                var lvis = new List<BSAFileEntry>(GetRootNode(e.Node).AllFiles.Length);

                foreach (var lvi in GetRootNode(e.Node).AllFiles)
                    if (lvi.FullPath.StartsWith(s)) lvis.Add(lvi);

                GetRootNode(e.Node).Files = lvis.ToArray();
            }
            txtSearch_DoSearch(tvFolders, EventArgs.Empty);
        }

        #region mainMenu1

        private void openArchiveMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenBSA.ShowDialog() == DialogResult.OK)
                OpenArchive(OpenBSA.FileName, true);
        }

        private void closeSelectedArchiveMenuItem_Click(object sender, EventArgs e)
        {
            if (GetSelectedArchive() == null)
                return;

            CloseArchive(GetSelectedArchive());
        }

        private void recentFilesMenuItem_Popup(object sender, EventArgs e)
        {
            if (recentFilesMenuItem.MenuItems.Count > 2)
                emptyListMenuItem.Enabled = true;
            else
                emptyListMenuItem.Enabled = false;
        }

        private void emptyListMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = recentFilesMenuItem.MenuItems.Count - 1; i != 1; i--)
                recentFilesMenuItem.MenuItems.RemoveAt(i);
        }

        private void recentFiles_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string file = item.Tag.ToString();

            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                OpenArchive(file, true);
            }
            else
            {
                string message = string.Format("\"{1}\" doesn't exist anymore.{0}{0}" +
                    "Do you want to remove it from the recent files list?", Environment.NewLine, item.Tag.ToString());

                if (MessageBox.Show(this, message, "Lost File", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

            copyPathMenuItem.Enabled = hasSelectedItems;
            copyFolderPathMenuItem.Enabled = hasSelectedItems;
            copyFileNameMenuItem.Enabled = hasSelectedItems;
        }

        private void copyPathMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (int index in lvFiles.SelectedIndices)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(Environment.NewLine);

                builder.Append(_files[index].FullPath);
            }

            Clipboard.SetText(builder.ToString());
        }

        private void copyFolderPathMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (int index in lvFiles.SelectedIndices)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(Environment.NewLine);

                builder.Append(Path.GetDirectoryName(_files[index].FullPath));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void copyFileNameMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (int index in lvFiles.SelectedIndices)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(Environment.NewLine);

                builder.Append(Path.GetFileName(_files[index].FullPath));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (OptionsForm of = new OptionsForm())
            {
                if (of.ShowDialog(this) == DialogResult.OK)
                {
                    of.SaveChanges();
                    Settings.Default.Save();

                    // Sync changes to UI
                    this.LoadQuickExtractPaths();
                    this.UpdateColumns();
                }
            }
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox ab = new AboutBox())
            {
                ab.ShowDialog(this);
            }
        }

        #endregion

        #region contextMenu1

        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            bool hasSelectedItems = lvFiles.SelectedIndices.Count > 0;

            quickExtractsMenuItem.Enabled = hasSelectedItems;
            copyPathMenuItem1.Enabled = hasSelectedItems;
            copyFolderPathMenuItem1.Enabled = hasSelectedItems;
            copyFileNameMenuItem1.Enabled = hasSelectedItems;
        }

        private void quickExtractMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            QuickExtractPath path = menuItem.Tag as QuickExtractPath;

            if (!Directory.Exists(path.Path))
            {
                DialogResult result = MessageBox.Show(
                    this,
                    string.Format("{0} path doesn't exists anymore. Do you want to create it?", path.Name),
                    "Quick Extract",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return;

                Directory.CreateDirectory(path.Path);
            }

            var files = new List<BSAFileEntry>();

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

        #endregion

        /// <summary>
        /// Opens the given BSA archive, adding it to the TreeView and making it browsable.
        /// </summary>
        /// <param name="path">The BSA archive's file path.</param>
        /// <param name="addToRecentFiles">True if BSA archive should be added to recent files list.</param>
        public void OpenArchive(string path, bool addToRecentFiles = false)
        {
            BSAFileEntry[] Files;
            BSATreeNode newNode = new BSATreeNode(Path.GetFileNameWithoutExtension(path));

            try
            {
                newNode.BinaryReader = new BinaryReader(File.OpenRead(path), Encoding.Default);
                //if(Program.ReadCString(br)!="BSA") throw new fommException("File was not a valid BSA archive");
                uint type = newNode.BinaryReader.ReadUInt32();
                StringBuilder sb = new StringBuilder(64);
                if (type != 0x00415342 && type != 0x00000100)
                {
                    //Might be a fallout 2 dat
                    newNode.BinaryReader.BaseStream.Position = newNode.BinaryReader.BaseStream.Length - 8;
                    uint TreeSize = newNode.BinaryReader.ReadUInt32();
                    uint DataSize = newNode.BinaryReader.ReadUInt32();

                    if (DataSize != newNode.BinaryReader.BaseStream.Length)
                    {
                        MessageBox.Show("File is not a valid bsa archive");
                        newNode.BinaryReader.Close();
                        return;
                    }

                    newNode.BinaryReader.BaseStream.Position = DataSize - TreeSize - 8;
                    int FileCount = newNode.BinaryReader.ReadInt32();
                    Files = new BSAFileEntry[FileCount];

                    for (int i = 0; i < FileCount; i++)
                    {
                        int fileLen = newNode.BinaryReader.ReadInt32();
                        for (int j = 0; j < fileLen; j++) sb.Append(newNode.BinaryReader.ReadChar());
                        byte comp = newNode.BinaryReader.ReadByte();
                        uint realSize = newNode.BinaryReader.ReadUInt32();
                        uint compSize = newNode.BinaryReader.ReadUInt32();
                        uint offset = newNode.BinaryReader.ReadUInt32();
                        if (sb[0] == '\\') sb.Remove(0, 1);
                        Files[i] = new BSAFileEntry(sb.ToString(), offset, compSize, comp == 0 ? 0 : realSize);
                        sb.Length = 0;
                    }
                }
                else if (type == 0x0100)
                {
                    uint hashoffset = newNode.BinaryReader.ReadUInt32();
                    uint FileCount = newNode.BinaryReader.ReadUInt32();
                    Files = new BSAFileEntry[FileCount];

                    uint dataoffset = 12 + hashoffset + FileCount * 8;
                    uint fnameOffset1 = 12 + FileCount * 8;
                    uint fnameOffset2 = 12 + FileCount * 12;

                    for (int i = 0; i < FileCount; i++)
                    {
                        newNode.BinaryReader.BaseStream.Position = 12 + i * 8;
                        uint size = newNode.BinaryReader.ReadUInt32();
                        uint offset = newNode.BinaryReader.ReadUInt32() + dataoffset;
                        newNode.BinaryReader.BaseStream.Position = fnameOffset1 + i * 4;
                        newNode.BinaryReader.BaseStream.Position = newNode.BinaryReader.ReadInt32() + fnameOffset2;

                        sb.Length = 0;

                        while (true)
                        {
                            char b = newNode.BinaryReader.ReadChar();
                            if (b == '\0') break;
                            sb.Append(b);
                        }

                        Files[i] = new BSAFileEntry(sb.ToString(), offset, size);
                    }
                }
                else
                {
                    int version = newNode.BinaryReader.ReadInt32();

                    if (version != 0x67 && version != 0x68)
                    {
                        if (MessageBox.Show("This BSA archive has an unknown version number.\n" +
                                            "Attempt to open anyway?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            newNode.BinaryReader.Close();
                            return;
                        }
                    }

                    newNode.BinaryReader.BaseStream.Position += 4;
                    uint flags = newNode.BinaryReader.ReadUInt32();
                    newNode.Compressed = ((flags & 0x004) > 0);
                    newNode.ContainsFileNameBlobs = ((flags & 0x100) > 0 && version == 0x68);
                    int FolderCount = newNode.BinaryReader.ReadInt32();
                    int FileCount = newNode.BinaryReader.ReadInt32();
                    newNode.BinaryReader.BaseStream.Position += 12;
                    Files = new BSAFileEntry[FileCount];
                    int[] numfiles = new int[FolderCount];
                    newNode.BinaryReader.BaseStream.Position += 8;

                    for (int i = 0; i < FolderCount; i++)
                    {
                        numfiles[i] = newNode.BinaryReader.ReadInt32();
                        newNode.BinaryReader.BaseStream.Position += 12;
                    }

                    newNode.BinaryReader.BaseStream.Position -= 8;
                    int filecount = 0;

                    for (int i = 0; i < FolderCount; i++)
                    {
                        int k = newNode.BinaryReader.ReadByte();
                        while (--k > 0) sb.Append(newNode.BinaryReader.ReadChar());
                        newNode.BinaryReader.BaseStream.Position++;
                        string folder = sb.ToString();

                        for (int j = 0; j < numfiles[i]; j++)
                        {
                            newNode.BinaryReader.BaseStream.Position += 8;
                            uint size = newNode.BinaryReader.ReadUInt32();
                            bool comp = newNode.Compressed;

                            if ((size & (1 << 30)) != 0)
                            {
                                comp = !comp;
                                size ^= 1 << 30;
                            }
                            Files[filecount++] = new BSAFileEntry(comp, folder, newNode.BinaryReader.ReadUInt32(), size);
                        }
                        sb.Length = 0;
                    }

                    for (int i = 0; i < FileCount; i++)
                    {
                        while (true)
                        {
                            char c = newNode.BinaryReader.ReadChar();

                            if (c == '\0')
                                break;

                            sb.Append(c);
                        }
                        Files[i].FileName = sb.ToString();
                        sb.Length = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                if (newNode.BinaryReader != null)
                    newNode.BinaryReader.Close();

                newNode.BinaryReader = null;
                MessageBox.Show("An error occured trying to open the archive.\n" + ex.Message);
                return;
            }

            MenuItem mi = new MenuItem("Close");
            mi.Tag = newNode;
            mi.Click += delegate (object sender, EventArgs e)
            {
                CloseArchive(newNode);
                if (tvFolders.Nodes.Count == 0)
                {
                    lvFiles.BeginUpdate();
                    _files.Clear();
                    lvFiles.EndUpdate();
                }
                else
                    txtSearch_DoSearch(null, null);
            };
            ContextMenu cm = new ContextMenu(new MenuItem[] { mi });
            newNode.ContextMenu = cm;
            newNode.Files = Files;
            newNode.Nodes.Add("empty");
            tvFolders.Nodes.Add(newNode);

            if (newNode.IsExpanded)
                newNode.Collapse();

            txtSearch.Text = "";
            btnExtract.Enabled = true;
            btnExtractAll.Enabled = true;
            btnPreview.Enabled = true;

            if (addToRecentFiles)
                AddToRecentFiles(path);

            tvFolders.SelectedNode = newNode;
        }

        /// <summary>
        /// Adds the given file to the recent files list. If it already exists in the list, it gets bumped up to the top.
        /// </summary>
        /// <param name="file">The file to add.</param>
        private void AddToRecentFiles(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            if (RecentListContains(file))
            {
                MenuItem item = RecentListGetItemByString(file);

                if (item == null)
                    return;

                int index = recentFilesMenuItem.MenuItems.IndexOf(item);
                recentFilesMenuItem.MenuItems.Remove(item);
                recentFilesMenuItem.MenuItems.Add(2, item);
            }
            else
            {
                MenuItem newItem = new MenuItem(Path.GetFileName(file), new EventHandler(recentFiles_Click));
                newItem.Tag = file;
                recentFilesMenuItem.MenuItems.Add(2, newItem);
            }
        }

        /// <summary>
        /// Closes the given BSA archive, removing it from the TreeView.
        /// </summary>
        /// <param name="bsaNode"></param>
        private void CloseArchive(BSATreeNode bsaNode)
        {
            if (GetSelectedArchive() == bsaNode)
            {
                lvFiles.BeginUpdate();
                _files.Clear();
                lvFiles.EndUpdate();
            }

            if (bsaNode.BinaryReader != null)
                bsaNode.BinaryReader.Close();

            tvFolders.Nodes.Remove(bsaNode);

            if (tvFolders.GetNodeCount(false) == 0)
            {
                btnPreview.Enabled = false;
                btnExtract.Enabled = false;
                btnExtractAll.Enabled = false;
            }
        }

        /// <summary>
        /// Closes all open archives, clearing the TreeView.
        /// </summary>
        private void CloseArchives()
        {
            lvFiles.BeginUpdate();
            _files.Clear();
            lvFiles.EndUpdate();

            foreach (BSATreeNode node in tvFolders.Nodes)
            {
                if (node.BinaryReader != null)
                    node.BinaryReader.Close();
            }

            tvFolders.Nodes.Clear();
        }

        /// <summary>
        /// Extracts the given file(s) to the given path.
        /// </summary>
        /// <param name="folder">The path to extract files to.</param>
        /// <param name="useFolderPath">True to use full folder path for files, false to extract straight to path.</param>
        /// <param name="gui">True to show a progression dialog.</param>
        /// <param name="files">The files in the selected BSA archive to extract.</param>
        private void ExtractFiles(string folder, bool useFolderPath, bool gui, params BSAFileEntry[] files)
        {
            if (gui)
            {
                pf = new ProgressForm("Unpacking archive", false);
                pf.EnableCancel();
                pf.SetProgressRange(files.Length);
                pf.Canceled += delegate { bw.CancelAsync(); };
                pf.Show(this);

                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += bw_DoWork;
                bw.ProgressChanged += bw_ProgressChanged;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.RunWorkerAsync(new object[] { folder, useFolderPath, files, GetSelectedArchive() });
            }
            else
            {
                try
                {
                    BSATreeNode root = GetSelectedArchive();

                    foreach (BSAFileEntry fe in files)
                    {
                        string path = useFolderPath ? folder : Path.Combine(folder, fe.FileName);

                        fe.Extract(path, useFolderPath, root.BinaryReader, root.ContainsFileNameBlobs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        #region ExtractFiles variables

        BackgroundWorker bw;
        ProgressForm pf;

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] arguments = e.Argument as object[];

            string folder = (arguments)[0] as string;
            bool useFolderPath = bool.Parse((arguments)[1].ToString());
            BSAFileEntry[] files = (arguments)[2] as BSAFileEntry[];
            BSATreeNode root = (arguments)[3] as BSATreeNode;

            try
            {
                int count = 0;

                foreach (BSAFileEntry fe in files)
                {
                    if (bw.CancellationPending)
                    {
                        e.Result = false;
                        break;
                    }

                    string path = useFolderPath ? folder : Path.Combine(folder, fe.FileName);

                    fe.Extract(path, useFolderPath, root.BinaryReader, root.ContainsFileNameBlobs);
                    bw.ReportProgress(count++);
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pf.UpdateProgress(e.ProgressPercentage);
            this.Text = string.Format("{0}% - {1}", pf.GetProgressPercentage(), _untouchedTitle);
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

            if (e.Result is bool)
            {
                if (!(bool)e.Result)
                {
                    MessageBox.Show("Operation cancelled", "Message");
                }
            }
            else if (e.Result is Exception)
            {
                MessageBox.Show(((Exception)e.Result).Message, "Error");
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
        private BSATreeNode GetRootNode(TreeNode node)
        {
            TreeNode rootNode = node;
            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;
            return (BSATreeNode)rootNode;
        }

        /// <summary>
        /// Returns the selected BSA archive.
        /// </summary>
        private BSATreeNode GetSelectedArchive()
        {
            if (tvFolders.SelectedNode == null)
                return null;

            return GetRootNode(tvFolders.SelectedNode);
        }

        /// <summary>
        /// Loads quick extract paths into Quick Extract menu item.
        /// </summary>
        private void LoadQuickExtractPaths()
        {
            quickExtractsMenuItem.MenuItems.Clear();

            foreach (QuickExtractPath path in Settings.Default.QuickExtractPaths)
            {
                MenuItem menuItem = new MenuItem(path.Name, quickExtractMenuItem_Click);

                menuItem.Tag = path;

                quickExtractsMenuItem.MenuItems.Add(menuItem);
            }
        }

        /// <summary>
        /// Returns true if recent files list contains the given file, false otherwise.
        /// </summary>
        /// <param name="file">The file to check.</param>
        private bool RecentListContains(string file)
        {
            foreach (MenuItem item in recentFilesMenuItem.MenuItems)
                if (item.Tag != null && item.Tag.ToString() == file) return true;
            return false;
        }

        /// <summary>
        /// Returns the given file's MenuItem.
        /// </summary>
        /// <param name="file">The file to get MenuItem from.</param>
        private MenuItem RecentListGetItemByString(string file)
        {
            foreach (MenuItem item in recentFilesMenuItem.MenuItems)
                if (item.Tag != null && item.Tag.ToString() == file) return item;

            return null;
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

            for (int i = recentFilesMenuItem.MenuItems.Count - 1; i != 1; i--)
                Settings.Default.RecentFiles.Add(recentFilesMenuItem.MenuItems[i].Tag.ToString());
        }

        /// <summary>
        /// Sorts all nodes in given TreeNode.
        /// </summary>
        /// <param name="rootNode">The TreeNode whose children is to be sorted.</param>
        private void SortNodes(TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                TreeNode[] nodes = new TreeNode[node.Nodes.Count];

                node.Nodes.CopyTo(nodes, 0);

                Array.Sort<TreeNode>(nodes, new TreeNodeSorter());

                node.Nodes.Clear();
                node.Nodes.AddRange(nodes);

                SortNodes(node);
            }
        }

        /// <summary>
        /// Shows or hides additional columns according to settings.
        /// </summary>
        private void UpdateColumns()
        {
            if (_extraColumns == null)
                _extraColumns = new ColumnHeader[] { columnHeader2, columnHeader3, columnHeader4 };

            if (Settings.Default.MoreColumns)
            {
                if (lvFiles.Columns.Count > 1)
                    return;

                lvFiles.BeginUpdate();
                lvFiles.Columns.AddRange(_extraColumns);
                lvFiles.EndUpdate();
            }
            else
            {
                foreach (ColumnHeader column in _extraColumns)
                    lvFiles.Columns.Remove(column);
            }
        }
    }

    public class BSASorter : System.Collections.Generic.Comparer<BSAFileEntry>
    {
        internal static BSASortOrder order = 0;
        internal static bool desc = true;

        public static void SetSorter(BSASortOrder sortOrder, bool sortDesc)
        {
            order = sortOrder;
            desc = sortDesc;
        }

        public override int Compare(BSAFileEntry a, BSAFileEntry b)
        {
            BSAFileEntry fa = a;
            BSAFileEntry fb = b;
            switch (order)
            {
                case BSASortOrder.FolderName:
                    return (desc) ? string.Compare(fa.LowerName, fb.LowerName) : string.Compare(fb.LowerName, fa.LowerName);
                case BSASortOrder.FileName:
                    return (desc) ? string.Compare(fa.FileName, fb.FileName) : string.Compare(fb.FileName, fa.FileName);
                case BSASortOrder.FileSize:
                    return (desc) ? fa.Size.CompareTo(fb.Size) : fb.Size.CompareTo(fa.Size);
                case BSASortOrder.Offset:
                    return (desc) ? fa.Offset.CompareTo(fb.Offset) : fb.Offset.CompareTo(fa.Offset);
                case BSASortOrder.FileType:
                    return (desc) ? string.Compare(Path.GetExtension(fa.FileName), Path.GetExtension(fb.FileName)) :
                                    string.Compare(Path.GetExtension(fb.FileName), Path.GetExtension(fa.FileName));
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
                if (b == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (b == null)
                {
                    return 1;
                }
                else
                {
                    return a.Text.CompareTo(b.Text);
                }
            }
        }
    }
}