namespace BSA_Browser.Dialogs
{
    partial class ExtractArchivesDialog
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
            this.lvArchives = new System.Windows.Forms.ListView();
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvArchives
            // 
            this.lvArchives.CheckBoxes = true;
            this.lvArchives.Location = new System.Drawing.Point(12, 12);
            this.lvArchives.Name = "lvArchives";
            this.lvArchives.Size = new System.Drawing.Size(310, 208);
            this.lvArchives.TabIndex = 0;
            this.lvArchives.UseCompatibleStateImageBehavior = false;
            this.lvArchives.View = System.Windows.Forms.View.List;
            this.lvArchives.SelectedIndexChanged += new System.EventHandler(this.lvArchives_SelectedIndexChanged);
            this.lvArchives.Enter += new System.EventHandler(this.lvArchives_Enter);
            // 
            // btnExtract
            // 
            this.btnExtract.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExtract.Location = new System.Drawing.Point(166, 226);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 23);
            this.btnExtract.TabIndex = 1;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(247, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ExtractArchivesDialog
            // 
            this.AcceptButton = this.btnExtract;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(334, 261);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.lvArchives);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtractArchivesDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Extract Archives";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvArchives;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnCancel;
    }
}