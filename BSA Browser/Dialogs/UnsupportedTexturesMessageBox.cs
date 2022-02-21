using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpBSABA2;

namespace BSA_Browser.Dialogs
{
    public partial class UnsupportedTexturesMessageBox : Form
    {
        private const int NormalHeight = 210;

        private bool _shown = false;

        public UnsupportedTexturesMessageBox()
        {
            InitializeComponent();
            // Set here so list view can be visible in editor
            this.Height = NormalHeight;
        }

        public UnsupportedTexturesMessageBox(IEnumerable<ArchiveEntry> entries) : this()
        {
            int count = 0;
            foreach (var entry in entries)
            {
                count++;
                var lvi = new ListViewItem(entry.FullPath);
                if (entry is SharpBSABA2.BA2Util.BA2TextureEntry texture)
                {
                    lvi.SubItems.Add(texture.format.ToString());
                }
                else if (entry is SharpBSABA2.BA2Util.BA2GNFEntry gnf)
                {
                    lvi.SubItems.Add(gnf.format.ToString());
                }
                listView1.Items.Add(lvi);
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (_shown == false)
            {
                this.Height = NormalHeight + 200;
                listView1.Visible = true;
            }
            else
            {
                this.Height = NormalHeight;
                listView1.Visible = false;
            }
            _shown ^= true;
        }
    }
}
