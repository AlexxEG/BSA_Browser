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
            this.btnCancel = new System.Windows.Forms.Button();
            this.lDescription = new System.Windows.Forms.Label();
            this.lHeader = new System.Windows.Forms.Label();
            this.lFooter = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.ForeColor = System.Drawing.Color.Lime;
            this.pbProgress.Location = new System.Drawing.Point(12, 92);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(380, 23);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(317, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Stop";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lDescription
            // 
            this.lDescription.AutoSize = true;
            this.lDescription.Location = new System.Drawing.Point(12, 53);
            this.lDescription.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(60, 13);
            this.lDescription.TabIndex = 5;
            this.lDescription.Text = "Description";
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
            this.lHeader.Size = new System.Drawing.Size(404, 40);
            this.lHeader.TabIndex = 6;
            this.lHeader.Text = "Extracting...";
            this.lHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lFooter
            // 
            this.lFooter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lFooter.AutoSize = true;
            this.lFooter.Location = new System.Drawing.Point(12, 14);
            this.lFooter.Margin = new System.Windows.Forms.Padding(3);
            this.lFooter.Name = "lFooter";
            this.lFooter.Size = new System.Drawing.Size(37, 13);
            this.lFooter.TabIndex = 7;
            this.lFooter.Text = "Footer";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.lFooter);
            this.panel1.Location = new System.Drawing.Point(0, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 42);
            this.panel1.TabIndex = 8;
            // 
            // ProgressForm
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(404, 166);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lHeader);
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.pbProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unpacking archive";
            this.Load += new System.EventHandler(this.ProgressForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lDescription;
        private System.Windows.Forms.Label lHeader;
        private System.Windows.Forms.Label lFooter;
        private System.Windows.Forms.Panel panel1;
    }
}