using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BSA_Browser.Classes;
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
        ColumnHeader m_SortingColumn;
        Settings _settings = Properties.Settings.Default;

        public BSABrowser()
        {
            InitializeComponent();
            this.Text += " (" + Program.GetVersion() + ")";
            lvFiles.ContextMenu = contextMenu1;

            if (_settings.UpdateSettings)
            {
                _settings.Upgrade();
                _settings.UpdateSettings = false;
                _settings.Save();
            }

            string path = _settings.LastBSAUnpackPath;

            if (!string.IsNullOrEmpty(path))
                SaveAllDialog.SelectedPath = path;

            if (_settings.RecentFiles != null)
            {
                foreach (string item in _settings.RecentFiles)
                    AddToRecentFiles(item);
            }

            // Set lvFiles sorter
            BSASorter.SetSorter(_settings.SortType, _settings.SortDesc);
            lvFiles.ListViewItemSorter = new BSASorter();

            Program.SetWindowTheme(tvFolders.Handle, "explorer", null);
            Program.SendMessage(lvFiles.Handle, 0x127, 0x10001, 0);
            Program.SetWindowTheme(lvFiles.Handle, "explorer", null);
            Program.SendMessage(lvFiles.Handle, 0x1000 + 54, 0x00010000, 0x00010000);
            Program.SendMessage(txtSearch.Handle, 0x1500 + 1, IntPtr.Zero.ToInt32(), "Enter a Filter");
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
            if (_settings.WindowStates == null)
            {
                _settings.WindowStates = new WindowStates();
            }

            // Add this form if it doesn't exists
            if (!_settings.WindowStates.Contains(this.Name))
            {
                _settings.WindowStates.Add(this.Name);
            }

            // Restore window state
            _settings.WindowStates[this.Name].RestoreForm(this);

            // Restore sorting preferences
            cmbSortOrder.SelectedIndex = (int)_settings.SortType;
            cbDesc.Checked = _settings.SortDesc;
        }

        private void BSABrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tvFolders.GetNodeCount(false) > 0)
                CloseArchives();

            SaveRecentFiles();
            _settings.WindowStates[this.Name].SaveForm(this);
            _settings.LastBSAUnpackPath = SaveAllDialog.SelectedPath;
            _settings.Save();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (OpenBSA.ShowDialog() == DialogResult.OK)
                OpenArchive(OpenBSA.FileName, true);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            if (lvFiles.SelectedItems.Count == 1)
            {
                BSAFileEntry fe = (BSAFileEntry)lvFiles.SelectedItems[0].Tag;
                SaveSingleDialog.FileName = fe.FileName;

                if (SaveSingleDialog.ShowDialog() == DialogResult.OK)
                    ExtractFiles(SaveSingleDialog.FileName, false, false, fe);
            }
            else
            {
                if (SaveAllDialog.ShowDialog() == DialogResult.OK)
                {
                    BSAFileEntry[] files = new BSAFileEntry[lvFiles.SelectedItems.Count];

                    for (int i = 0; i < lvFiles.SelectedItems.Count; i++)
                    {
                        files[i] = (BSAFileEntry)lvFiles.SelectedItems[i].Tag;
                    }

                    ExtractFiles(SaveAllDialog.SelectedPath, true, true, files);
                }
            }
        }

        private void btnExtractAll_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode == null)
                return;

            BSAFileEntry[] files = (BSAFileEntry[])GetSelectedArchive().Files;

            if (SaveAllDialog.ShowDialog() == DialogResult.OK)
            {
                ExtractFiles(SaveAllDialog.SelectedPath, true, true, files);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            if (lvFiles.SelectedItems.Count == 1)
            {
                BSAFileEntry fe = (BSAFileEntry)lvFiles.SelectedItems[0].Tag;

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
            lvFiles.Sort();
            _settings.SortType = (BSASortOrder)cmbSortOrder.SelectedIndex;
        }

        private void cbDesc_CheckedChanged(object sender, EventArgs e)
        {
            BSASorter.SetSorter((BSASortOrder)cmbSortOrder.SelectedIndex, cbDesc.Checked);
            lvFiles.Sort();
            _settings.SortDesc = cbDesc.Checked;
        }

        private void lvFiles_Enter(object sender, EventArgs e)
        {
            Program.SendMessage(lvFiles.Handle, 0x127, 0x10001, 0);
        }

        private void lvFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!(lvFiles.SelectedItems.Count >= 1))
                return;

            DataObject obj = new DataObject();
            StringCollection sc = new StringCollection();

            foreach (ListViewItem item in lvFiles.SelectedItems)
            {
                BSAFileEntry fe = (BSAFileEntry)item.Tag;
                string path = Path.Combine(Program.CreateTempDirectory(), fe.FileName);
                BSATreeNode root = GetSelectedArchive();

                fe.Extract(path, false, root.BinaryReader, root.ContainsFileNameBlobs);
                sc.Add(path);
            }

            obj.SetFileDropList(sc);
            lvFiles.DoDragDrop(obj, DragDropEffects.Move);
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.SendMessage(lvFiles.Handle, 0x127, 0x10001, 0);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!(tvFolders.GetNodeCount(false) > 0) || tvFolders.SelectedNode == null)
                return;

            string str = txtSearch.Text;

            if (cbRegex.Checked && str.Length > 0)
            {
                Regex regex;

                try
                {
                    regex = new Regex(str, RegexOptions.Singleline);
                }
                catch { return; }

                lvFiles.BeginUpdate();
                lvFiles.Items.Clear();
                List<ListViewItem> lvis = new List<ListViewItem>(GetSelectedArchive().Files.Length);

                for (int i = 0; i < GetSelectedArchive().Items.Length; i++)
                    if (regex.IsMatch(GetSelectedArchive().Items[i].Text))
                        lvis.Add(GetSelectedArchive().Items[i]);

                lvFiles.Items.AddRange(lvis.ToArray());
                lvFiles.EndUpdate();
            }
            else
            {
                str = str.ToLowerInvariant();
                lvFiles.BeginUpdate();
                lvFiles.Items.Clear();

                if (str.Length == 0)
                    lvFiles.Items.AddRange(GetSelectedArchive().Items);
                else
                {
                    List<ListViewItem> lvis = new List<ListViewItem>(GetSelectedArchive().Files.Length);

                    for (int i = 0; i < GetSelectedArchive().Items.Length; i++)
                        if (GetSelectedArchive().Items[i].Text.Contains(str))
                            lvis.Add(GetSelectedArchive().Items[i]);

                    lvFiles.Items.AddRange(lvis.ToArray());
                }
                lvFiles.EndUpdate();
            }

            lFileCount.Text = string.Format("{0:n0} files", lvFiles.Items.Count);
        }

        private void tvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (GetRootNode(e.Node).AllItems != null)
                return;

            e.Node.Nodes.Clear();
            Dictionary<string, TreeNode> nodes = new Dictionary<string, TreeNode>();
            GetRootNode(e.Node).AllItems = (ListViewItem[])GetRootNode(e.Node).Items.Clone();

            foreach (ListViewItem lvi in GetRootNode(e.Node).AllItems)
            {
                string path = Path.GetDirectoryName(lvi.Text);

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
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (GetRootNode(e.Node).AllItems == null)
                tvFolders_BeforeExpand(null, new TreeViewCancelEventArgs(e.Node, false, TreeViewAction.Unknown));

            string s = (string)e.Node.Tag;

            if (s == null)
                GetRootNode(e.Node).Items = GetRootNode(e.Node).AllItems;
            else
            {
                List<ListViewItem> lvis = new List<ListViewItem>(GetRootNode(e.Node).AllItems.Length);
                foreach (ListViewItem lvi in GetRootNode(e.Node).AllItems)
                    if (lvi.Text.StartsWith(s)) lvis.Add(lvi);

                GetRootNode(e.Node).Items = lvis.ToArray();
            }
            txtSearch_TextChanged(tvFolders, EventArgs.Empty);
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
            bool hasSelectedItems = lvFiles.SelectedItems.Count > 0;

            copyPathMenuItem.Enabled = hasSelectedItems;
            copyFolderPathMenuItem.Enabled = hasSelectedItems;
            copyFileNameMenuItem.Enabled = hasSelectedItems;
        }

        private void copyPathMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (ListViewItem file in lvFiles.SelectedItems)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(Environment.NewLine);

                builder.Append(file.Text);
            }

            Clipboard.SetText(builder.ToString());
        }

        private void copyFolderPathMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (ListViewItem file in lvFiles.SelectedItems)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(Environment.NewLine);

                builder.Append(Path.GetDirectoryName(file.Text));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void copyFileNameMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            foreach (ListViewItem file in lvFiles.SelectedItems)
            {
                if (!string.IsNullOrEmpty(builder.ToString()))
                    builder.Append(Environment.NewLine);

                builder.Append(Path.GetFileName(file.Text));
            }

            Clipboard.SetText(builder.ToString());
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (OptionsForm of = new OptionsForm())
            {
                if (of.ShowDialog(this) == DialogResult.OK)
                {
                    of.Save();
                    _settings.Save();
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
            bool hasSelectedItems = lvFiles.SelectedItems.Count > 0;

            menuItem1.Enabled = hasSelectedItems;
            copyPathMenuItem1.Enabled = hasSelectedItems;
            copyFolderPathMenuItem1.Enabled = hasSelectedItems;
            copyFileNameMenuItem1.Enabled = hasSelectedItems;
        }

        private void menuItem1_Popup(object sender, EventArgs e)
        {
            extractFallout3MenuItem1.Enabled = _settings.Fallout3_QuickExportEnable;
            extractFalloutNewVegasMenuItem1.Enabled = _settings.FalloutNV_QuickExportEnable;
            extractOblivionMenuItem1.Enabled = _settings.Oblivion_QuickExportEnable;
            extractSkyrimMenuItem1.Enabled = _settings.Skyrim_QuickExportEnable;
        }

        private void extractFallout3MenuItem1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_settings.Fallout3_QuickExportPath))
            {
                MessageBox.Show(this, "Fallout 3 path doesn't exists, or is not set.");
                return;
            }

            string path = _settings.Fallout3_QuickExportPath + "\\Data\\";
            BSAFileEntry[] files = new BSAFileEntry[lvFiles.SelectedItems.Count];

            for (int i = 0; i < lvFiles.SelectedItems.Count; i++)
            {
                files[i] = (BSAFileEntry)lvFiles.SelectedItems[i].Tag;
            }

            ExtractFiles(path, true, true, files);
        }

        private void extractFalloutNewVegasMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_settings.FalloutNV_QuickExportPath))
            {
                MessageBox.Show(this, "Fallout New Vegas path doesn't exists, or is not set.");
                return;
            }

            string path = _settings.FalloutNV_QuickExportPath + "\\Data\\";
            BSAFileEntry[] files = new BSAFileEntry[lvFiles.SelectedItems.Count];

            for (int i = 0; i < lvFiles.SelectedItems.Count; i++)
            {
                files[i] = (BSAFileEntry)lvFiles.SelectedItems[i].Tag;
            }

            ExtractFiles(path, true, true, files);
        }

        private void extractOblivionMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_settings.Oblivion_QuickExportPath))
            {
                MessageBox.Show(this, "Oblivion path doesn't exists, or is not set.");
                return;
            }

            string path = _settings.Oblivion_QuickExportPath + "\\Data\\";
            BSAFileEntry[] files = new BSAFileEntry[lvFiles.SelectedItems.Count];

            for (int i = 0; i < lvFiles.SelectedItems.Count; i++)
            {
                files[i] = (BSAFileEntry)lvFiles.SelectedItems[i].Tag;
            }

            ExtractFiles(path, true, true, files);
        }

        private void extractSkyrimMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(_settings.Skyrim_QuickExportPath))
            {
                MessageBox.Show(this, "Skyrim path doesn't exists, or is not set.");
                return;
            }

            string path = _settings.Skyrim_QuickExportPath + "\\Data\\";
            BSAFileEntry[] files = new BSAFileEntry[lvFiles.SelectedItems.Count];

            for (int i = 0; i < lvFiles.SelectedItems.Count; i++)
            {
                files[i] = (BSAFileEntry)lvFiles.SelectedItems[i].Tag;
            }

            ExtractFiles(path, true, true, files);
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
            mi.Click += delegate(object sender, EventArgs e)
            {
                CloseArchive(newNode);
                if (tvFolders.Nodes.Count == 0)
                    lvFiles.Items.Clear();
                else
                    txtSearch_TextChanged(null, null);
            };
            ContextMenu cm = new ContextMenu(new MenuItem[] { mi });
            newNode.ContextMenu = cm;
            newNode.Files = Files;
            newNode.Nodes.Add("empty");
            tvFolders.Nodes.Add(newNode);

            if (newNode.IsExpanded)
                newNode.Collapse();

            txtSearch.Text = "";
            UpdateFileList(newNode);
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
                lvFiles.Items.Clear();

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
            lvFiles.Items.Clear();

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
        /// <param name="path">The path to extract files to.</param>
        /// <param name="useFolderName">True to use full folder path for files, false to extract straight to path.</param>
        /// <param name="gui">True to show a progression dialog.</param>
        /// <param name="files">The files in the selected BSA archive to extract.</param>
        private void ExtractFiles(string path, bool useFolderName, bool gui, params BSAFileEntry[] files)
        {
            ProgressForm pf = null;
            int count = 0;

            if (gui)
            {
                pf = new ProgressForm("Unpacking archive", false);
                pf.EnableCancel();
                pf.SetProgressRange(files.Length);
                pf.Show();
            }

            try
            {
                BSATreeNode root = GetSelectedArchive();

                foreach (BSAFileEntry fe in files)
                {
                    fe.Extract(path, useFolderName, root.BinaryReader, root.ContainsFileNameBlobs);

                    if (gui)
                    {
                        pf.UpdateProgress(count++);
                        Application.DoEvents();
                    }
                }
            }
            catch (fommException)
            {
                MessageBox.Show("Operation cancelled", "Message");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            if (gui)
            {
                pf.Unblock();
                pf.Close();
            }
        }

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
            if (_settings.RecentFiles == null)
                _settings.RecentFiles = new StringCollection();
            else
                _settings.RecentFiles.Clear();

            for (int i = recentFilesMenuItem.MenuItems.Count - 1; i != 1; i--)
                _settings.RecentFiles.Add(recentFilesMenuItem.MenuItems[i].Tag.ToString());
        }

        /// <summary>
        /// Updates BSA archive file list.
        /// </summary>
        /// <param name="bsaNode">The BSA archive to update.</param>
        private void UpdateFileList(BSATreeNode bsaNode)
        {
            bsaNode.Items = new ListViewItem[bsaNode.Files.Length];

            for (int i = 0; i < bsaNode.Files.Length; i++)
            {
                BSAFileEntry file = bsaNode.Files[i];
                ListViewItem lvi = new ListViewItem(Path.Combine(file.Folder, file.FileName));

                lvi.Tag = file;
                lvi.ToolTipText =
                    "File size: " + FormatBytes(file.Size) + "\n" +
                    "File offset: " + file.Offset + " bytes\n" +
                    (file.Compressed ? "Compressed" : "Uncompressed");
                bsaNode.Items[i] = lvi;
            }

            if (bsaNode.IsSelected)
            {
                lvFiles.BeginUpdate();
                lvFiles.Items.AddRange((tvFolders.SelectedNode.FirstNode as BSATreeNode).Items);
                lvFiles.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvFiles.EndUpdate();
            }
        }
    }

    public class BSASorter : System.Collections.IComparer
    {
        internal static BSASortOrder order = 0;
        internal static bool desc = true;

        public static void SetSorter(BSASortOrder sortOrder, bool sortDesc)
        {
            order = sortOrder;
            desc = sortDesc;
        }

        public int Compare(object a, object b)
        {
            BSAFileEntry fa = (BSAFileEntry)((ListViewItem)a).Tag;
            BSAFileEntry fb = (BSAFileEntry)((ListViewItem)b).Tag;
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
}