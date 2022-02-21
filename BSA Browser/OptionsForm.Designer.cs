namespace BSA_Browser
{
    partial class OptionsForm
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Texture/Image Viewer", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Text Viewer", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(".dds");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(".bmp");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(".png");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(".jpg");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(".txt");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(".bat");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(".xml");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(".lst");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(".psc");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(".json");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cbAssociateFiles = new System.Windows.Forms.CheckBox();
            this.cbShellIntegration = new System.Windows.Forms.CheckBox();
            this.chbRememberArchives = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chbIconsFileList = new System.Windows.Forms.CheckBox();
            this.chbIconsFolderTree = new System.Windows.Forms.CheckBox();
            this.btnResetToDefaultGeneral = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chbSortBSADirectories = new System.Windows.Forms.CheckBox();
            this.chbRetrieveRealSize = new System.Windows.Forms.CheckBox();
            this.cbEncodings = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chbCheckForUpdates = new System.Windows.Forms.CheckBox();
            this.nudMaxRecentFiles = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnResetToDefaultExtraction = new System.Windows.Forms.Button();
            this.chbReplaceGNFExt = new System.Windows.Forms.CheckBox();
            this.chbMatchLastWriteTime = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lvQuickExtract = new L0ki.Controls.ReordableItemListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnResetToDefaultPreview = new System.Windows.Forms.Button();
            this.nudMaxResolutionH = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudMaxResolutionW = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvPreviewing = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxRecentFiles)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxResolutionH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxResolutionW)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(476, 366);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(557, 366);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(6, 7);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(600, 86);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Quick Extract";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(588, 60);
            this.label1.TabIndex = 0;
            this.label1.Text = "Add custom quick extract paths. Quick extract can extract files to a selected fol" +
    "der, optionally maintaining the folder path, by using the right click menu.\n\nDra" +
    "g items to reorder quick extract paths.";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(168, 293);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(87, 293);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(6, 293);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add...";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(620, 348);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbAssociateFiles);
            this.tabPage1.Controls.Add(this.cbShellIntegration);
            this.tabPage1.Controls.Add(this.chbRememberArchives);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.btnResetToDefaultGeneral);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.chbCheckForUpdates);
            this.tabPage1.Controls.Add(this.nudMaxRecentFiles);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage1.Size = new System.Drawing.Size(612, 322);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cbAssociateFiles
            // 
            this.cbAssociateFiles.AutoSize = true;
            this.cbAssociateFiles.Location = new System.Drawing.Point(363, 13);
            this.cbAssociateFiles.Name = "cbAssociateFiles";
            this.cbAssociateFiles.Size = new System.Drawing.Size(236, 17);
            this.cbAssociateFiles.TabIndex = 8;
            this.cbAssociateFiles.Text = "Associate BSA Browser with .bsa && .ba2 files";
            this.cbAssociateFiles.UseVisualStyleBackColor = true;
            this.cbAssociateFiles.CheckedChanged += new System.EventHandler(this.cbAssociateFiles_CheckedChanged);
            // 
            // cbShellIntegration
            // 
            this.cbShellIntegration.AutoSize = true;
            this.cbShellIntegration.Location = new System.Drawing.Point(381, 36);
            this.cbShellIntegration.Name = "cbShellIntegration";
            this.cbShellIntegration.Size = new System.Drawing.Size(168, 17);
            this.cbShellIntegration.TabIndex = 7;
            this.cbShellIntegration.Text = "Shell context menu integration";
            this.cbShellIntegration.UseVisualStyleBackColor = true;
            // 
            // chbRememberArchives
            // 
            this.chbRememberArchives.AutoSize = true;
            this.chbRememberArchives.Location = new System.Drawing.Point(13, 13);
            this.chbRememberArchives.Name = "chbRememberArchives";
            this.chbRememberArchives.Size = new System.Drawing.Size(121, 17);
            this.chbRememberArchives.TabIndex = 6;
            this.chbRememberArchives.Text = "Remember Archives";
            this.chbRememberArchives.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.chbIconsFileList);
            this.groupBox3.Controls.Add(this.chbIconsFolderTree);
            this.groupBox3.Location = new System.Drawing.Point(13, 221);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(177, 85);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Icons";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Show icons in:";
            // 
            // chbIconsFileList
            // 
            this.chbIconsFileList.AutoSize = true;
            this.chbIconsFileList.Location = new System.Drawing.Point(9, 38);
            this.chbIconsFileList.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.chbIconsFileList.Name = "chbIconsFileList";
            this.chbIconsFileList.Size = new System.Drawing.Size(61, 17);
            this.chbIconsFileList.TabIndex = 0;
            this.chbIconsFileList.Text = "File List";
            this.chbIconsFileList.UseVisualStyleBackColor = true;
            // 
            // chbIconsFolderTree
            // 
            this.chbIconsFolderTree.AutoSize = true;
            this.chbIconsFolderTree.Location = new System.Drawing.Point(9, 55);
            this.chbIconsFolderTree.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.chbIconsFolderTree.Name = "chbIconsFolderTree";
            this.chbIconsFolderTree.Size = new System.Drawing.Size(80, 17);
            this.chbIconsFolderTree.TabIndex = 1;
            this.chbIconsFolderTree.Text = "Folder Tree";
            this.chbIconsFolderTree.UseVisualStyleBackColor = true;
            // 
            // btnResetToDefaultGeneral
            // 
            this.btnResetToDefaultGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetToDefaultGeneral.Location = new System.Drawing.Point(481, 286);
            this.btnResetToDefaultGeneral.Name = "btnResetToDefaultGeneral";
            this.btnResetToDefaultGeneral.Size = new System.Drawing.Size(118, 23);
            this.btnResetToDefaultGeneral.TabIndex = 5;
            this.btnResetToDefaultGeneral.Text = "Reset to Default";
            this.btnResetToDefaultGeneral.UseVisualStyleBackColor = true;
            this.btnResetToDefaultGeneral.Click += new System.EventHandler(this.btnResetToDefaultGeneral_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chbSortBSADirectories);
            this.groupBox2.Controls.Add(this.chbRetrieveRealSize);
            this.groupBox2.Controls.Add(this.cbEncodings);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(13, 97);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(401, 112);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Archive Options";
            // 
            // chbSortBSADirectories
            // 
            this.chbSortBSADirectories.AutoSize = true;
            this.chbSortBSADirectories.Location = new System.Drawing.Point(6, 19);
            this.chbSortBSADirectories.Name = "chbSortBSADirectories";
            this.chbSortBSADirectories.Size = new System.Drawing.Size(302, 17);
            this.chbSortBSADirectories.TabIndex = 0;
            this.chbSortBSADirectories.Text = "Sort directories in archives (Reload archives to take effect)";
            this.chbSortBSADirectories.UseVisualStyleBackColor = true;
            // 
            // chbRetrieveRealSize
            // 
            this.chbRetrieveRealSize.Location = new System.Drawing.Point(6, 42);
            this.chbRetrieveRealSize.Name = "chbRetrieveRealSize";
            this.chbRetrieveRealSize.Size = new System.Drawing.Size(389, 34);
            this.chbRetrieveRealSize.TabIndex = 1;
            this.chbRetrieveRealSize.Text = "Always show uncompressed file size, even if the parsing will be slower (Reload ar" +
    "chives to take effect)";
            this.chbRetrieveRealSize.UseVisualStyleBackColor = true;
            // 
            // cbEncodings
            // 
            this.cbEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEncodings.FormattingEnabled = true;
            this.cbEncodings.Location = new System.Drawing.Point(67, 85);
            this.cbEncodings.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.cbEncodings.Name = "cbEncodings";
            this.cbEncodings.Size = new System.Drawing.Size(210, 21);
            this.cbEncodings.TabIndex = 3;
            this.cbEncodings.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cbEncodings_Format);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Encoding:";
            // 
            // chbCheckForUpdates
            // 
            this.chbCheckForUpdates.AutoSize = true;
            this.chbCheckForUpdates.Location = new System.Drawing.Point(13, 60);
            this.chbCheckForUpdates.Name = "chbCheckForUpdates";
            this.chbCheckForUpdates.Size = new System.Drawing.Size(177, 17);
            this.chbCheckForUpdates.TabIndex = 2;
            this.chbCheckForUpdates.Text = "Check for updates automatically";
            this.chbCheckForUpdates.UseVisualStyleBackColor = true;
            // 
            // nudMaxRecentFiles
            // 
            this.nudMaxRecentFiles.Location = new System.Drawing.Point(94, 34);
            this.nudMaxRecentFiles.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.nudMaxRecentFiles.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMaxRecentFiles.Name = "nudMaxRecentFiles";
            this.nudMaxRecentFiles.Size = new System.Drawing.Size(43, 20);
            this.nudMaxRecentFiles.TabIndex = 1;
            this.nudMaxRecentFiles.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 36);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Max recent files:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnResetToDefaultExtraction);
            this.tabPage4.Controls.Add(this.chbReplaceGNFExt);
            this.tabPage4.Controls.Add(this.chbMatchLastWriteTime);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage4.Size = new System.Drawing.Size(612, 322);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Extraction";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnResetToDefaultExtraction
            // 
            this.btnResetToDefaultExtraction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetToDefaultExtraction.Location = new System.Drawing.Point(481, 286);
            this.btnResetToDefaultExtraction.Name = "btnResetToDefaultExtraction";
            this.btnResetToDefaultExtraction.Size = new System.Drawing.Size(118, 23);
            this.btnResetToDefaultExtraction.TabIndex = 6;
            this.btnResetToDefaultExtraction.Text = "Reset to Default";
            this.btnResetToDefaultExtraction.UseVisualStyleBackColor = true;
            this.btnResetToDefaultExtraction.Click += new System.EventHandler(this.btnResetToDefaultExtraction_Click);
            // 
            // chbReplaceGNFExt
            // 
            this.chbReplaceGNFExt.AutoSize = true;
            this.chbReplaceGNFExt.Location = new System.Drawing.Point(13, 36);
            this.chbReplaceGNFExt.Name = "chbReplaceGNFExt";
            this.chbReplaceGNFExt.Size = new System.Drawing.Size(220, 17);
            this.chbReplaceGNFExt.TabIndex = 1;
            this.chbReplaceGNFExt.Text = "Extract PS4 textures with .GNF extension";
            this.chbReplaceGNFExt.UseVisualStyleBackColor = true;
            // 
            // chbMatchLastWriteTime
            // 
            this.chbMatchLastWriteTime.AutoSize = true;
            this.chbMatchLastWriteTime.Location = new System.Drawing.Point(13, 13);
            this.chbMatchLastWriteTime.Name = "chbMatchLastWriteTime";
            this.chbMatchLastWriteTime.Size = new System.Drawing.Size(287, 17);
            this.chbMatchLastWriteTime.TabIndex = 0;
            this.chbMatchLastWriteTime.Text = "Match last changed date on extracted files with archive";
            this.chbMatchLastWriteTime.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lvQuickExtract);
            this.tabPage2.Controls.Add(this.btnRemove);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.btnEdit);
            this.tabPage2.Controls.Add(this.btnAdd);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(612, 322);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Quick Extract";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvQuickExtract
            // 
            this.lvQuickExtract.AllowDrop = true;
            this.lvQuickExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvQuickExtract.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvQuickExtract.FullRowSelect = true;
            this.lvQuickExtract.HideSelection = false;
            this.lvQuickExtract.Location = new System.Drawing.Point(6, 99);
            this.lvQuickExtract.Name = "lvQuickExtract";
            this.lvQuickExtract.ShowGroups = false;
            this.lvQuickExtract.Size = new System.Drawing.Size(600, 188);
            this.lvQuickExtract.TabIndex = 1;
            this.lvQuickExtract.UseCompatibleStateImageBehavior = false;
            this.lvQuickExtract.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Path";
            this.columnHeader2.Width = 282;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Maintain folder path";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 120;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnResetToDefaultPreview);
            this.tabPage3.Controls.Add(this.nudMaxResolutionH);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.nudMaxResolutionW);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(612, 322);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Preview";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnResetToDefaultPreview
            // 
            this.btnResetToDefaultPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetToDefaultPreview.Location = new System.Drawing.Point(190, 287);
            this.btnResetToDefaultPreview.Name = "btnResetToDefaultPreview";
            this.btnResetToDefaultPreview.Size = new System.Drawing.Size(118, 23);
            this.btnResetToDefaultPreview.TabIndex = 7;
            this.btnResetToDefaultPreview.Text = "Reset to Default";
            this.btnResetToDefaultPreview.UseVisualStyleBackColor = true;
            this.btnResetToDefaultPreview.Click += new System.EventHandler(this.btnResetToDefaultPreview_Click);
            // 
            // nudMaxResolutionH
            // 
            this.nudMaxResolutionH.Location = new System.Drawing.Point(25, 63);
            this.nudMaxResolutionH.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxResolutionH.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudMaxResolutionH.Name = "nudMaxResolutionH";
            this.nudMaxResolutionH.Size = new System.Drawing.Size(48, 20);
            this.nudMaxResolutionH.TabIndex = 6;
            this.nudMaxResolutionH.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(79, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Height";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Width";
            // 
            // nudMaxResolutionW
            // 
            this.nudMaxResolutionW.Location = new System.Drawing.Point(25, 36);
            this.nudMaxResolutionW.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudMaxResolutionW.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudMaxResolutionW.Name = "nudMaxResolutionW";
            this.nudMaxResolutionW.Size = new System.Drawing.Size(48, 20);
            this.nudMaxResolutionW.TabIndex = 2;
            this.nudMaxResolutionW.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Max Resolution:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lvPreviewing);
            this.groupBox1.Location = new System.Drawing.Point(320, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 298);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Built-in Previewing";
            // 
            // lvPreviewing
            // 
            this.lvPreviewing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPreviewing.CheckBoxes = true;
            this.lvPreviewing.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lvPreviewing.FullRowSelect = true;
            this.lvPreviewing.GridLines = true;
            listViewGroup1.Header = "Texture/Image Viewer";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "Text Viewer";
            listViewGroup2.Name = "listViewGroup2";
            this.lvPreviewing.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.lvPreviewing.HideSelection = false;
            listViewItem1.Group = listViewGroup1;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.Group = listViewGroup1;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.Group = listViewGroup1;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.Group = listViewGroup1;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.Group = listViewGroup2;
            listViewItem5.StateImageIndex = 0;
            listViewItem6.Group = listViewGroup2;
            listViewItem6.StateImageIndex = 0;
            listViewItem7.Group = listViewGroup2;
            listViewItem7.StateImageIndex = 0;
            listViewItem8.Group = listViewGroup2;
            listViewItem8.StateImageIndex = 0;
            listViewItem9.Group = listViewGroup2;
            listViewItem9.StateImageIndex = 0;
            listViewItem10.Group = listViewGroup2;
            listViewItem10.StateImageIndex = 0;
            this.lvPreviewing.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10});
            this.lvPreviewing.Location = new System.Drawing.Point(6, 19);
            this.lvPreviewing.Name = "lvPreviewing";
            this.lvPreviewing.Size = new System.Drawing.Size(268, 273);
            this.lvPreviewing.TabIndex = 0;
            this.lvPreviewing.UseCompatibleStateImageBehavior = false;
            this.lvPreviewing.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "File Type";
            this.columnHeader4.Width = 240;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(644, 401);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsForm_FormClosing);
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.groupBox5.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxRecentFiles)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxResolutionH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxResolutionW)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox5;
        private L0ki.Controls.ReordableItemListView lvQuickExtract;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.CheckBox chbSortBSADirectories;
        private System.Windows.Forms.CheckBox chbRetrieveRealSize;
        private System.Windows.Forms.NumericUpDown nudMaxRecentFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbEncodings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView lvPreviewing;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chbCheckForUpdates;
        private System.Windows.Forms.Button btnResetToDefaultGeneral;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chbIconsFileList;
        private System.Windows.Forms.CheckBox chbIconsFolderTree;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox chbMatchLastWriteTime;
        private System.Windows.Forms.CheckBox chbReplaceGNFExt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnResetToDefaultExtraction;
        private System.Windows.Forms.CheckBox chbRememberArchives;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudMaxResolutionW;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudMaxResolutionH;
        private System.Windows.Forms.Button btnResetToDefaultPreview;
        private System.Windows.Forms.CheckBox cbShellIntegration;
        private System.Windows.Forms.CheckBox cbAssociateFiles;
    }
}