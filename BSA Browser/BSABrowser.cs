using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
        private bool Compressed;
        private bool ContainsFileNameBlobs;
        ColumnHeader m_SortingColumn;

        public BSABrowser()
        {
            InitializeComponent();
            lvFiles.ContextMenu = contextMenu1;

            string path = Properties.Settings.Default.LastBSAUnpackPath;

            if (!string.IsNullOrEmpty(path))
                SaveAllDialog.SelectedPath = path;

            if (Properties.Settings.Default.RecentFiles != null)
            {
                foreach (string item in Properties.Settings.Default.RecentFiles)
                    AddToRecentFiles(item);
            }

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
            this.Location = Properties.Settings.Default.BSABrowser_Location;
            this.Size = Properties.Settings.Default.BSABrowser_Size;
            splitContainer1.SplitterDistance = Properties.Settings.Default.BSABrowser_SplitterDistance;
            cmbSortOrder.SelectedIndex = 0;
        }

        private void BSABrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tvFolders.GetNodeCount(false) > 0)
                CloseArchives();

            SaveRecentFiles();
            Properties.Settings.Default.BSABrowser_Location = this.Location;
            Properties.Settings.Default.BSABrowser_Size = this.Size;
            Properties.Settings.Default.LastBSAUnpackPath = SaveAllDialog.SelectedPath;
            Properties.Settings.Default.BSABrowser_SplitterDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.Save();
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
                    fe.Extract(SaveSingleDialog.FileName, false, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
            }
            else
            {
                if (SaveAllDialog.ShowDialog() == DialogResult.OK)
                {
                    ProgressForm pf = new ProgressForm("Unpacking archive", false);
                    pf.EnableCancel();
                    pf.SetProgressRange(lvFiles.SelectedItems.Count);
                    pf.Show();
                    int count = 0;

                    try
                    {
                        foreach (ListViewItem lvi in lvFiles.SelectedItems)
                        {
                            BSAFileEntry fe = (BSAFileEntry)lvi.Tag;
                            fe.Extract(SaveAllDialog.SelectedPath, true, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
                            pf.UpdateProgress(count++);
                            Application.DoEvents();
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

                    pf.Unblock();
                    pf.Close();
                }
            }
        }

        private void btnExtractAll_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode == null)
                return;

            BSAFileEntry[] Files = (BSAFileEntry[])GetRootNode(tvFolders.SelectedNode).Files;
            if (SaveAllDialog.ShowDialog() == DialogResult.OK)
            {
                ProgressForm pf = new ProgressForm("Unpacking archive", false);
                pf.EnableCancel();
                pf.SetProgressRange(Files.Length);
                pf.Show();
                int count = 0;

                try
                {
                    foreach (BSAFileEntry fe in Files)
                    {
                        fe.Extract(SaveAllDialog.SelectedPath, true, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
                        pf.UpdateProgress(count++);
                        Application.DoEvents();
                    }
                }
                catch (fommCancelException)
                {
                    MessageBox.Show("Operation cancelled", "Message");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }

                pf.Unblock();
                pf.Close();
            }
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            lvFiles.ListViewItemSorter = new BSASorter();
            lvFiles.ListViewItemSorter = null;
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
                        fe.Extract(Path.Combine(path, fe.FileName), false, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
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
        }

        private void cbDesc_CheckedChanged(object sender, EventArgs e)
        {
            BSASorter.SetSorter((BSASortOrder)cmbSortOrder.SelectedIndex, cbDesc.Checked);
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
            System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();

            foreach (ListViewItem item in lvFiles.SelectedItems)
            {
                BSAFileEntry fe = (BSAFileEntry)item.Tag;
                string path = Path.Combine(Program.CreateTempDirectory(), fe.FileName);
                fe.Extract(path, false, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
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
            if (!(tvFolders.GetNodeCount(false) > 0) ||
                tvFolders.SelectedNode == null)
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
                List<ListViewItem> lvis = new List<ListViewItem>(GetRootNode(tvFolders.SelectedNode).Files.Length);

                for (int i = 0; i < GetRootNode(tvFolders.SelectedNode).Items.Length; i++)
                    if (regex.IsMatch(GetRootNode(tvFolders.SelectedNode).Items[i].Text))
                        lvis.Add(GetRootNode(tvFolders.SelectedNode).Items[i]);

                lvFiles.Items.AddRange(lvis.ToArray());
                lvFiles.EndUpdate();
            }
            else
            {
                str = str.ToLowerInvariant();
                lvFiles.BeginUpdate();
                lvFiles.Items.Clear();

                if (str.Length == 0)
                    lvFiles.Items.AddRange(GetRootNode(tvFolders.SelectedNode).Items);
                else
                {
                    List<ListViewItem> lvis = new List<ListViewItem>(GetRootNode(tvFolders.SelectedNode).Files.Length);

                    for (int i = 0; i < GetRootNode(tvFolders.SelectedNode).Items.Length; i++)
                        if (GetRootNode(tvFolders.SelectedNode).Items[i].Text.Contains(str))
                            lvis.Add(GetRootNode(tvFolders.SelectedNode).Items[i]);

                    lvFiles.Items.AddRange(lvis.ToArray());
                }
                lvFiles.EndUpdate();
            }
        }

        private void tvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (GetRootNode(e.Node).AllItems != null)
                return;

            e.Node.Nodes.Clear();
            System.Collections.Generic.Dictionary<string, TreeNode> nodes = new System.Collections.Generic.Dictionary<string, TreeNode>();
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
                System.Collections.Generic.List<ListViewItem> lvis = new System.Collections.Generic.List<ListViewItem>(GetRootNode(e.Node).AllItems.Length);
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

        private void closeSelArchiveMenuItem_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode == null)
                return;

            if (GetRootNode(tvFolders.SelectedNode) == null)
                return;

            CloseArchive(GetRootNode(tvFolders.SelectedNode));
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
            //fileToolStripMenuItem.DropDown.Close();

            if (File.Exists(item.Tag.ToString()))
            {
                OpenArchive(item.Tag.ToString());
                recentFilesMenuItem.MenuItems.Remove(item);
                recentFilesMenuItem.MenuItems.Add(2, item);
            }
            else
                if (MessageBox.Show(this, "\"" + item.Tag.ToString() + "\" dosen't exist anymore." + Environment.NewLine + Environment.NewLine +
                                          "Want to remove from recent list?", "Lost File", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    recentFilesMenuItem.MenuItems.Remove(item);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem2_Popup(object sender, EventArgs e)
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
            OptionsForm of = new OptionsForm();
            if (of.ShowDialog(this) == DialogResult.OK)
            {
                Settings.Default.Fallout3_QuickExportPath = of.txtFallout3Path.Text;
                Settings.Default.FalloutNV_QuickExportPath = of.txtFalloutNVPath.Text;
                Settings.Default.Oblivion_QuickExportPath = of.txtOblivionPath.Text;
                Settings.Default.Skyrim_QuickExportPath = of.txtSkyrimPath.Text;
                Settings.Default.Save();
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
            extractFallout3MenuItem1.Enabled = Settings.Default.Fallout3_QuickExportEnable;
            extractFalloutNewVegasMenuItem1.Enabled = Settings.Default.Fallout3_QuickExportEnable;
            extractOblivionMenuItem1.Enabled = Settings.Default.Fallout3_QuickExportEnable;
            extractSkyrimMenuItem1.Enabled = Settings.Default.Fallout3_QuickExportEnable;
        }

        private void extractFallout3MenuItem1_Click(object sender, EventArgs e)
        {
            ProgressForm pf = new ProgressForm("Unpacking archive", false);
            pf.EnableCancel();
            pf.SetProgressRange(lvFiles.SelectedItems.Count);
            pf.Show();
            int count = 0;

            try
            {
                foreach (ListViewItem lvi in lvFiles.SelectedItems)
                {
                    BSAFileEntry fe = (BSAFileEntry)lvi.Tag;
                    fe.Extract(Settings.Default.Fallout3_QuickExportPath + @"\Data\", true, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
                    pf.UpdateProgress(count++);
                    Application.DoEvents();
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

            pf.Unblock();
            pf.Close();
        }

        private void extractFalloutNewVegasMenuItem1_Click(object sender, EventArgs e)
        {
            ProgressForm pf = new ProgressForm("Unpacking archive", false);
            pf.EnableCancel();
            pf.SetProgressRange(lvFiles.SelectedItems.Count);
            pf.Show();
            int count = 0;

            try
            {
                foreach (ListViewItem lvi in lvFiles.SelectedItems)
                {
                    BSAFileEntry fe = (BSAFileEntry)lvi.Tag;
                    fe.Extract(Settings.Default.FalloutNV_QuickExportPath + @"\Data\", true, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
                    pf.UpdateProgress(count++);
                    Application.DoEvents();
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

            pf.Unblock();
            pf.Close();
        }

        private void extractOblivionMenuItem1_Click(object sender, EventArgs e)
        {
            ProgressForm pf = new ProgressForm("Unpacking archive", false);
            pf.EnableCancel();
            pf.SetProgressRange(lvFiles.SelectedItems.Count);
            pf.Show();
            int count = 0;

            try
            {
                foreach (ListViewItem lvi in lvFiles.SelectedItems)
                {
                    BSAFileEntry fe = (BSAFileEntry)lvi.Tag;
                    fe.Extract(Settings.Default.Oblivion_QuickExportPath + @"\Data\", true, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
                    pf.UpdateProgress(count++);
                    Application.DoEvents();
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

            pf.Unblock();
            pf.Close();
        }

        private void extractSkyrimMenuItem1_Click(object sender, EventArgs e)
        {
            ProgressForm pf = new ProgressForm("Unpacking archive", false);
            pf.EnableCancel();
            pf.SetProgressRange(lvFiles.SelectedItems.Count);
            pf.Show();
            int count = 0;

            try
            {
                foreach (ListViewItem lvi in lvFiles.SelectedItems)
                {
                    BSAFileEntry fe = (BSAFileEntry)lvi.Tag;
                    fe.Extract(Settings.Default.Skyrim_QuickExportPath + @"\Data\", true, GetRootNode(tvFolders.SelectedNode).BinaryReader, ContainsFileNameBlobs);
                    pf.UpdateProgress(count++);
                    Application.DoEvents();
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

            pf.Unblock();
            pf.Close();
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

        public string FormatBytes(long bytes)
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

        private BSATreeNode GetRootNode(TreeNode node)
        {
            TreeNode rootNode = node;
            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;
            return (BSATreeNode)rootNode;
        }

        private bool RecentListContains(string file)
        {
            foreach (MenuItem item in recentFilesMenuItem.MenuItems)
                if (item.Tag != null && item.Tag.ToString() == file) return true;
            return false;
        }

        private MenuItem RecentListGetItemByString(string file)
        {
            foreach (MenuItem item in recentFilesMenuItem.MenuItems)
                if (item.Tag != null && item.Tag.ToString() == file) return item;

            return null;
        }

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

        private void CloseArchive(BSATreeNode bsaNode)
        {
            if (GetRootNode(tvFolders.SelectedNode) == bsaNode)
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

        private void CloseArchives()
        {
            lvFiles.Items.Clear();
            tvFolders.Nodes.Clear();
        }

        internal void OpenArchive(string path, bool addToRecentFiles = false)
        {
            BSAFileEntry[] Files;
            BSATreeNode newNode = new BSATreeNode(Path.GetFileNameWithoutExtension(path));

            try
            {
                newNode.BinaryReader = new BinaryReader(File.OpenRead(path), System.Text.Encoding.Default);
                //if(Program.ReadCString(br)!="BSA") throw new fommException("File was not a valid BSA archive");
                uint type = newNode.BinaryReader.ReadUInt32();
                System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
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
                    if ((flags & 0x004) > 0) Compressed = true; else Compressed = false;
                    if ((flags & 0x100) > 0 && version == 0x68) ContainsFileNameBlobs = true; else ContainsFileNameBlobs = false;
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
                            bool comp = Compressed;

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

        private void SaveRecentFiles()
        {
            if (Settings.Default.RecentFiles == null)
                Settings.Default.RecentFiles = new StringCollection();

            Settings.Default.RecentFiles.Clear();

            for (int i = recentFilesMenuItem.MenuItems.Count - 1; i != 1; i--)
                Settings.Default.RecentFiles.Add(recentFilesMenuItem.MenuItems[i].Tag.ToString());
        }

        private void UpdateFileList(BSATreeNode bsaNode)
        {
            bsaNode.Items = new ListViewItem[bsaNode.Files.Length];

            for (int i = 0; i < bsaNode.Files.Length; i++)
            {
                BSAFileEntry file = bsaNode.Files[i];
                ListViewItem lvi = new ListViewItem(Path.Combine(file.Folder, file.FileName));

                lvi.Tag = file;
                lvi.ToolTipText = "File size: " + FormatBytes(file.Size) + "\nFile offset: " + FormatBytes(file.Offset) + (file.Compressed ? "\nCompressed" : "\nUncompressed");
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