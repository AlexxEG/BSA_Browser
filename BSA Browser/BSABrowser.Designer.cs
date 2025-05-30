﻿namespace BSA_Browser
{
    partial class BSABrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BSABrowser));
            this.lvFiles = new System.Windows.Forms.ListView();
            this.columnFilePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnArchive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnExtractAll = new System.Windows.Forms.Button();
            this.OpenArchiveDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnExtractAllFolders = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.foldersImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tlvFolders = new BrightIdeasSoftware.TreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cbRegex = new System.Windows.Forms.CheckBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.fileMenuItem = new System.Windows.Forms.MenuItem();
            this.openArchiveMenuItem = new System.Windows.Forms.MenuItem();
            this.closeSelectedArchiveMenuItem = new System.Windows.Forms.MenuItem();
            this.closeAllArchivesMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.extractArchivesMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.recentFilesMenuItem = new System.Windows.Forms.MenuItem();
            this.emptyListMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.editMenuItem = new System.Windows.Forms.MenuItem();
            this.copyMenuItem = new System.Windows.Forms.MenuItem();
            this.copyPathMenuItem = new System.Windows.Forms.MenuItem();
            this.copyFolderPathMenuItem = new System.Windows.Forms.MenuItem();
            this.copyFileNameMenuItem = new System.Windows.Forms.MenuItem();
            this.selectAllMenuItem = new System.Windows.Forms.MenuItem();
            this.toolsMenuItem = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.compareArchivesMenuItem = new System.Windows.Forms.MenuItem();
            this.openFoldersMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.checkForUpdateMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.githubMenuItem = new System.Windows.Forms.MenuItem();
            this.discordMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.fallout4NexusPageMenuItem = new System.Windows.Forms.MenuItem();
            this.skyrimSENexusPageMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.extractMenuItem = new System.Windows.Forms.MenuItem();
            this.extractFoldersMenuItem = new System.Windows.Forms.MenuItem();
            this.previewMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.quickExtractsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.copyMenuItem1 = new System.Windows.Forms.MenuItem();
            this.copyPathMenuItem1 = new System.Windows.Forms.MenuItem();
            this.copyFolderPathMenuItem1 = new System.Windows.Forms.MenuItem();
            this.copyFileNameMenuItem1 = new System.Windows.Forms.MenuItem();
            this.selectAllMenuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.compareMenuItem = new System.Windows.Forms.MenuItem();
            this.compareCancelMenuItem = new System.Windows.Forms.MenuItem();
            this.lFileCount = new System.Windows.Forms.Label();
            this.archiveContextMenu = new System.Windows.Forms.ContextMenu();
            this.extractAllFilesMenuItem = new System.Windows.Forms.MenuItem();
            this.extractAllFoldersMenuItem = new System.Windows.Forms.MenuItem();
            this.loadedSeparatorMenuItem1 = new System.Windows.Forms.MenuItem();
            this.reloadMenuItem = new System.Windows.Forms.MenuItem();
            this.openContainingFolderMenuItem = new System.Windows.Forms.MenuItem();
            this.loadedSeparatorMenuItem2 = new System.Windows.Forms.MenuItem();
            this.closeMenuItem = new System.Windows.Forms.MenuItem();
            this.alwaysHideSeparatorMenuItem1 = new System.Windows.Forms.MenuItem();
            this.removeLoadedMenuItem = new System.Windows.Forms.MenuItem();
            this.removeUnloadedMenuItem = new System.Windows.Forms.MenuItem();
            this.alwaysHideSeparatorMenuItem2 = new System.Windows.Forms.MenuItem();
            this.unloadedOpenContainingFolderMenuItem = new System.Windows.Forms.MenuItem();
            this.unloadedSeparatorMenuItem1 = new System.Windows.Forms.MenuItem();
            this.unloadedLoadMenuItem = new System.Windows.Forms.MenuItem();
            this.unloadedRemoveMenuItem = new System.Windows.Forms.MenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tlvFolders)).BeginInit();
            this.SuspendLayout();
            // 
            // lvFiles
            // 
            this.lvFiles.AllowDrop = true;
            this.lvFiles.AutoArrange = false;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFilePath,
            this.columnFileSize,
            this.columnArchive});
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.HideSelection = false;
            this.lvFiles.LabelWrap = false;
            this.lvFiles.Location = new System.Drawing.Point(0, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.ShowItemToolTips = true;
            this.lvFiles.Size = new System.Drawing.Size(499, 261);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.VirtualMode = true;
            this.lvFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvFiles_ColumnClick);
            this.lvFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvFiles_ItemDrag);
            this.lvFiles.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvFiles_RetrieveVirtualItem);
            this.lvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.File_DragDrop);
            this.lvFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.File_DragOver);
            this.lvFiles.DoubleClick += new System.EventHandler(this.lvFiles_DoubleClick);
            this.lvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvFiles_KeyDown);
            // 
            // columnFilePath
            // 
            this.columnFilePath.Text = "File Path";
            this.columnFilePath.Width = 227;
            // 
            // columnFileSize
            // 
            this.columnFileSize.Text = "File Size";
            this.columnFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnFileSize.Width = 80;
            // 
            // columnArchive
            // 
            this.columnArchive.Text = "Archive";
            // 
            // filesImageList
            // 
            this.filesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.filesImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.filesImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpen.Location = new System.Drawing.Point(12, 276);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnExtractAll
            // 
            this.btnExtractAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExtractAll.Enabled = false;
            this.btnExtractAll.Location = new System.Drawing.Point(168, 276);
            this.btnExtractAll.Name = "btnExtractAll";
            this.btnExtractAll.Size = new System.Drawing.Size(83, 23);
            this.btnExtractAll.TabIndex = 3;
            this.btnExtractAll.Text = "Extract";
            this.toolTip1.SetToolTip(this.btnExtractAll, "Extract all files currently shown directly into selected folder.");
            this.btnExtractAll.UseVisualStyleBackColor = true;
            this.btnExtractAll.Click += new System.EventHandler(this.btnExtractFiles_Click);
            // 
            // OpenArchiveDialog
            // 
            this.OpenArchiveDialog.Filter = "All supported files|*.bsa;*.ba2;*.dat|Fallout or Oblivion BSA archives|*.bsa|Fall" +
    "out 4 BA2 archives|*.ba2|Fallout 2 dat archive|*.dat";
            this.OpenArchiveDialog.Multiselect = true;
            this.OpenArchiveDialog.RestoreDirectory = true;
            this.OpenArchiveDialog.Title = "Select archive to open";
            this.OpenArchiveDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenArchiveDialog_FileOk);
            // 
            // btnExtractAllFolders
            // 
            this.btnExtractAllFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExtractAllFolders.Enabled = false;
            this.btnExtractAllFolders.Location = new System.Drawing.Point(257, 276);
            this.btnExtractAllFolders.Name = "btnExtractAllFolders";
            this.btnExtractAllFolders.Size = new System.Drawing.Size(126, 23);
            this.btnExtractAllFolders.TabIndex = 2;
            this.btnExtractAllFolders.Text = "Extract with folders";
            this.toolTip1.SetToolTip(this.btnExtractAllFolders, "Extract all files currently shown into selected folder with the folder tree intac" +
        "t.");
            this.btnExtractAllFolders.UseVisualStyleBackColor = true;
            this.btnExtractAllFolders.Click += new System.EventHandler(this.btnExtractFolders_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSearch.Location = new System.Drawing.Point(12, 308);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(371, 20);
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(389, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Search";
            // 
            // foldersImageList
            // 
            this.foldersImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.foldersImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.foldersImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 9);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tlvFolders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvFiles);
            this.splitContainer1.Size = new System.Drawing.Size(610, 261);
            this.splitContainer1.SplitterDistance = 107;
            this.splitContainer1.TabIndex = 10;
            // 
            // tlvFolders
            // 
            this.tlvFolders.AllColumns.Add(this.olvColumn1);
            this.tlvFolders.AllowDrop = true;
            this.tlvFolders.CellEditUseWholeCell = false;
            this.tlvFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1});
            this.tlvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlvFolders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.tlvFolders.HideSelection = false;
            this.tlvFolders.Location = new System.Drawing.Point(0, 0);
            this.tlvFolders.MultiSelect = false;
            this.tlvFolders.Name = "tlvFolders";
            this.tlvFolders.ShowGroups = false;
            this.tlvFolders.ShowItemToolTips = true;
            this.tlvFolders.Size = new System.Drawing.Size(107, 261);
            this.tlvFolders.TabIndex = 14;
            this.tlvFolders.UseCellFormatEvents = true;
            this.tlvFolders.UseCompatibleStateImageBehavior = false;
            this.tlvFolders.UseOverlays = false;
            this.tlvFolders.View = System.Windows.Forms.View.Details;
            this.tlvFolders.VirtualMode = true;
            this.tlvFolders.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.tlvFolders_ItemSelectionChanged);
            this.tlvFolders.DragDrop += new System.Windows.Forms.DragEventHandler(this.File_DragDrop);
            this.tlvFolders.DragOver += new System.Windows.Forms.DragEventHandler(this.File_DragOver);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "Name";
            this.olvColumn1.FillsFreeSpace = true;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 200;
            // 
            // cbRegex
            // 
            this.cbRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbRegex.AutoSize = true;
            this.cbRegex.Location = new System.Drawing.Point(436, 310);
            this.cbRegex.Name = "cbRegex";
            this.cbRegex.Size = new System.Drawing.Size(74, 17);
            this.cbRegex.TabIndex = 8;
            this.cbRegex.Text = "Use regex";
            this.cbRegex.UseVisualStyleBackColor = true;
            this.cbRegex.CheckedChanged += new System.EventHandler(this.cbRegex_CheckedChanged);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.toolsMenuItem,
            this.openFoldersMenuItem,
            this.helpMenuItem});
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.Index = 0;
            this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.openArchiveMenuItem,
            this.closeSelectedArchiveMenuItem,
            this.closeAllArchivesMenuItem,
            this.menuItem4,
            this.extractArchivesMenuItem,
            this.menuItem6,
            this.recentFilesMenuItem,
            this.menuItem10,
            this.exitMenuItem});
            this.fileMenuItem.Text = "File";
            this.fileMenuItem.Popup += new System.EventHandler(this.fileMenuItem_Popup);
            // 
            // openArchiveMenuItem
            // 
            this.openArchiveMenuItem.Index = 0;
            this.openArchiveMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.openArchiveMenuItem.Text = "Open Archive...";
            this.openArchiveMenuItem.Click += new System.EventHandler(this.openArchiveMenuItem_Click);
            // 
            // closeSelectedArchiveMenuItem
            // 
            this.closeSelectedArchiveMenuItem.Index = 1;
            this.closeSelectedArchiveMenuItem.Text = "Close Selected Archive";
            this.closeSelectedArchiveMenuItem.Click += new System.EventHandler(this.closeSelectedArchiveMenuItem_Click);
            // 
            // closeAllArchivesMenuItem
            // 
            this.closeAllArchivesMenuItem.Index = 2;
            this.closeAllArchivesMenuItem.Text = "Close All Archives";
            this.closeAllArchivesMenuItem.Click += new System.EventHandler(this.closeAllArchivesMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.Text = "-";
            // 
            // extractArchivesMenuItem
            // 
            this.extractArchivesMenuItem.Index = 4;
            this.extractArchivesMenuItem.Text = "Extract Archives...";
            this.extractArchivesMenuItem.Click += new System.EventHandler(this.extractArchivesMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            this.menuItem6.Text = "-";
            // 
            // recentFilesMenuItem
            // 
            this.recentFilesMenuItem.Index = 6;
            this.recentFilesMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.emptyListMenuItem,
            this.menuItem9});
            this.recentFilesMenuItem.Text = "Recent Files...";
            this.recentFilesMenuItem.Popup += new System.EventHandler(this.recentFilesMenuItem_Popup);
            // 
            // emptyListMenuItem
            // 
            this.emptyListMenuItem.Index = 0;
            this.emptyListMenuItem.Text = "Empty List";
            this.emptyListMenuItem.Click += new System.EventHandler(this.emptyListMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.Text = "-";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 7;
            this.menuItem10.Text = "-";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Index = 8;
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // editMenuItem
            // 
            this.editMenuItem.Index = 1;
            this.editMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyMenuItem,
            this.selectAllMenuItem});
            this.editMenuItem.Text = "Edit";
            this.editMenuItem.Popup += new System.EventHandler(this.editMenuItem_Popup);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Index = 0;
            this.copyMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyPathMenuItem,
            this.copyFolderPathMenuItem,
            this.copyFileNameMenuItem});
            this.copyMenuItem.Text = "Copy";
            // 
            // copyPathMenuItem
            // 
            this.copyPathMenuItem.Index = 0;
            this.copyPathMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.copyPathMenuItem.Text = "Path";
            this.copyPathMenuItem.Click += new System.EventHandler(this.copyPathMenuItem_Click);
            // 
            // copyFolderPathMenuItem
            // 
            this.copyFolderPathMenuItem.Index = 1;
            this.copyFolderPathMenuItem.Text = "Folder Path";
            this.copyFolderPathMenuItem.Click += new System.EventHandler(this.copyFolderPathMenuItem_Click);
            // 
            // copyFileNameMenuItem
            // 
            this.copyFileNameMenuItem.Index = 2;
            this.copyFileNameMenuItem.Text = "File Name";
            this.copyFileNameMenuItem.Click += new System.EventHandler(this.copyFileNameMenuItem_Click);
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Index = 1;
            this.selectAllMenuItem.Text = "Select All";
            this.selectAllMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 2;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.optionsMenuItem,
            this.menuItem1,
            this.compareArchivesMenuItem});
            this.toolsMenuItem.Text = "Tools";
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Index = 0;
            this.optionsMenuItem.Text = "Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "-";
            // 
            // compareArchivesMenuItem
            // 
            this.compareArchivesMenuItem.Index = 2;
            this.compareArchivesMenuItem.Text = "Compare Archives...";
            this.compareArchivesMenuItem.Click += new System.EventHandler(this.compareArchivesMenuItem_Click);
            // 
            // openFoldersMenuItem
            // 
            this.openFoldersMenuItem.Index = 3;
            this.openFoldersMenuItem.Text = "Open Folders";
            this.openFoldersMenuItem.Click += new System.EventHandler(this.openFoldersMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Index = 4;
            this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.checkForUpdateMenuItem,
            this.menuItem3,
            this.githubMenuItem,
            this.discordMenuItem,
            this.menuItem2,
            this.fallout4NexusPageMenuItem,
            this.skyrimSENexusPageMenuItem,
            this.menuItem13,
            this.aboutMenuItem});
            this.helpMenuItem.Text = "Help";
            this.helpMenuItem.Popup += new System.EventHandler(this.helpMenuItem_Popup);
            // 
            // checkForUpdateMenuItem
            // 
            this.checkForUpdateMenuItem.Index = 0;
            this.checkForUpdateMenuItem.Text = "Check for update";
            this.checkForUpdateMenuItem.Click += new System.EventHandler(this.checkForUpdateMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "-";
            // 
            // githubMenuItem
            // 
            this.githubMenuItem.Index = 2;
            this.githubMenuItem.Text = "GitHub";
            this.githubMenuItem.Click += new System.EventHandler(this.githubMenuItem_Click);
            // 
            // discordMenuItem
            // 
            this.discordMenuItem.Index = 3;
            this.discordMenuItem.Text = "Discord (Help/Discussion)";
            this.discordMenuItem.Click += new System.EventHandler(this.discordMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 4;
            this.menuItem2.Text = "-";
            // 
            // fallout4NexusPageMenuItem
            // 
            this.fallout4NexusPageMenuItem.Index = 5;
            this.fallout4NexusPageMenuItem.Text = "Fallout 4 Nexus Page";
            this.fallout4NexusPageMenuItem.Click += new System.EventHandler(this.fallout4NexusPageMenuItem_Click);
            // 
            // skyrimSENexusPageMenuItem
            // 
            this.skyrimSENexusPageMenuItem.Index = 6;
            this.skyrimSENexusPageMenuItem.Text = "Skyrim SE Nexus Page";
            this.skyrimSENexusPageMenuItem.Click += new System.EventHandler(this.skyrimSENexusPageMenuItem_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 7;
            this.menuItem13.Text = "-";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Index = 8;
            this.aboutMenuItem.Text = "About BSA Browser";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.extractMenuItem,
            this.extractFoldersMenuItem,
            this.previewMenuItem,
            this.menuItem11,
            this.quickExtractsMenuItem,
            this.menuItem5,
            this.copyMenuItem1,
            this.selectAllMenuItem1,
            this.menuItem7,
            this.compareMenuItem,
            this.compareCancelMenuItem});
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // extractMenuItem
            // 
            this.extractMenuItem.Index = 0;
            this.extractMenuItem.Text = "Extract";
            this.extractMenuItem.Click += new System.EventHandler(this.extractMenuItem_Click);
            // 
            // extractFoldersMenuItem
            // 
            this.extractFoldersMenuItem.Index = 1;
            this.extractFoldersMenuItem.Text = "Extract Folders";
            this.extractFoldersMenuItem.Click += new System.EventHandler(this.extractFoldersMenuItem_Click);
            // 
            // previewMenuItem
            // 
            this.previewMenuItem.Index = 2;
            this.previewMenuItem.Text = "Preview";
            this.previewMenuItem.Click += new System.EventHandler(this.previewMenuItem_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 3;
            this.menuItem11.Text = "-";
            // 
            // quickExtractsMenuItem
            // 
            this.quickExtractsMenuItem.Index = 4;
            this.quickExtractsMenuItem.Text = "Quick extract...";
            this.quickExtractsMenuItem.Click += new System.EventHandler(this.quickExtractsMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 5;
            this.menuItem5.Text = "-";
            // 
            // copyMenuItem1
            // 
            this.copyMenuItem1.Index = 6;
            this.copyMenuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyPathMenuItem1,
            this.copyFolderPathMenuItem1,
            this.copyFileNameMenuItem1});
            this.copyMenuItem1.Text = "Copy";
            // 
            // copyPathMenuItem1
            // 
            this.copyPathMenuItem1.Index = 0;
            this.copyPathMenuItem1.Text = "Path";
            this.copyPathMenuItem1.Click += new System.EventHandler(this.copyPathMenuItem1_Click);
            // 
            // copyFolderPathMenuItem1
            // 
            this.copyFolderPathMenuItem1.Index = 1;
            this.copyFolderPathMenuItem1.Text = "Folder Path";
            this.copyFolderPathMenuItem1.Click += new System.EventHandler(this.copyFolderPathMenuItem1_Click);
            // 
            // copyFileNameMenuItem1
            // 
            this.copyFileNameMenuItem1.Index = 2;
            this.copyFileNameMenuItem1.Text = "File Name";
            this.copyFileNameMenuItem1.Click += new System.EventHandler(this.copyFileNameMenuItem1_Click);
            // 
            // selectAllMenuItem1
            // 
            this.selectAllMenuItem1.Index = 7;
            this.selectAllMenuItem1.Text = "Select All";
            this.selectAllMenuItem1.Click += new System.EventHandler(this.selectAllMenuItem1_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 8;
            this.menuItem7.Text = "-";
            // 
            // compareMenuItem
            // 
            this.compareMenuItem.Index = 9;
            this.compareMenuItem.Text = "Compare...";
            this.compareMenuItem.Click += new System.EventHandler(this.compareMenuItem_Click);
            // 
            // compareCancelMenuItem
            // 
            this.compareCancelMenuItem.Index = 10;
            this.compareCancelMenuItem.Text = "Cancel";
            this.compareCancelMenuItem.Click += new System.EventHandler(this.compareCancelMenuItem_Click);
            // 
            // lFileCount
            // 
            this.lFileCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lFileCount.Location = new System.Drawing.Point(522, 315);
            this.lFileCount.Margin = new System.Windows.Forms.Padding(3);
            this.lFileCount.Name = "lFileCount";
            this.lFileCount.Size = new System.Drawing.Size(100, 13);
            this.lFileCount.TabIndex = 12;
            this.lFileCount.Text = "0 files";
            this.lFileCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // archiveContextMenu
            // 
            this.archiveContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.extractAllFilesMenuItem,
            this.extractAllFoldersMenuItem,
            this.loadedSeparatorMenuItem1,
            this.reloadMenuItem,
            this.openContainingFolderMenuItem,
            this.loadedSeparatorMenuItem2,
            this.closeMenuItem,
            this.alwaysHideSeparatorMenuItem1,
            this.removeLoadedMenuItem,
            this.removeUnloadedMenuItem,
            this.alwaysHideSeparatorMenuItem2,
            this.unloadedOpenContainingFolderMenuItem,
            this.unloadedSeparatorMenuItem1,
            this.unloadedLoadMenuItem,
            this.unloadedRemoveMenuItem});
            this.archiveContextMenu.Popup += new System.EventHandler(this.archiveContextMenu_Popup);
            // 
            // extractAllFilesMenuItem
            // 
            this.extractAllFilesMenuItem.Index = 0;
            this.extractAllFilesMenuItem.Text = "Extract All Files";
            this.extractAllFilesMenuItem.Click += new System.EventHandler(this.extractAllFilesMenuItem_Click);
            // 
            // extractAllFoldersMenuItem
            // 
            this.extractAllFoldersMenuItem.Index = 1;
            this.extractAllFoldersMenuItem.Text = "Extract All Folders";
            this.extractAllFoldersMenuItem.Click += new System.EventHandler(this.extractAllFoldersMenuItem_Click);
            // 
            // loadedSeparatorMenuItem1
            // 
            this.loadedSeparatorMenuItem1.Index = 2;
            this.loadedSeparatorMenuItem1.Text = "-";
            // 
            // reloadMenuItem
            // 
            this.reloadMenuItem.Index = 3;
            this.reloadMenuItem.Text = "Reload";
            this.reloadMenuItem.Click += new System.EventHandler(this.reloadMenuItem_Click);
            // 
            // openContainingFolderMenuItem
            // 
            this.openContainingFolderMenuItem.Index = 4;
            this.openContainingFolderMenuItem.Text = "Open Containing Folder";
            this.openContainingFolderMenuItem.Click += new System.EventHandler(this.openContainingFolderMenuItem_Click);
            // 
            // loadedSeparatorMenuItem2
            // 
            this.loadedSeparatorMenuItem2.Index = 5;
            this.loadedSeparatorMenuItem2.Text = "-";
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Index = 6;
            this.closeMenuItem.Text = "Close";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            // 
            // alwaysHideSeparatorMenuItem1
            // 
            this.alwaysHideSeparatorMenuItem1.Index = 7;
            this.alwaysHideSeparatorMenuItem1.Text = "-";
            // 
            // removeLoadedMenuItem
            // 
            this.removeLoadedMenuItem.Index = 8;
            this.removeLoadedMenuItem.Text = "Remove Loaded";
            this.removeLoadedMenuItem.Click += new System.EventHandler(this.removeLoadedMenuItem_Click);
            // 
            // removeUnloadedMenuItem
            // 
            this.removeUnloadedMenuItem.Index = 9;
            this.removeUnloadedMenuItem.Text = "Remove Unloaded";
            this.removeUnloadedMenuItem.Click += new System.EventHandler(this.removeUnloadedMenuItem_Click);
            // 
            // alwaysHideSeparatorMenuItem2
            // 
            this.alwaysHideSeparatorMenuItem2.Index = 10;
            this.alwaysHideSeparatorMenuItem2.Text = "-";
            // 
            // unloadedOpenContainingFolderMenuItem
            // 
            this.unloadedOpenContainingFolderMenuItem.Index = 11;
            this.unloadedOpenContainingFolderMenuItem.Text = "Open Containing Folder";
            this.unloadedOpenContainingFolderMenuItem.Click += new System.EventHandler(this.unloadedOpenContainingFolderMenuItem_Click);
            // 
            // unloadedSeparatorMenuItem1
            // 
            this.unloadedSeparatorMenuItem1.Index = 12;
            this.unloadedSeparatorMenuItem1.Text = "-";
            // 
            // unloadedLoadMenuItem
            // 
            this.unloadedLoadMenuItem.Index = 13;
            this.unloadedLoadMenuItem.Text = "Load";
            this.unloadedLoadMenuItem.Click += new System.EventHandler(this.unloadedLoadMenuItem_Click);
            // 
            // unloadedRemoveMenuItem
            // 
            this.unloadedRemoveMenuItem.Index = 14;
            this.unloadedRemoveMenuItem.Text = "Remove";
            this.unloadedRemoveMenuItem.Click += new System.EventHandler(this.unloadedRemoveMenuItem_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 281);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "All files:";
            // 
            // BSABrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 340);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lFileCount);
            this.Controls.Add(this.cbRegex);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnExtractAllFolders);
            this.Controls.Add(this.btnExtractAll);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(639, 180);
            this.Name = "BSABrowser";
            this.Text = "BSA Browser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BSABrowser_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BSABrowser_FormClosed);
            this.Load += new System.EventHandler(this.BSABrowser_Load);
            this.Shown += new System.EventHandler(this.BSABrowser_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tlvFolders)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnExtractAll;
        private System.Windows.Forms.OpenFileDialog OpenArchiveDialog;
        private System.Windows.Forms.ColumnHeader columnFilePath;
        private System.Windows.Forms.Button btnExtractAllFolders;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox cbRegex;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem fileMenuItem;
        private System.Windows.Forms.MenuItem openArchiveMenuItem;
        private System.Windows.Forms.MenuItem closeSelectedArchiveMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem recentFilesMenuItem;
        private System.Windows.Forms.MenuItem emptyListMenuItem;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.MenuItem quickExtractsMenuItem;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem copyPathMenuItem1;
        private System.Windows.Forms.MenuItem copyFolderPathMenuItem1;
        private System.Windows.Forms.MenuItem copyFileNameMenuItem1;
        private System.Windows.Forms.MenuItem editMenuItem;
        private System.Windows.Forms.MenuItem copyPathMenuItem;
        private System.Windows.Forms.MenuItem copyFolderPathMenuItem;
        private System.Windows.Forms.MenuItem copyFileNameMenuItem;
        private System.Windows.Forms.MenuItem helpMenuItem;
        private System.Windows.Forms.MenuItem aboutMenuItem;
        private System.Windows.Forms.Label lFileCount;
        private System.Windows.Forms.ColumnHeader columnFileSize;
        private System.Windows.Forms.MenuItem openFoldersMenuItem;
        private System.Windows.Forms.MenuItem checkForUpdateMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem closeAllArchivesMenuItem;
        private System.Windows.Forms.MenuItem extractMenuItem;
        private System.Windows.Forms.MenuItem extractFoldersMenuItem;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem copyMenuItem1;
        private System.Windows.Forms.MenuItem selectAllMenuItem1;
        private System.Windows.Forms.MenuItem copyMenuItem;
        private System.Windows.Forms.MenuItem selectAllMenuItem;
        private System.Windows.Forms.MenuItem previewMenuItem;
        private System.Windows.Forms.ContextMenu archiveContextMenu;
        private System.Windows.Forms.MenuItem extractAllFilesMenuItem;
        private System.Windows.Forms.MenuItem extractAllFoldersMenuItem;
        private System.Windows.Forms.MenuItem loadedSeparatorMenuItem1;
        private System.Windows.Forms.MenuItem closeMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem extractArchivesMenuItem;
        private System.Windows.Forms.MenuItem toolsMenuItem;
        private System.Windows.Forms.MenuItem compareArchivesMenuItem;
        private System.Windows.Forms.MenuItem fallout4NexusPageMenuItem;
        private System.Windows.Forms.MenuItem skyrimSENexusPageMenuItem;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem githubMenuItem;
        private System.Windows.Forms.MenuItem discordMenuItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.ImageList foldersImageList;
        private System.Windows.Forms.ImageList filesImageList;
        private System.Windows.Forms.ColumnHeader columnArchive;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem compareMenuItem;
        private System.Windows.Forms.MenuItem compareCancelMenuItem;
        private System.Windows.Forms.MenuItem openContainingFolderMenuItem;
        private System.Windows.Forms.MenuItem loadedSeparatorMenuItem2;
        private System.Windows.Forms.MenuItem reloadMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuItem optionsMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private BrightIdeasSoftware.TreeListView tlvFolders;
        private System.Windows.Forms.MenuItem alwaysHideSeparatorMenuItem1;
        private System.Windows.Forms.MenuItem removeLoadedMenuItem;
        private System.Windows.Forms.MenuItem removeUnloadedMenuItem;
        private System.Windows.Forms.MenuItem alwaysHideSeparatorMenuItem2;
        private System.Windows.Forms.MenuItem unloadedOpenContainingFolderMenuItem;
        private System.Windows.Forms.MenuItem unloadedSeparatorMenuItem1;
        private System.Windows.Forms.MenuItem unloadedLoadMenuItem;
        private System.Windows.Forms.MenuItem unloadedRemoveMenuItem;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
    }
}

