using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BSA_Browser
{
    public partial class ProgressForm : Form
    {
        private bool blockClose = true;

        /// <summary>
        /// Occurs when the "Cancel" button is clicked, or the form is closed.
        /// </summary>
        public event EventHandler Canceled;

        public ProgressForm(string title, bool showRatio)
        {
            InitializeComponent();
            Application.UseWaitCursor = true;
            Text = title;
            if (!showRatio)
            {
                pbRatio.Visible = false;
                lRatio.Visible = false;
                pbProgress.Height += 21;
                lProgress.Top += 10;
            }
            this.Closing += new CancelEventHandler(ProgressForm_FormClosing);
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        private void ProgressForm_FormClosing(object sender, CancelEventArgs e)
        {
            if (blockClose)
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

        /// <summary>
        /// Enables the "Cancel" button.
        /// </summary>
        public void EnableCancel()
        {
            btnCancel.Enabled = true;
        }

        public int GetProgressPercentage()
        {
            return ((int)(100 * (float)pbProgress.Value / (float)pbProgress.Maximum));
        }

        /// <summary>
        /// Sets the ProgressBar maximum value. 
        /// </summary>
        /// <param name="high">The maximum value.</param>
        public void SetProgressRange(int high)
        {
            pbProgress.Maximum = high;
        }

        /// <summary>
        /// Unblocks the form, allowing it to be closed.
        /// </summary>
        public void Unblock()
        {
            blockClose = false;
        }

        /// <summary>
        /// Updates the ProgressBar value and percentage text.
        /// </summary>
        /// <param name="value">The new value.</param>
        public void UpdateProgress(int value)
        {
            pbProgress.Value = Math.Min(value, pbProgress.Maximum);
            lProgress.Text = ((int)(100 * (float)value / (float)pbProgress.Maximum)).ToString() + "%";
            if (!Focused) Focus();
        }

        private void OnCanceled(EventArgs e)
        {
            Canceled?.Invoke(this, e);
        }
    }

    public class fommCancelException : fommException { public fommCancelException(string msg) : base(msg) { } }
}
