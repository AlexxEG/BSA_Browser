using System;
using System.IO;
using System.Windows.Forms;
using BSA_Browser.Properties;

namespace BSA_Browser
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            lvQuickExtract.ContextMenu = contextMenu1;

            // Restore enabled from Settings
            lvQuickExtract.Items[0].Checked = Settings.Default.Fallout3_QuickExportEnable;
            lvQuickExtract.Items[1].Checked = Settings.Default.FalloutNV_QuickExportEnable;
            lvQuickExtract.Items[2].Checked = Settings.Default.Oblivion_QuickExportEnable;
            lvQuickExtract.Items[3].Checked = Settings.Default.Skyrim_QuickExportEnable;

            // Restore game paths from Settings
            lvQuickExtract.Items[0].SubItems[2].Text = Settings.Default.Fallout3_QuickExportPath;
            lvQuickExtract.Items[1].SubItems[2].Text = Settings.Default.FalloutNV_QuickExportPath;
            lvQuickExtract.Items[2].SubItems[2].Text = Settings.Default.Oblivion_QuickExportPath;
            lvQuickExtract.Items[3].SubItems[2].Text = Settings.Default.Skyrim_QuickExportPath;

            Program.SendMessage(lvQuickExtract.Handle, 0x127, 0x10001, 0);
            Program.SendMessage(lvQuickExtract.Handle, 0x1000 + 54, 0x00010000, 0x00010000);
            Program.SetWindowTheme(lvQuickExtract.Handle, "explorer", null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            // Reset MenuItems
            setPathMenuItem.Enabled = clearPathMenuItem.Enabled = true;

            if (lvQuickExtract.SelectedItems.Count == 0)
            {
                setPathMenuItem.Enabled = false;
                clearPathMenuItem.Enabled = false;
            }
            else if (lvQuickExtract.SelectedItems.Count > 1)
            {
                // Disable 'Set Path' if there is multiple selected items.
                setPathMenuItem.Enabled = false;
            }
        }

        private void setPathMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = lvQuickExtract.SelectedItems[0];

            if (Directory.Exists(item.SubItems[2].Text))
                folderBrowserDialog1.SelectedPath = item.SubItems[2].Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                item.SubItems[2].Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void clearPathMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvQuickExtract.SelectedItems)
            {
                item.SubItems[2].Text = string.Empty;
            }
        }

        private void lvQuickExtract_Enter(object sender, EventArgs e)
        {
            Program.SendMessage(lvQuickExtract.Handle, 0x127, 0x10001, 0);
        }

        private void lvQuickExtract_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.SendMessage(lvQuickExtract.Handle, 0x127, 0x10001, 0);
        }

        public void Save()
        {
            Settings.Default.Fallout3_QuickExportEnable = lvQuickExtract.Items[0].Checked;
            Settings.Default.FalloutNV_QuickExportEnable = lvQuickExtract.Items[1].Checked;
            Settings.Default.Oblivion_QuickExportEnable = lvQuickExtract.Items[2].Checked;
            Settings.Default.Skyrim_QuickExportEnable = lvQuickExtract.Items[3].Checked;

            Settings.Default.Fallout3_QuickExportPath = lvQuickExtract.Items[0].SubItems[2].Text;
            Settings.Default.FalloutNV_QuickExportPath = lvQuickExtract.Items[1].SubItems[2].Text;
            Settings.Default.Oblivion_QuickExportPath = lvQuickExtract.Items[2].SubItems[2].Text;
            Settings.Default.Skyrim_QuickExportPath = lvQuickExtract.Items[3].SubItems[2].Text;
        }
    }
}
