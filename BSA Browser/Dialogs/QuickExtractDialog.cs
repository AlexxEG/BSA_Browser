using BSA_Browser.Classes;
using System;
using System.Windows.Forms;

namespace BSA_Browser.Dialogs
{
    public partial class QuickExtractDialog : Form
    {
        public string Path
        {
            get
            {
                return txtPath.Text;
            }
        }
        public string PathName
        {
            get
            {
                return txtName.Text;
            }
        }
        public bool UseFolderPath
        {
            get
            {
                return chbUseFolderPath.Checked;
            }
        }

        public QuickExtractDialog()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(IWin32Window owner, QuickExtractPath path)
        {
            txtName.Text = path.Name;
            txtPath.Text = path.Path;
            chbUseFolderPath.Checked = path.UseFolderPath;

            return this.ShowDialog(owner);
        }

        private void QuickExtractDialog_Load(object sender, EventArgs e)
        {
            // Set it here otherwise DPI scaling will not work correctly, for some reason
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFolderDialog())
            {
                if (!string.IsNullOrEmpty(txtPath.Text))
                    ofd.InitialFolder = txtPath.Text;

                if (ofd.ShowDialog(this) == DialogResult.OK)
                    txtPath.Text = ofd.Folder;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show(this, "Please fill all fields.");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
