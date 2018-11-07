namespace BSA_Browser
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lProgress = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lCurrentFile = new System.Windows.Forms.Label();
            this.lHeader = new System.Windows.Forms.Label();
            this.lFooter = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.ForeColor = System.Drawing.Color.Lime;
            this.pbProgress.Location = new System.Drawing.Point(15, 92);
            this.pbProgress.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.pbProgress.Maximum = 10000;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(377, 28);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 0;
            // 
            // lProgress
            // 
            this.lProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lProgress.AutoSize = true;
            this.lProgress.Location = new System.Drawing.Point(371, 73);
            this.lProgress.Name = "lProgress";
            this.lProgress.Size = new System.Drawing.Size(21, 13);
            this.lProgress.TabIndex = 2;
            this.lProgress.Text = "0%";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(317, 126);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lCurrentFile
            // 
            this.lCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lCurrentFile.AutoSize = true;
            this.lCurrentFile.Location = new System.Drawing.Point(12, 73);
            this.lCurrentFile.Margin = new System.Windows.Forms.Padding(3);
            this.lCurrentFile.Name = "lCurrentFile";
            this.lCurrentFile.Size = new System.Drawing.Size(52, 13);
            this.lCurrentFile.TabIndex = 5;
            this.lCurrentFile.Text = "Waiting...";
            // 
            // lHeader
            // 
            this.lHeader.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lHeader.Location = new System.Drawing.Point(0, 0);
            this.lHeader.Margin = new System.Windows.Forms.Padding(3);
            this.lHeader.Name = "lHeader";
            this.lHeader.Padding = new System.Windows.Forms.Padding(10);
            this.lHeader.Size = new System.Drawing.Size(404, 50);
            this.lHeader.TabIndex = 6;
            this.lHeader.Text = "Header";
            this.lHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lFooter
            // 
            this.lFooter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lFooter.AutoSize = true;
            this.lFooter.Location = new System.Drawing.Point(12, 131);
            this.lFooter.Margin = new System.Windows.Forms.Padding(3);
            this.lFooter.Name = "lFooter";
            this.lFooter.Size = new System.Drawing.Size(30, 13);
            this.lFooter.TabIndex = 7;
            this.lFooter.Text = "(0/0)";
            // 
            // ProgressForm
            // 
            this.ClientSize = new System.Drawing.Size(404, 161);
            this.Controls.Add(this.lFooter);
            this.Controls.Add(this.lHeader);
            this.Controls.Add(this.lCurrentFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lProgress);
            this.Controls.Add(this.pbProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.ProgressForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lCurrentFile;
        private System.Windows.Forms.Label lHeader;
        private System.Windows.Forms.Label lFooter;
    }
}