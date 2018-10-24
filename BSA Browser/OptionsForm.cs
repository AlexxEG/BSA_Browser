using System;
using System.Drawing;
using System.Windows.Forms;
using BSA_Browser.Classes;
using BSA_Browser.Controls;
using BSA_Browser.Dialogs;
using BSA_Browser.Properties;

namespace BSA_Browser
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            chbSortBSADirectories.Checked = Settings.Default.SortArchiveDirectories;
            chbRetrieveRealSize.Checked = Settings.Default.RetrieveRealSize;
            chbUseATIFourCC.Checked = Settings.Default.UseATIFourCC;

            foreach (var path in Settings.Default.QuickExtractPaths)
            {
                var item = new ListViewItem(path.Name);
                item.SubItems.Add(path.Path);
                item.SubItems.Add(path.UseFolderPath ? "Yes" : "No");
                item.Tag = path;

                lvQuickExtract.Items.Add(item);
            }

            lvQuickExtract.EnableVisualStyles();
            lvQuickExtract.EnableVisualStylesSelection();
            lvQuickExtract.HideFocusRectangle();
        }

        public OptionsForm(int tabPage)
            : this()
        {
            tabControl1.SelectedIndex = tabPage;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            if (!Settings.Default.WindowStates.Contains(this.Name))
            {
                Settings.Default.WindowStates.Add(this.Name);
            }

            // Restore only columns.
            Settings.Default.WindowStates[this.Name].RestoreForm(this);

            // Center form to Owner
            if (this.Owner != null)
            {
                this.Location = new Point(
                    Owner.Location.X + Owner.Width / 2 - Width / 2,
                    Owner.Location.Y + Owner.Height / 2 - Height / 2);
            }
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowStates[this.Name].SaveForm(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void lvQuickExtract_Enter(object sender, EventArgs e)
        {
            lvQuickExtract.HideFocusRectangle();
        }

        private void lvQuickExtract_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvQuickExtract.HideFocusRectangle();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var cpd = new QuickExtractDialog())
            {
                if (cpd.ShowDialog(this) == DialogResult.Cancel)
                    return;

                var path = new QuickExtractPath(cpd.PathName, cpd.Path, cpd.UseFolderPath);
                var newItem = new ListViewItem(cpd.PathName);

                newItem.SubItems.Add(cpd.Path);
                newItem.SubItems.Add(cpd.UseFolderPath ? "Yes" : "No");
                newItem.Tag = path;

                lvQuickExtract.Items.Insert(lvQuickExtract.Items.Count, newItem);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvQuickExtract.SelectedItems.Count == 0)
                return;

            using (var cpd = new QuickExtractDialog())
            {
                var item = lvQuickExtract.SelectedItems[0];
                var path = (QuickExtractPath)item.Tag;

                if (cpd.ShowDialog(this, path) == DialogResult.Cancel)
                    return;

                path.Name = cpd.PathName;
                path.Path = cpd.Path;
                path.UseFolderPath = cpd.UseFolderPath;

                item.Text = cpd.PathName;
                item.SubItems[1].Text = cpd.Path;
                item.SubItems[2].Text = cpd.UseFolderPath ? "Yes" : "No";
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvQuickExtract.SelectedItems.Count == 0)
                return;

            int index = lvQuickExtract.SelectedItems[0].Index;

            lvQuickExtract.Items.RemoveAt(index);
        }

        public void SaveChanges()
        {
            Settings.Default.SortArchiveDirectories = chbSortBSADirectories.Checked;
            Settings.Default.RetrieveRealSize = chbRetrieveRealSize.Checked;
            Settings.Default.UseATIFourCC = chbUseATIFourCC.Checked;

            Settings.Default.QuickExtractPaths.Clear();

            foreach (ListViewItem item in lvQuickExtract.Items)
            {
                Settings.Default.QuickExtractPaths.Add((QuickExtractPath)item.Tag);
            }
        }
    }
}
