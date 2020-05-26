using BSA_Browser.Classes;
using BSA_Browser.Dialogs;
using BSA_Browser.Extensions;
using BSA_Browser.Properties;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BSA_Browser
{
    public partial class OptionsForm : Form
    {
        public const int QuickExtractIndex = 2;

        private Encoding[] _encodings = new Encoding[]
        {
            Encoding.UTF7,
            Encoding.Default,
            Encoding.ASCII,
            Encoding.Unicode,
            Encoding.UTF32,
            Encoding.UTF8
        };

        public OptionsForm()
        {
            InitializeComponent();

            nudMaxRecentFiles.Value = Settings.Default.RecentFiles_MaxFiles;
            chbCheckForUpdates.Checked = Settings.Default.CheckForUpdates;
            chbSortBSADirectories.Checked = Settings.Default.SortArchiveDirectories;
            chbRetrieveRealSize.Checked = Settings.Default.RetrieveRealSize;

            cbEncodings.Items.AddRange(_encodings);
            cbEncodings.SelectedItem = Encoding.GetEncoding(Settings.Default.EncodingCodePage);

            chbIconsFileList.Checked = Settings.Default.Icons.HasFlag(Enums.Icons.FileList);
            chbIconsFolderTree.Checked = Settings.Default.Icons.HasFlag(Enums.Icons.FolderTree);

            chbMatchLastWriteTime.Checked = Settings.Default.MatchLastWriteTime;
            chbReplaceGNFExt.Checked = Settings.Default.ReplaceGNFExt;

            foreach (var path in Settings.Default.QuickExtractPaths)
            {
                var item = new ListViewItem(path.Name);
                item.SubItems.Add(path.Path);
                item.SubItems.Add(path.UseFolderPath ? "Yes" : "No");
                item.Tag = path;

                lvQuickExtract.Items.Add(item);
            }

            lvQuickExtract.EnableVisualStyles();
            lvPreviewing.EnableVisualStyles();

            foreach (ListViewItem item in lvPreviewing.Items)
            {
                item.Checked = Settings.Default.BuiltInPreviewing.Contains(item.Text);
            }
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
            Settings.Default.WindowStates[this.Name].RestoreForm(this, true, false);

            // Center form to Owner
            if (this.Owner != null)
            {
                this.Location = new Point(
                    Owner.Location.X + Owner.Width / 2 - Width / 2,
                    Owner.Location.Y + Owner.Height / 2 - Height / 2);
            }

            // Set it here otherwise DPI scaling will not work correctly, for some reason
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowStates[this.Name].SaveForm(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cbEncodings_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Encoding)
            {
                if (e.ListItem == Encoding.UTF7)
                    e.Value = "UTF-7 (Default)";
                else if (e.ListItem == Encoding.Default)
                    e.Value = "System Default (" + ((Encoding)e.ListItem).BodyName + ")";
                else
                    e.Value = ((Encoding)e.ListItem).EncodingName;
            }
        }

        private void btnResetToDefault_Click(object sender, EventArgs e)
        {
            nudMaxRecentFiles.Value = 30;
            chbCheckForUpdates.Checked = true;
            chbSortBSADirectories.Checked = true;
            chbRetrieveRealSize.Checked = false;
            cbEncodings.SelectedIndex = 0;
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
            Settings.Default.RecentFiles_MaxFiles = (int)nudMaxRecentFiles.Value;
            Settings.Default.CheckForUpdates = chbCheckForUpdates.Checked;
            Settings.Default.SortArchiveDirectories = chbSortBSADirectories.Checked;
            Settings.Default.RetrieveRealSize = chbRetrieveRealSize.Checked;
            Settings.Default.EncodingCodePage = (cbEncodings.SelectedItem as Encoding).CodePage;

            Enums.Icons icons = Enums.Icons.None;
            if (chbIconsFileList.Checked) icons |= Enums.Icons.FileList;
            if (chbIconsFolderTree.Checked) icons |= Enums.Icons.FolderTree;
            Settings.Default.Icons = icons;

            Settings.Default.MatchLastWriteTime = chbMatchLastWriteTime.Checked;
            Settings.Default.ReplaceGNFExt = chbReplaceGNFExt.Checked;

            Settings.Default.QuickExtractPaths.Clear();
            Settings.Default.QuickExtractPaths.AddRange(lvQuickExtract.Items
                .Cast<ListViewItem>().Select(x => (QuickExtractPath)x.Tag));

            Settings.Default.BuiltInPreviewing.Clear();
            Settings.Default.BuiltInPreviewing.AddRange(lvPreviewing.Items
                .Cast<ListViewItem>().Where(x => x.Checked).Select(x => x.Text).ToArray());
        }
    }
}
