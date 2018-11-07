using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BSA_Browser
{
    public partial class ProgressForm : Form
    {
        /// <summary>
        /// Occurs when the "Cancel" button is clicked, or the form is closed.
        /// </summary>
        public event EventHandler Canceled;

        /// <summary>
        /// Gets or sets whether to block form closing.
        /// </summary>
        public bool BlockClose { get; set; } = true;

        /// <summary>
        /// Gets or sets whether Cancel button is enabled.
        /// </summary>
        public bool Cancelable
        {
            get { return btnCancel.Enabled; }
            set { btnCancel.Enabled = value; }
        }

        /// <summary>
        /// Gets or sets the file currently being extracted.
        /// </summary>
        public string CurrentFile
        {
            get { return lCurrentFile.Text.Substring(0, lCurrentFile.Text.Length - 3); }
            set { lCurrentFile.Text = $"{value}..."; }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        public string Header
        {
            get { return lHeader.Text; }
            set { lHeader.Text = value; }
        }

        /// <summary>
        /// Gets or sets the footer text.
        /// </summary>
        public string Footer
        {
            get { return lFooter.Text; }
            set { lFooter.Text = value; }
        }

        /// <summary>
        /// Gets or sets the maximum progress.
        /// </summary>
        public int Maximum
        {
            get { return pbProgress.Maximum; }
            set { pbProgress.Maximum = value; }
        }

        /// <summary>
        /// Gets or sets the progress (NOT percentage).
        /// </summary>
        public int Progress
        {
            get { return pbProgress.Value; }
            set
            {
                pbProgress.Value = Math.Min(value, pbProgress.Maximum);
                lProgress.Text = 100 * value / pbProgress.Maximum + "%";
                if (!Focused) Focus();
            }
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        public int ProgressPercentage
        {
            get
            {
                return 100 * pbProgress.Value / pbProgress.Maximum;
            }
        }

        public ProgressForm(string title)
        {
            InitializeComponent();
            Application.UseWaitCursor = true;
            this.Text = title;
            this.Closing += new CancelEventHandler(ProgressForm_FormClosing);
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        private void ProgressForm_FormClosing(object sender, CancelEventArgs e)
        {
            if (this.BlockClose)
            {
                e.Cancel = true;
            }
            else
            {
                Application.UseWaitCursor = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCanceled(EventArgs.Empty);
        }

        private void OnCanceled(EventArgs e)
        {
            Canceled?.Invoke(this, e);
        }
    }
}
