using BSA_Browser.Properties;
using SharpBSABA2;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BSA_Browser.Dialogs
{
    public partial class TextViewer : Form
    {
        public ArchiveEntry Entry { get; private set; }

        public TextViewer(ArchiveEntry entry)
        {
            InitializeComponent();

            this.Text = entry.FileName;
            this.Entry = entry;

            using (var sr = new StreamReader(entry.GetDataStream()))
            {
                textEditorControl1.Text = sr.ReadToEnd();
            }

            textEditorControl1.SetHighlighting(this.DetectHighlighting(entry.FileName));
        }

        private void TextViewer_Load(object sender, EventArgs e)
        {
            // Add this form if it doesn't exists
            if (!Settings.Default.WindowStates.Contains(this.Name))
            {
                Settings.Default.WindowStates.Add(this.Name);
            }

            // Restore window state
            Settings.Default.WindowStates[this.Name].RestoreForm(this);

            if (Application.OpenForms.Cast<Form>().Any(x => x != this && x.Name == this.Name))
            {
                this.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            }
        }

        private void TextViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save window state
            Settings.Default.WindowStates[this.Name].SaveForm(this);
            Settings.Default.Save();
        }

        private void extractMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                string ext = Path.GetExtension(this.Entry.FileName);

                sfd.AddExtension = sfd.CheckFileExists = sfd.CheckPathExists = false;
                sfd.FileName = Path.GetFileName(this.Entry.FileName);
                sfd.Filter = $"{ext.TrimStart('.').ToUpper()} file (*{ext})|*{ext}|All files|*.*";

                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    this.Entry.Extract(
                        Path.GetDirectoryName(sfd.FileName),
                        false,
                        Path.GetFileName(sfd.FileName));
                }
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string DetectHighlighting(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".json":
                    return "JavaScript";
                case ".xml":
                    return "XML";
                default:
                    return "Default";
            }
        }
    }
}
