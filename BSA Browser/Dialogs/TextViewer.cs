using BSA_Browser.Classes;
using BSA_Browser.Properties;
using ICSharpCode.TextEditor.Document;
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
        Timer _hideLabelTimer;

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

            splitContainer1.Panel2Collapsed = true;
        }

        private void TextViewer_Load(object sender, EventArgs e)
        {
            // Restore window state
            Settings.Default.WindowStates.Restore(this, false, restoreSplitContainers: false);

            if (Application.OpenForms.Cast<Form>().Any(x => x != this && x.Name == this.Name))
            {
                this.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
            }
        }

        private void TextViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save window state
            Settings.Default.WindowStates.Save(this, saveSplitContainers: false);
        }

        private void SplitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            // Draw a top border
            e.Graphics.DrawLine(SystemPens.ActiveBorder, 0, 0, e.ClipRectangle.Right, 0);
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                splitContainer1.Panel2Collapsed = true;
                // Suppress sound when clicking Esc
                e.Handled = e.SuppressKeyPress = true;
                textEditorControl1.Focus();
            }
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

        private void findMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;

            if (splitContainer1.Panel2Collapsed)
                textEditorControl1.Focus();
            else
                txtFind.Focus();
        }

        private void findNextMenuItem_Click(object sender, EventArgs e)
        {
            int startIndex = textEditorControl1.ActiveTextAreaControl.SelectionManager.SelectionCollection.Count > 0 ?
                textEditorControl1.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].EndOffset : 0;
            int offset = textEditorControl1.Text.IndexOf(txtFind.Text, startIndex,
                chbCaseSensitive.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

            if (offset == -1)
            {
                this.ReachedEOD();
                return;
            }

            var startPos = textEditorControl1.Document.OffsetToPosition(offset);
            var endPos = textEditorControl1.Document.OffsetToPosition(offset + txtFind.Text.Length);

            textEditorControl1.ActiveTextAreaControl.SelectionManager.SetSelection(startPos, endPos);
            textEditorControl1.ActiveTextAreaControl.ScrollTo(startPos.Line, startPos.Column);
        }

        private void findPreviousMenuItem_Click(object sender, EventArgs e)
        {
            int startIndex = textEditorControl1.ActiveTextAreaControl.SelectionManager.SelectionCollection.Count > 0 ?
                textEditorControl1.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].Offset : textEditorControl1.ActiveTextAreaControl.Caret.Offset;
            int offset = textEditorControl1.Text.LastIndexOf(txtFind.Text, startIndex, startIndex,
                chbCaseSensitive.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

            if (offset == -1)
            {
                this.ReachedEOD();
                return;
            }

            var startPos = textEditorControl1.Document.OffsetToPosition(offset);
            var endPos = textEditorControl1.Document.OffsetToPosition(offset + txtFind.Text.Length);

            textEditorControl1.ActiveTextAreaControl.SelectionManager.SetSelection(startPos, endPos);
            textEditorControl1.ActiveTextAreaControl.ScrollTo(startPos.Line, startPos.Column);
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            LimitedAction.RunAfter(2, 500, delegate
            {
                if (string.IsNullOrEmpty(txtFind.Text))
                    this.ClearHighlighting();
                else
                    this.HighlightText(txtFind.Text);
            });
        }

        private void ClearHighlighting()
        {
            textEditorControl1.Document.MarkerStrategy.RemoveAll(x => true);
            textEditorControl1.ActiveTextAreaControl.Refresh();
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

        private void HighlightText(string text)
        {
            textEditorControl1.Document.MarkerStrategy.RemoveAll(x => true);

            int startIndex = 0;
            int offset = -1;

            do
            {
                offset = textEditorControl1.Text.IndexOf(text, startIndex);

                if (offset < 0)
                    continue;

                startIndex = offset + text.Length;

                textEditorControl1.Document.MarkerStrategy.AddMarker(
                    new TextMarker(offset, text.Length, TextMarkerType.SolidBlock, Color.Yellow, Color.Black));
            }
            while (offset >= 0);

            textEditorControl1.ActiveTextAreaControl.Refresh();
        }

        private void ReachedEOD()
        {
            if (_hideLabelTimer == null)
            {
                _hideLabelTimer = new Timer();
                _hideLabelTimer.Interval = 2000;
                _hideLabelTimer.Tick += delegate { lReachedEOD.Visible = false; _hideLabelTimer.Stop(); };
            }

            _hideLabelTimer.Stop();
            lReachedEOD.Visible = true;
            _hideLabelTimer.Start();
        }
    }
}
