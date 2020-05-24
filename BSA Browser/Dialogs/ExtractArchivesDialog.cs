using BSA_Browser.Extensions;
using SharpBSABA2;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BSA_Browser.Dialogs
{
    public partial class ExtractArchivesDialog : Form
    {
        public ICollection<Archive> Archives { get; set; }
        public ICollection<Archive> Selected
        {
            get
            {
                var archives = new List<Archive>();

                foreach (ListViewItem item in lvArchives.CheckedItems)
                    archives.Add((Archive)item.Tag);

                return archives;
            }
        }

        public ExtractArchivesDialog(ICollection<Archive> archives)
        {
            InitializeComponent();

            this.Archives = archives;

            foreach (var archive in archives)
            {
                var item = new ListViewItem(Path.GetFileName(archive.FullPath))
                {
                    Checked = true,
                    Tag = archive
                };

                lvArchives.Items.Add(item);
            }

            lvArchives.EnableVisualStyles();
        }

        public static ExtractArchivesDialog ShowDialog(IWin32Window owner, ICollection<Archive> archives)
        {
            var dialog = new ExtractArchivesDialog(archives);
            dialog.ShowDialog(owner);
            return dialog;
        }

        private void ExtractArchivesDialog_Load(object sender, System.EventArgs e)
        {
            // Set it here otherwise DPI scaling will not work correctly, for some reason
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }
    }
}
