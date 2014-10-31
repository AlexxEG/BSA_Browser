using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BSA_Browser
{
    internal class ProgressForm : Form
    {
        #region FormDesignerGunk
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
            this.pbRatio = new System.Windows.Forms.ProgressBar();
            this.lProgress = new System.Windows.Forms.Label();
            this.lRatio = new System.Windows.Forms.Label();
            this.bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.ForeColor = System.Drawing.Color.Lime;
            this.pbProgress.Location = new System.Drawing.Point(12, 12);
            this.pbProgress.Maximum = 10000;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(255, 15);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 0;
            // 
            // pbRatio
            // 
            this.pbRatio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbRatio.ForeColor = System.Drawing.Color.Lime;
            this.pbRatio.Location = new System.Drawing.Point(12, 33);
            this.pbRatio.Maximum = 10000;
            this.pbRatio.Name = "pbRatio";
            this.pbRatio.Size = new System.Drawing.Size(255, 15);
            this.pbRatio.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbRatio.TabIndex = 1;
            // 
            // lProgress
            // 
            this.lProgress.AutoSize = true;
            this.lProgress.Location = new System.Drawing.Point(273, 10);
            this.lProgress.Name = "lProgress";
            this.lProgress.Size = new System.Drawing.Size(0, 13);
            this.lProgress.TabIndex = 2;
            // 
            // lRatio
            // 
            this.lRatio.AutoSize = true;
            this.lRatio.Location = new System.Drawing.Point(273, 31);
            this.lRatio.Name = "lRatio";
            this.lRatio.Size = new System.Drawing.Size(0, 13);
            this.lRatio.TabIndex = 3;
            // 
            // bCancel
            // 
            this.bCancel.Enabled = false;
            this.bCancel.Location = new System.Drawing.Point(107, 54);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 4;
            this.bCancel.Text = "Cancel";
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // ProgressForm
            // 
            this.ClientSize = new System.Drawing.Size(309, 88);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.lRatio);
            this.Controls.Add(this.lProgress);
            this.Controls.Add(this.pbRatio);
            this.Controls.Add(this.pbProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.ProgressBar pbRatio;
        private System.Windows.Forms.Label lProgress;
        private System.Windows.Forms.Label lRatio;
        #endregion
        private Button bCancel;
        private bool BlockClose = true;

        internal ProgressForm(string title, bool ShowRatio)
        {
            InitializeComponent();
            Application.UseWaitCursor = true;
            Text = title;
            if (!ShowRatio)
            {
                pbRatio.Visible = false;
                lRatio.Visible = false;
                pbProgress.Height += 21;
                lProgress.Top += 10;
            }
            this.Closing += new CancelEventHandler(ProgressForm_FormClosing);
        }

        internal void SetProgressRange(int high)
        {
            pbProgress.Maximum = high;
        }

        internal void EnableCancel()
        {
            bCancel.Enabled = true;
        }

        internal void Unblock() { BlockClose = false; }

        internal void UpdateProgress(int value)
        {
            pbProgress.Value = value;
            lProgress.Text = ((int)(100 * (float)value / (float)pbProgress.Maximum)).ToString() + "%";
            if (!Focused) Focus();
        }

        private void ProgressForm_FormClosing(object sender, CancelEventArgs e)
        {
            if (BlockClose)
            {
                e.Cancel = true;
            }
            else
            {
                Application.UseWaitCursor = false;
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            throw new fommCancelException("");
        }
    }

    internal class fommCancelException : fommException { internal fommCancelException(string msg) : base(msg) { } }
}
