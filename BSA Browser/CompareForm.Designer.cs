namespace BSA_Browser
{
    partial class CompareForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareForm));
            this.lvArchive = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lChunksB = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lFileCountB = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lVersionB = new System.Windows.Forms.Label();
            this.lTypeB = new System.Windows.Forms.Label();
            this.lChunksBB = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lNoNameTableB = new System.Windows.Forms.Label();
            this.cbArchiveB = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lChunksA = new System.Windows.Forms.Label();
            this.lFileCountA = new System.Windows.Forms.Label();
            this.lVersionA = new System.Windows.Forms.Label();
            this.lChunksAA = new System.Windows.Forms.Label();
            this.lNoNameTableA = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lTypeA = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbArchiveA = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chbFilterUnique = new System.Windows.Forms.CheckBox();
            this.chbFilterChanged = new System.Windows.Forms.CheckBox();
            this.chbFilterIdentical = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lStats = new System.Windows.Forms.Label();
            this.lAdded = new System.Windows.Forms.Label();
            this.lRemoved = new System.Windows.Forms.Label();
            this.lChanged = new System.Windows.Forms.Label();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.extractLeftMenuItem = new System.Windows.Forms.MenuItem();
            this.extractRightMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.extractSelectedLeftMenuItem = new System.Windows.Forms.MenuItem();
            this.extractSelectedRightMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.previewLeftMenuItem = new System.Windows.Forms.MenuItem();
            this.previewRightMenuItem = new System.Windows.Forms.MenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvArchive
            // 
            this.lvArchive.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.tableLayoutPanel1.SetColumnSpan(this.lvArchive, 2);
            this.lvArchive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvArchive.FullRowSelect = true;
            this.lvArchive.GridLines = true;
            this.lvArchive.HideSelection = false;
            this.lvArchive.LabelWrap = false;
            this.lvArchive.Location = new System.Drawing.Point(3, 109);
            this.lvArchive.Name = "lvArchive";
            this.lvArchive.ShowItemToolTips = true;
            this.lvArchive.Size = new System.Drawing.Size(770, 292);
            this.lvArchive.TabIndex = 0;
            this.lvArchive.UseCompatibleStateImageBehavior = false;
            this.lvArchive.View = System.Windows.Forms.View.Details;
            this.lvArchive.VirtualMode = true;
            this.lvArchive.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lvArchive_ColumnWidthChanging);
            this.lvArchive.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvArchive_RetrieveVirtualItem);
            this.lvArchive.Resize += new System.EventHandler(this.lvArchive_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Files A";
            this.columnHeader1.Width = 370;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Files B";
            this.columnHeader2.Width = 370;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lvArchive, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 426);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lChunksB);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.lFileCountB);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.lVersionB);
            this.groupBox3.Controls.Add(this.lTypeB);
            this.groupBox3.Controls.Add(this.lChunksBB);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.lNoNameTableB);
            this.groupBox3.Controls.Add(this.cbArchiveB);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(391, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.groupBox3.Size = new System.Drawing.Size(382, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Archive B";
            // 
            // lChunksB
            // 
            this.lChunksB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lChunksB.AutoSize = true;
            this.lChunksB.Location = new System.Drawing.Point(66, 81);
            this.lChunksB.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lChunksB.Name = "lChunksB";
            this.lChunksB.Size = new System.Drawing.Size(10, 13);
            this.lChunksB.TabIndex = 19;
            this.lChunksB.Text = "-";
            this.lChunksB.Visible = false;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(18, 49);
            this.label14.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 13);
            this.label14.TabIndex = 13;
            this.label14.Text = "Version:";
            // 
            // lFileCountB
            // 
            this.lFileCountB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lFileCountB.AutoSize = true;
            this.lFileCountB.Location = new System.Drawing.Point(66, 65);
            this.lFileCountB.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lFileCountB.Name = "lFileCountB";
            this.lFileCountB.Size = new System.Drawing.Size(10, 13);
            this.lFileCountB.TabIndex = 18;
            this.lFileCountB.Text = "-";
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(109, 49);
            this.label16.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(34, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Type:";
            // 
            // lVersionB
            // 
            this.lVersionB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lVersionB.AutoSize = true;
            this.lVersionB.Location = new System.Drawing.Point(66, 49);
            this.lVersionB.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lVersionB.Name = "lVersionB";
            this.lVersionB.Size = new System.Drawing.Size(10, 13);
            this.lVersionB.TabIndex = 17;
            this.lVersionB.Text = "-";
            // 
            // lTypeB
            // 
            this.lTypeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lTypeB.AutoSize = true;
            this.lTypeB.Location = new System.Drawing.Point(143, 49);
            this.lTypeB.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lTypeB.Name = "lTypeB";
            this.lTypeB.Size = new System.Drawing.Size(10, 13);
            this.lTypeB.TabIndex = 12;
            this.lTypeB.Text = "-";
            // 
            // lChunksBB
            // 
            this.lChunksBB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lChunksBB.AutoSize = true;
            this.lChunksBB.Location = new System.Drawing.Point(17, 81);
            this.lChunksBB.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lChunksBB.Name = "lChunksBB";
            this.lChunksBB.Size = new System.Drawing.Size(46, 13);
            this.lChunksBB.TabIndex = 16;
            this.lChunksBB.Text = "Chunks:";
            this.lChunksBB.Visible = false;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 65);
            this.label13.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 13);
            this.label13.TabIndex = 14;
            this.label13.Text = "File Count:";
            // 
            // lNoNameTableB
            // 
            this.lNoNameTableB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lNoNameTableB.AutoSize = true;
            this.lNoNameTableB.ForeColor = System.Drawing.Color.Purple;
            this.lNoNameTableB.Location = new System.Drawing.Point(300, 81);
            this.lNoNameTableB.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lNoNameTableB.Name = "lNoNameTableB";
            this.lNoNameTableB.Size = new System.Drawing.Size(76, 13);
            this.lNoNameTableB.TabIndex = 15;
            this.lNoNameTableB.Text = "No name table";
            this.lNoNameTableB.Visible = false;
            // 
            // cbArchiveB
            // 
            this.cbArchiveB.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbArchiveB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchiveB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbArchiveB.FormattingEnabled = true;
            this.cbArchiveB.Location = new System.Drawing.Point(3, 19);
            this.cbArchiveB.Name = "cbArchiveB";
            this.cbArchiveB.Size = new System.Drawing.Size(376, 24);
            this.cbArchiveB.TabIndex = 20;
            this.cbArchiveB.SelectedIndexChanged += new System.EventHandler(this.cbArchives_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lChunksA);
            this.groupBox2.Controls.Add(this.lFileCountA);
            this.groupBox2.Controls.Add(this.lVersionA);
            this.groupBox2.Controls.Add(this.lChunksAA);
            this.groupBox2.Controls.Add(this.lNoNameTableA);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lTypeA);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cbArchiveA);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.groupBox2.Size = new System.Drawing.Size(382, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Archive A";
            // 
            // lChunksA
            // 
            this.lChunksA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lChunksA.AutoSize = true;
            this.lChunksA.Location = new System.Drawing.Point(66, 81);
            this.lChunksA.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lChunksA.Name = "lChunksA";
            this.lChunksA.Size = new System.Drawing.Size(10, 13);
            this.lChunksA.TabIndex = 10;
            this.lChunksA.Text = "-";
            this.lChunksA.Visible = false;
            // 
            // lFileCountA
            // 
            this.lFileCountA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lFileCountA.AutoSize = true;
            this.lFileCountA.Location = new System.Drawing.Point(66, 65);
            this.lFileCountA.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lFileCountA.Name = "lFileCountA";
            this.lFileCountA.Size = new System.Drawing.Size(10, 13);
            this.lFileCountA.TabIndex = 9;
            this.lFileCountA.Text = "-";
            // 
            // lVersionA
            // 
            this.lVersionA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lVersionA.AutoSize = true;
            this.lVersionA.Location = new System.Drawing.Point(66, 49);
            this.lVersionA.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lVersionA.Name = "lVersionA";
            this.lVersionA.Size = new System.Drawing.Size(10, 13);
            this.lVersionA.TabIndex = 8;
            this.lVersionA.Text = "-";
            // 
            // lChunksAA
            // 
            this.lChunksAA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lChunksAA.AutoSize = true;
            this.lChunksAA.Location = new System.Drawing.Point(17, 81);
            this.lChunksAA.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lChunksAA.Name = "lChunksAA";
            this.lChunksAA.Size = new System.Drawing.Size(46, 13);
            this.lChunksAA.TabIndex = 7;
            this.lChunksAA.Text = "Chunks:";
            this.lChunksAA.Visible = false;
            // 
            // lNoNameTableA
            // 
            this.lNoNameTableA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lNoNameTableA.AutoSize = true;
            this.lNoNameTableA.ForeColor = System.Drawing.Color.Purple;
            this.lNoNameTableA.Location = new System.Drawing.Point(300, 81);
            this.lNoNameTableA.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lNoNameTableA.Name = "lNoNameTableA";
            this.lNoNameTableA.Size = new System.Drawing.Size(76, 13);
            this.lNoNameTableA.TabIndex = 6;
            this.lNoNameTableA.Text = "No name table";
            this.lNoNameTableA.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 65);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "File Count:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 49);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Version:";
            // 
            // lTypeA
            // 
            this.lTypeA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lTypeA.AutoSize = true;
            this.lTypeA.Location = new System.Drawing.Point(143, 49);
            this.lTypeA.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lTypeA.Name = "lTypeA";
            this.lTypeA.Size = new System.Drawing.Size(10, 13);
            this.lTypeA.TabIndex = 3;
            this.lTypeA.Text = "-";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 49);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Type:";
            // 
            // cbArchiveA
            // 
            this.cbArchiveA.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbArchiveA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchiveA.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbArchiveA.FormattingEnabled = true;
            this.cbArchiveA.Location = new System.Drawing.Point(3, 19);
            this.cbArchiveA.Name = "cbArchiveA";
            this.cbArchiveA.Size = new System.Drawing.Size(376, 24);
            this.cbArchiveA.TabIndex = 11;
            this.cbArchiveA.SelectedIndexChanged += new System.EventHandler(this.cbArchives_SelectedIndexChanged);
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.chbFilterUnique);
            this.flowLayoutPanel1.Controls.Add(this.chbFilterChanged);
            this.flowLayoutPanel1.Controls.Add(this.chbFilterIdentical);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.lStats);
            this.flowLayoutPanel1.Controls.Add(this.lAdded);
            this.flowLayoutPanel1.Controls.Add(this.lRemoved);
            this.flowLayoutPanel1.Controls.Add(this.lChanged);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 404);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(776, 22);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // chbFilterUnique
            // 
            this.chbFilterUnique.AutoSize = true;
            this.chbFilterUnique.Checked = true;
            this.chbFilterUnique.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbFilterUnique.Location = new System.Drawing.Point(3, 3);
            this.chbFilterUnique.Name = "chbFilterUnique";
            this.chbFilterUnique.Size = new System.Drawing.Size(60, 17);
            this.chbFilterUnique.TabIndex = 0;
            this.chbFilterUnique.Text = "Unique";
            this.chbFilterUnique.UseVisualStyleBackColor = true;
            this.chbFilterUnique.CheckedChanged += new System.EventHandler(this.lFilters_CheckedChanged);
            // 
            // chbFilterChanged
            // 
            this.chbFilterChanged.AutoSize = true;
            this.chbFilterChanged.Checked = true;
            this.chbFilterChanged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbFilterChanged.Location = new System.Drawing.Point(69, 3);
            this.chbFilterChanged.Name = "chbFilterChanged";
            this.chbFilterChanged.Size = new System.Drawing.Size(69, 17);
            this.chbFilterChanged.TabIndex = 1;
            this.chbFilterChanged.Text = "Changed";
            this.chbFilterChanged.UseVisualStyleBackColor = true;
            this.chbFilterChanged.CheckedChanged += new System.EventHandler(this.lFilters_CheckedChanged);
            // 
            // chbFilterIdentical
            // 
            this.chbFilterIdentical.AutoSize = true;
            this.chbFilterIdentical.Checked = true;
            this.chbFilterIdentical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbFilterIdentical.Location = new System.Drawing.Point(144, 3);
            this.chbFilterIdentical.Name = "chbFilterIdentical";
            this.chbFilterIdentical.Size = new System.Drawing.Size(66, 17);
            this.chbFilterIdentical.TabIndex = 2;
            this.chbFilterIdentical.Text = "Identical";
            this.chbFilterIdentical.UseVisualStyleBackColor = true;
            this.chbFilterIdentical.CheckedChanged += new System.EventHandler(this.lFilters_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(216, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(1);
            this.label4.Size = new System.Drawing.Size(11, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "|";
            // 
            // lStats
            // 
            this.lStats.AutoSize = true;
            this.lStats.Location = new System.Drawing.Point(233, 3);
            this.lStats.Margin = new System.Windows.Forms.Padding(3);
            this.lStats.Name = "lStats";
            this.lStats.Padding = new System.Windows.Forms.Padding(1);
            this.lStats.Size = new System.Drawing.Size(54, 15);
            this.lStats.TabIndex = 4;
            this.lStats.Text = "Statistics:";
            // 
            // lAdded
            // 
            this.lAdded.AutoSize = true;
            this.lAdded.ForeColor = System.Drawing.Color.Green;
            this.lAdded.Location = new System.Drawing.Point(293, 3);
            this.lAdded.Margin = new System.Windows.Forms.Padding(3);
            this.lAdded.Name = "lAdded";
            this.lAdded.Padding = new System.Windows.Forms.Padding(1);
            this.lAdded.Size = new System.Drawing.Size(56, 15);
            this.lAdded.TabIndex = 5;
            this.lAdded.Text = "{0} added";
            // 
            // lRemoved
            // 
            this.lRemoved.AutoSize = true;
            this.lRemoved.ForeColor = System.Drawing.Color.Red;
            this.lRemoved.Location = new System.Drawing.Point(355, 3);
            this.lRemoved.Margin = new System.Windows.Forms.Padding(3);
            this.lRemoved.Name = "lRemoved";
            this.lRemoved.Padding = new System.Windows.Forms.Padding(1);
            this.lRemoved.Size = new System.Drawing.Size(67, 15);
            this.lRemoved.TabIndex = 6;
            this.lRemoved.Text = "{0} removed";
            // 
            // lChanged
            // 
            this.lChanged.AutoSize = true;
            this.lChanged.ForeColor = System.Drawing.Color.Blue;
            this.lChanged.Location = new System.Drawing.Point(428, 3);
            this.lChanged.Margin = new System.Windows.Forms.Padding(3);
            this.lChanged.Name = "lChanged";
            this.lChanged.Padding = new System.Windows.Forms.Padding(1);
            this.lChanged.Size = new System.Drawing.Size(68, 15);
            this.lChanged.TabIndex = 7;
            this.lChanged.Text = "{0} changed";
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.extractLeftMenuItem,
            this.extractRightMenuItem,
            this.menuItem3,
            this.extractSelectedLeftMenuItem,
            this.extractSelectedRightMenuItem,
            this.menuItem1,
            this.previewLeftMenuItem,
            this.previewRightMenuItem});
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // extractLeftMenuItem
            // 
            this.extractLeftMenuItem.Index = 0;
            this.extractLeftMenuItem.Text = "Extract Left";
            this.extractLeftMenuItem.Click += new System.EventHandler(this.extractLeftMenuItem_Click);
            // 
            // extractRightMenuItem
            // 
            this.extractRightMenuItem.Index = 1;
            this.extractRightMenuItem.Text = "Extract Right";
            this.extractRightMenuItem.Click += new System.EventHandler(this.extractRightMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.Text = "-";
            // 
            // extractSelectedLeftMenuItem
            // 
            this.extractSelectedLeftMenuItem.Index = 3;
            this.extractSelectedLeftMenuItem.Text = "Extract Selected Left";
            this.extractSelectedLeftMenuItem.Click += new System.EventHandler(this.extractSelectedLeftMenuItem_Click);
            // 
            // extractSelectedRightMenuItem
            // 
            this.extractSelectedRightMenuItem.Index = 4;
            this.extractSelectedRightMenuItem.Text = "Extract Selected Right";
            this.extractSelectedRightMenuItem.Click += new System.EventHandler(this.extractSelectedRightMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 5;
            this.menuItem1.Text = "-";
            // 
            // previewLeftMenuItem
            // 
            this.previewLeftMenuItem.Index = 6;
            this.previewLeftMenuItem.Text = "Preview Left";
            this.previewLeftMenuItem.Click += new System.EventHandler(this.previewLeftMenuItem_Click);
            // 
            // previewRightMenuItem
            // 
            this.previewRightMenuItem.Index = 7;
            this.previewRightMenuItem.Text = "Preview Right";
            this.previewRightMenuItem.Click += new System.EventHandler(this.previewRightMenuItem_Click);
            // 
            // CompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompareForm";
            this.Text = "Compare";
            this.Load += new System.EventHandler(this.CompareForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvArchive;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lTypeA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lChunksAA;
        private System.Windows.Forms.Label lNoNameTableA;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lChunksB;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lFileCountB;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lVersionB;
        private System.Windows.Forms.Label lTypeB;
        private System.Windows.Forms.Label lChunksBB;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lNoNameTableB;
        private System.Windows.Forms.Label lChunksA;
        private System.Windows.Forms.Label lFileCountA;
        private System.Windows.Forms.Label lVersionA;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chbFilterUnique;
        private System.Windows.Forms.CheckBox chbFilterChanged;
        private System.Windows.Forms.CheckBox chbFilterIdentical;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem extractLeftMenuItem;
        private System.Windows.Forms.MenuItem extractRightMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem previewLeftMenuItem;
        private System.Windows.Forms.MenuItem previewRightMenuItem;
        private System.Windows.Forms.MenuItem extractSelectedLeftMenuItem;
        private System.Windows.Forms.MenuItem extractSelectedRightMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.ComboBox cbArchiveB;
        private System.Windows.Forms.ComboBox cbArchiveA;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lStats;
        private System.Windows.Forms.Label lAdded;
        private System.Windows.Forms.Label lRemoved;
        private System.Windows.Forms.Label lChanged;
    }
}