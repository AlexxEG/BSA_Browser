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
            this.lvArchiveA = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbArchiveA = new System.Windows.Forms.ComboBox();
            this.cbArchiveB = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lComparison = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvArchiveA
            // 
            this.lvArchiveA.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.tableLayoutPanel1.SetColumnSpan(this.lvArchiveA, 2);
            this.lvArchiveA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvArchiveA.FullRowSelect = true;
            this.lvArchiveA.GridLines = true;
            this.lvArchiveA.Location = new System.Drawing.Point(3, 33);
            this.lvArchiveA.Name = "lvArchiveA";
            this.lvArchiveA.Size = new System.Drawing.Size(770, 285);
            this.lvArchiveA.TabIndex = 0;
            this.lvArchiveA.UseCompatibleStateImageBehavior = false;
            this.lvArchiveA.View = System.Windows.Forms.View.Details;
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
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lvArchiveA, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbArchiveA, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbArchiveB, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 321);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // cbArchiveA
            // 
            this.cbArchiveA.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbArchiveA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchiveA.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbArchiveA.FormattingEnabled = true;
            this.cbArchiveA.Location = new System.Drawing.Point(3, 3);
            this.cbArchiveA.Name = "cbArchiveA";
            this.cbArchiveA.Size = new System.Drawing.Size(382, 24);
            this.cbArchiveA.TabIndex = 2;
            this.cbArchiveA.SelectedIndexChanged += new System.EventHandler(this.cbArchiveA_SelectedIndexChanged);
            // 
            // cbArchiveB
            // 
            this.cbArchiveB.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbArchiveB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchiveB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbArchiveB.FormattingEnabled = true;
            this.cbArchiveB.Location = new System.Drawing.Point(391, 3);
            this.cbArchiveB.Name = "cbArchiveB";
            this.cbArchiveB.Size = new System.Drawing.Size(382, 24);
            this.cbArchiveB.TabIndex = 3;
            this.cbArchiveB.SelectedIndexChanged += new System.EventHandler(this.cbArchiveB_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lComparison);
            this.groupBox1.Location = new System.Drawing.Point(15, 339);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(770, 99);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compared";
            // 
            // lComparison
            // 
            this.lComparison.AutoSize = true;
            this.lComparison.Location = new System.Drawing.Point(6, 24);
            this.lComparison.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.lComparison.Name = "lComparison";
            this.lComparison.Size = new System.Drawing.Size(123, 65);
            this.lComparison.TabIndex = 5;
            this.lComparison.Text = "{0} added\r\n{1} removed\r\n{2} changed\r\n\r\n{3} files left, {4} files right";
            // 
            // CompareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompareForm";
            this.Text = "Compare";
            this.Load += new System.EventHandler(this.CompareForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvArchiveA;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox cbArchiveA;
        private System.Windows.Forms.ComboBox cbArchiveB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lComparison;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}