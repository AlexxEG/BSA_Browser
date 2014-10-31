using BSA_Browser.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace BSA_Browser
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            txtFallout3Path.Text = Properties.Settings.Default.Fallout3_QuickExportPath;
            txtFalloutNVPath.Text = Properties.Settings.Default.FalloutNV_QuickExportPath;
            txtOblivionPath.Text = Properties.Settings.Default.Oblivion_QuickExportPath;
            txtSkyrimPath.Text = Properties.Settings.Default.Skyrim_QuickExportPath;

            chbFallout3.Checked = Properties.Settings.Default.Fallout3_QuickExportEnable;
            txtFallout3Path.Enabled = Properties.Settings.Default.Fallout3_QuickExportEnable;

            chbFalloutNV.Checked = Properties.Settings.Default.FalloutNV_QuickExportEnable;
            txtFalloutNVPath.Enabled = Properties.Settings.Default.FalloutNV_QuickExportEnable;

            chbOblivion.Checked = Properties.Settings.Default.Oblivion_QuickExportEnable;
            txtOblivionPath.Enabled = Properties.Settings.Default.Oblivion_QuickExportEnable;

            chbSkyrim.Checked = Properties.Settings.Default.Skyrim_QuickExportEnable;
            txtSkyrimPath.Enabled = Properties.Settings.Default.Skyrim_QuickExportEnable;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtFallout3Path.Text))
                folderBrowserDialog1.SelectedPath = txtFallout3Path.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtFallout3Path.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtFalloutNVPath.Text))
                folderBrowserDialog1.SelectedPath = txtFalloutNVPath.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtFalloutNVPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnBrowse3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtOblivionPath.Text))
                folderBrowserDialog1.SelectedPath = txtOblivionPath.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtOblivionPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnBrowse4_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtSkyrimPath.Text))
                folderBrowserDialog1.SelectedPath = txtSkyrimPath.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtSkyrimPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void chbFallout3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Fallout3_QuickExportEnable = chbFallout3.Checked;
            txtFallout3Path.Enabled = chbFallout3.Checked;
        }

        private void chbFalloutNV_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FalloutNV_QuickExportEnable = chbFalloutNV.Checked;
            txtFalloutNVPath.Enabled = chbFalloutNV.Checked;
        }

        private void chbOblivion_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Oblivion_QuickExportEnable = chbOblivion.Checked;
            txtOblivionPath.Enabled = chbOblivion.Checked;
        }

        private void chbSkyrim_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Skyrim_QuickExportEnable = chbSkyrim.Checked;
            txtSkyrimPath.Enabled = chbSkyrim.Checked;
        }
    }
}
