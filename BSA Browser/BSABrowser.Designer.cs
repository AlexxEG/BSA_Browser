namespace BSA_Browser
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
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnExtractAll = new System.Windows.Forms.Button();
            this.OpenArchiveDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnExtract = new System.Windows.Forms.Button();
            this.cmbSortOrder = new System.Windows.Forms.ComboBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbRegex = new System.Windows.Forms.CheckBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.fileMenuItem = new System.Windows.Forms.MenuItem();
            this.openArchiveMnuItem = new System.Windows.Forms.MenuItem();
            this.closeSelectedArchiveMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.optionsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.recentFilesMenuItem = new System.Windows.Forms.MenuItem();
            this.emptyListMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.exitMenuItem = new System.Windows.Forms.MenuItem();
            this.editMenuItem = new System.Windows.Forms.MenuItem();
            this.copyPathMenuItem = new System.Windows.Forms.MenuItem();
            this.copyFolderPathMenuItem = new System.Windows.Forms.MenuItem();
            this.copyFileNameMenuItem = new System.Windows.Forms.MenuItem();
            this.openFoldersMenuItem = new System.Windows.Forms.MenuItem();
            this.helpMenuItem = new System.Windows.Forms.MenuItem();
            this.checkForUpdateMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.aboutMenuItem = new System.Windows.Forms.MenuItem();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.quickExtractsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.copyPathMenuItem1 = new System.Windows.Forms.MenuItem();
            this.copyFolderPathMenuItem1 = new System.Windows.Forms.MenuItem();
            this.copyFileNameMenuItem1 = new System.Windows.Forms.MenuItem();
            this.cbDesc = new System.Windows.Forms.CheckBox();
            this.lFileCount = new System.Windows.Forms.Label();
            this.closeAllArchivesMenuItem = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvFiles
            // 
            this.lvFiles.AutoArrange = false;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFiles.LabelWrap = false;
            this.lvFiles.Location = new System.Drawing.Point(0, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.ShowItemToolTips = true;
            this.lvFiles.Size = new System.Drawing.Size(499, 261);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.VirtualMode = true;
            this.lvFiles.ItemActivate += new System.EventHandler(this.btnExtract_Click);
            this.lvFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvFiles_ItemDrag);
            this.lvFiles.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvFiles_RetrieveVirtualItem);
            this.lvFiles.SelectedIndexChanged += new System.EventHandler(this.lvFiles_SelectedIndexChanged);
            this.lvFiles.Enter += new System.EventHandler(this.lvFiles_Enter);
            this.lvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvFiles_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File path";
            this.columnHeader1.Width = 227;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "File Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 80;
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
            this.btnExtractAll.Location = new System.Drawing.Point(273, 276);
            this.btnExtractAll.Name = "btnExtractAll";
            this.btnExtractAll.Size = new System.Drawing.Size(75, 23);
            this.btnExtractAll.TabIndex = 3;
            this.btnExtractAll.Text = "Extract all";
            this.btnExtractAll.UseVisualStyleBackColor = true;
            this.btnExtractAll.Click += new System.EventHandler(this.btnExtractAll_Click);
            // 
            // OpenArchiveDialog
            // 
            this.OpenArchiveDialog.Filter = "All supported files|*.bsa;*.ba2;*.dat|Fallout or Oblivion BSA archives|*.bsa|Fall" +
    "out 4 BA2 archives|*.ba2|Fallout 2 dat archive|*.dat";
            this.OpenArchiveDialog.Multiselect = true;
            this.OpenArchiveDialog.RestoreDirectory = true;
            this.OpenArchiveDialog.Title = "Select archive to open";
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExtract.Enabled = false;
            this.btnExtract.Location = new System.Drawing.Point(192, 276);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 23);
            this.btnExtract.TabIndex = 2;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // cmbSortOrder
            // 
            this.cmbSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortOrder.FormattingEnabled = true;
            this.cmbSortOrder.Items.AddRange(new object[] {
            "Folder name",
            "File name",
            "File size",
            "File extension"});
            this.cmbSortOrder.Location = new System.Drawing.Point(501, 278);
            this.cmbSortOrder.Name = "cmbSortOrder";
            this.cmbSortOrder.Size = new System.Drawing.Size(121, 21);
            this.cmbSortOrder.TabIndex = 5;
            this.cmbSortOrder.SelectedIndexChanged += new System.EventHandler(this.cmbSortOrder_SelectedIndexChanged);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreview.Enabled = false;
            this.btnPreview.Location = new System.Drawing.Point(111, 276);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 1;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSearch.Location = new System.Drawing.Point(12, 308);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(336, 20);
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(354, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Search";
            // 
            // tvFolders
            // 
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolders.HideSelection = false;
            this.tvFolders.Location = new System.Drawing.Point(0, 0);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.Size = new System.Drawing.Size(107, 261);
            this.tvFolders.TabIndex = 0;
            this.tvFolders.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvFolders_BeforeExpand);
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
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
            this.splitContainer1.Panel1.Controls.Add(this.tvFolders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvFiles);
            this.splitContainer1.Size = new System.Drawing.Size(610, 261);
            this.splitContainer1.SplitterDistance = 107;
            this.splitContainer1.TabIndex = 10;
            // 
            // cbRegex
            // 
            this.cbRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbRegex.AutoSize = true;
            this.cbRegex.Location = new System.Drawing.Point(401, 310);
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
            this.openFoldersMenuItem,
            this.helpMenuItem});
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.Index = 0;
            this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.openArchiveMnuItem,
            this.closeSelectedArchiveMenuItem,
            this.closeAllArchivesMenuItem,
            this.menuItem1,
            this.optionsMenuItem,
            this.menuItem6,
            this.recentFilesMenuItem,
            this.menuItem10,
            this.exitMenuItem});
            this.fileMenuItem.Text = "File";
            // 
            // openArchiveMnuItem
            // 
            this.openArchiveMnuItem.Index = 0;
            this.openArchiveMnuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.openArchiveMnuItem.Text = "Open Archive...";
            this.openArchiveMnuItem.Click += new System.EventHandler(this.openArchiveMenuItem_Click);
            // 
            // closeSelectedArchiveMenuItem
            // 
            this.closeSelectedArchiveMenuItem.Index = 1;
            this.closeSelectedArchiveMenuItem.Text = "Close Selected Archive";
            this.closeSelectedArchiveMenuItem.Click += new System.EventHandler(this.closeSelectedArchiveMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 3;
            this.menuItem1.Text = "-";
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Index = 4;
            this.optionsMenuItem.Text = "Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
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
            this.copyPathMenuItem,
            this.copyFolderPathMenuItem,
            this.copyFileNameMenuItem});
            this.editMenuItem.Text = "Edit";
            this.editMenuItem.Popup += new System.EventHandler(this.editMenuItem_Popup);
            // 
            // copyPathMenuItem
            // 
            this.copyPathMenuItem.Index = 0;
            this.copyPathMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.copyPathMenuItem.Text = "Copy Path";
            this.copyPathMenuItem.Click += new System.EventHandler(this.copyPathMenuItem_Click);
            // 
            // copyFolderPathMenuItem
            // 
            this.copyFolderPathMenuItem.Index = 1;
            this.copyFolderPathMenuItem.Text = "Copy Folder Path";
            this.copyFolderPathMenuItem.Click += new System.EventHandler(this.copyFolderPathMenuItem_Click);
            // 
            // copyFileNameMenuItem
            // 
            this.copyFileNameMenuItem.Index = 2;
            this.copyFileNameMenuItem.Text = "Copy File Name";
            this.copyFileNameMenuItem.Click += new System.EventHandler(this.copyFileNameMenuItem_Click);
            // 
            // openFoldersMenuItem
            // 
            this.openFoldersMenuItem.Index = 2;
            this.openFoldersMenuItem.Text = "Open Folders";
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.Index = 3;
            this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.checkForUpdateMenuItem,
            this.menuItem3,
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
            // aboutMenuItem
            // 
            this.aboutMenuItem.Index = 2;
            this.aboutMenuItem.Text = "About BSA Browser";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.quickExtractsMenuItem,
            this.menuItem5,
            this.copyPathMenuItem1,
            this.copyFolderPathMenuItem1,
            this.copyFileNameMenuItem1});
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // quickExtractsMenuItem
            // 
            this.quickExtractsMenuItem.Index = 0;
            this.quickExtractsMenuItem.Text = "Quick extract...";
            this.quickExtractsMenuItem.Click += new System.EventHandler(this.quickExtractsMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.Text = "-";
            // 
            // copyPathMenuItem1
            // 
            this.copyPathMenuItem1.Index = 2;
            this.copyPathMenuItem1.Text = "Copy Path";
            this.copyPathMenuItem1.Click += new System.EventHandler(this.copyPathMenuItem1_Click);
            // 
            // copyFolderPathMenuItem1
            // 
            this.copyFolderPathMenuItem1.Index = 3;
            this.copyFolderPathMenuItem1.Text = "Copy Folder Path";
            this.copyFolderPathMenuItem1.Click += new System.EventHandler(this.copyFolderPathMenuItem1_Click);
            // 
            // copyFileNameMenuItem1
            // 
            this.copyFileNameMenuItem1.Index = 4;
            this.copyFileNameMenuItem1.Text = "Copy File Name";
            this.copyFileNameMenuItem1.Click += new System.EventHandler(this.copyFileNameMenuItem1_Click);
            // 
            // cbDesc
            // 
            this.cbDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDesc.AutoSize = true;
            this.cbDesc.Checked = true;
            this.cbDesc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDesc.Location = new System.Drawing.Point(444, 280);
            this.cbDesc.Name = "cbDesc";
            this.cbDesc.Size = new System.Drawing.Size(51, 17);
            this.cbDesc.TabIndex = 11;
            this.cbDesc.Text = "Desc";
            this.cbDesc.UseVisualStyleBackColor = true;
            this.cbDesc.CheckedChanged += new System.EventHandler(this.cbDesc_CheckedChanged);
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
            // closeAllArchivesMenuItem
            // 
            this.closeAllArchivesMenuItem.Index = 2;
            this.closeAllArchivesMenuItem.Text = "Close All Archives";
            this.closeAllArchivesMenuItem.Click += new System.EventHandler(this.closeAllArchivesMenuItem_Click);
            // 
            // BSABrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 340);
            this.Controls.Add(this.lFileCount);
            this.Controls.Add(this.cbDesc);
            this.Controls.Add(this.cbRegex);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.btnExtractAll);
            this.Controls.Add(this.cmbSortOrder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(639, 180);
            this.Name = "BSABrowser";
            this.Text = "BSA Browser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BSABrowser_FormClosing);
            this.Load += new System.EventHandler(this.BSABrowser_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnExtractAll;
        private System.Windows.Forms.OpenFileDialog OpenArchiveDialog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.ComboBox cmbSortOrder;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox cbRegex;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem fileMenuItem;
        private System.Windows.Forms.MenuItem openArchiveMnuItem;
        private System.Windows.Forms.MenuItem closeSelectedArchiveMenuItem;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem recentFilesMenuItem;
        private System.Windows.Forms.MenuItem emptyListMenuItem;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem exitMenuItem;
        private System.Windows.Forms.MenuItem optionsMenuItem;
        private System.Windows.Forms.CheckBox cbDesc;
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
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem openFoldersMenuItem;
        private System.Windows.Forms.MenuItem checkForUpdateMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem closeAllArchivesMenuItem;
    }
}

