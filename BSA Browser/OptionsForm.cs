﻿using BSA_Browser.Classes;
using BSA_Browser.Dialogs;
using BSA_Browser.Extensions;
using BSA_Browser.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        private readonly bool _fileAssociationInitialValue;
        private readonly bool _shellIntegrationInitialValue;

        public OptionsForm()
        {
            InitializeComponent();

            nudMaxRecentFiles.Value = Settings.Default.RecentFiles_MaxFiles;
            chbCheckForUpdates.Checked = Settings.Default.CheckForUpdates;
            chbRememberArchives.Checked = Settings.Default.RememberArchives;
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

            nudMaxResolutionW.Value = Settings.Default.PreviewMaxResolution.Width;
            nudMaxResolutionH.Value = Settings.Default.PreviewMaxResolution.Height;

            foreach (ListViewItem item in lvPreviewing.Items)
            {
                item.Checked = Settings.Default.BuiltInPreviewing.Contains(item.Text);
            }

            cbAssociateFiles.Checked = _fileAssociationInitialValue = FileAssociation.GetFileAssociationEnabled();
            cbShellIntegration.Checked = _shellIntegrationInitialValue = FileAssociation.GetShellIntegrationEnabled();
            cbShellIntegration.Enabled = cbAssociateFiles.Checked;
        }

        public OptionsForm(int tabPage)
            : this()
        {
            tabControl1.SelectedIndex = tabPage;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            // Restore only columns and location
            Settings.Default.WindowStates.Restore(this, false, true, false, true, false);

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
            Settings.Default.WindowStates.Save(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cbEncodings_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Encoding encoding)
            {
                if (encoding == Encoding.UTF7)
                    e.Value = "UTF-7 (Default)";
                else if (encoding == Encoding.Default)
                    e.Value = "System Default (" + encoding.BodyName + ")";
                else
                    e.Value = encoding.EncodingName;
            }
        }

        private void cbAssociateFiles_CheckedChanged(object sender, EventArgs e)
        {
            cbShellIntegration.Enabled = cbAssociateFiles.Checked;
        }

        private void btnResetToDefaultGeneral_Click(object sender, EventArgs e)
        {
            var s = Settings.Default;

            nudMaxRecentFiles.Value = GetDefaultPropertyValueInt(nameof(s.RecentFiles_MaxFiles));
            chbCheckForUpdates.Checked = GetDefaultPropertyValueBool(nameof(s.CheckForUpdates));
            chbRememberArchives.Checked = GetDefaultPropertyValueBool(nameof(s.RememberArchives));
            chbSortBSADirectories.Checked = GetDefaultPropertyValueBool(nameof(s.SortArchiveDirectories));
            chbRetrieveRealSize.Checked = GetDefaultPropertyValueBool(nameof(s.RetrieveRealSize));

            cbEncodings.SelectedIndex =
                cbEncodings.Items.IndexOf(Encoding.GetEncoding(GetDefaultPropertyValueInt(nameof(s.EncodingCodePage))));

            var icons = (Enums.Icons)Enum.Parse(typeof(Enums.Icons), GetDefaultPropertyValue(nameof(s.Icons)));
            chbIconsFileList.Checked = icons.HasFlag(Enums.Icons.FileList);
            chbIconsFolderTree.Checked = icons.HasFlag(Enums.Icons.FolderTree);
        }

        private void btnResetToDefaultExtraction_Click(object sender, EventArgs e)
        {
            var s = Settings.Default;

            chbMatchLastWriteTime.Checked = GetDefaultPropertyValueBool(nameof(s.MatchLastWriteTime));
            chbReplaceGNFExt.Checked = GetDefaultPropertyValueBool(nameof(s.ReplaceGNFExt));
        }

        private void btnResetToDefaultPreview_Click(object sender, EventArgs e)
        {
            var s = Settings.Default;
            Size maxResolution = this.ParseSizeString(s.Properties[nameof(s.PreviewMaxResolution)].DefaultValue.ToString());

            nudMaxResolutionW.Value = maxResolution.Width;
            nudMaxResolutionH.Value = maxResolution.Height;

            var builtInPreviewing = s.Properties[nameof(s.BuiltInPreviewing)].DefaultValue.ToString();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(StringCollection));
            var stringReader = new StringReader(builtInPreviewing);
            var sc = (StringCollection)serializer.Deserialize(stringReader);

            foreach (string str in sc)
            {
                lvPreviewing.Items
                    .Cast<ListViewItem>()
                    .First(x => x.Text == str).Checked = true;
            }
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
            Settings.Default.RememberArchives = chbRememberArchives.Checked;
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

            Settings.Default.PreviewMaxResolution = new Size(
                (int)nudMaxResolutionW.Value,
                (int)nudMaxResolutionH.Value);

            Settings.Default.BuiltInPreviewing.Clear();
            Settings.Default.BuiltInPreviewing.AddRange(lvPreviewing.Items
                .Cast<ListViewItem>().Where(x => x.Checked).Select(x => x.Text).ToArray());

            if (this.CheckAssociationAndIntegrationChanged())
            {
                if (FileAssociation.HasAdminPrivileges() == false)
                {
                    MessageBox.Show(this,
                        "File association and shell integration requires administrative rights.\n\nPrompt will be shown automatically.",
                        "BSA Browser",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                // If file association is disabled we don't care whether shell integration has changed or not
                bool? shellIntegration = null;
                if (cbAssociateFiles.Checked)
                    shellIntegration = cbShellIntegration.Checked;

                this.ToggleAssociateAndIntegration(cbAssociateFiles.Checked, shellIntegration);
            }
        }

        private bool CheckAssociationAndIntegrationChanged()
        {
            // If file association is disabled we don't care whether shell integration has changed or not
            bool? shellIntegration = null;
            if (cbAssociateFiles.Checked)
                shellIntegration = cbShellIntegration.Checked;

            return cbAssociateFiles.Checked != _fileAssociationInitialValue
                || shellIntegration != null && shellIntegration != _shellIntegrationInitialValue;
        }

        private bool ToggleAssociateAndIntegration(bool fileAssociation, bool? shellIntegration)
        {
            var args = new List<string>();

            if (fileAssociation != _fileAssociationInitialValue)
                args.Add(fileAssociation ? "--associate" : "--associate-disable");

            // Ignore 'shellIntegration' if it's null, don't even compare to cached
            if (shellIntegration != null && shellIntegration != _shellIntegrationInitialValue)
                args.Add(shellIntegration == true ? "--integration" : "--integration-disable");

            if (args.Count == 0)
                return false;

            if (FileAssociation.HasAdminPrivileges())
            {
                FileAssociation.ToggleAssociationAndIntegration(args.ToArray());
            }
            else
            {
                // Prompt for admin privileges
                var process = Process.Start(new ProcessStartInfo(Application.ExecutablePath)
                {
                    Arguments = string.Join(" ", args),
                    UseShellExecute = true,
                    Verb = "runas"
                });
                process.WaitForExit();
            }

            return true;
        }

        private string GetDefaultPropertyValue(string propertyName)
        {
            return Settings.Default.Properties[propertyName].DefaultValue.ToString();
        }

        private bool GetDefaultPropertyValueBool(string propertyName)
        {
            return bool.Parse(GetDefaultPropertyValue(propertyName));
        }

        private int GetDefaultPropertyValueInt(string propertyName)
        {
            return int.Parse(GetDefaultPropertyValue(propertyName));
        }

        private Size ParseSizeString(string stringToParse)
        {
            string[] parts = stringToParse.Replace(" ", "").Split(',');
            return new Size(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}
