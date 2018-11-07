using S16.Drawing;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BSA_Browser
{
    public partial class DDSViewer : Form
    {
        private const int BackgroundBoxSize = 16;

        public string Filename { get; private set; }

        public DDSImage DDSImage { get; private set; }
        public Size ImageSize { get; private set; }

        Exception _ex;

        private DDSViewer(string filename, Stream stream)
        {
            InitializeComponent();

            this.Filename = filename;
            this.Text = filename;

            try
            {
                this.LoadImage(stream);
                this.CalculateStartingSize();
            }
            catch (Exception ex)
            {
                _ex = ex;
            }
        }

        private void DDSViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ImageBox.Image = null;
            this.DDSImage?.Dispose();
            this.DDSImage = null;
        }

        private void DDSViewer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void DDSViewer_Paint(object sender, PaintEventArgs e)
        {
            int top = ImageBox.DisplayRectangle.Top;
            int left = ImageBox.DisplayRectangle.Left;
            Graphics g = e.Graphics;
            Brush brush = Brushes.White;
            Brush start_brush = brush;

            while (top < ImageBox.DisplayRectangle.Bottom)
            {
                while (left < ImageBox.DisplayRectangle.Right)
                {
                    brush = brush == Brushes.White ? Brushes.LightGray : Brushes.White;
                    g.FillRectangle(brush, left, top, BackgroundBoxSize, BackgroundBoxSize);
                    left += BackgroundBoxSize;
                }
                top += BackgroundBoxSize;
                left = 0;
                start_brush = start_brush == Brushes.White ? Brushes.LightGray : Brushes.White;
                brush = start_brush;
            }
        }

        private void DDSViewer_Resize(object sender, EventArgs e)
        {
            if (ImageBox.Size.Height < ImageSize.Height || ImageBox.Size.Width < ImageSize.Width)
            {
                ImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                ImageBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void DDSViewer_Shown(object sender, EventArgs e)
        {
            if (_ex != null)
            {
                MessageBox.Show(this, "Couldn't load image.\n\n" + _ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void CalculateStartingSize()
        {
            int windowHeight = ImageSize.Height;
            int windowWidth = ImageSize.Width;
            var workingArea = Screen.GetWorkingArea(this.DesktopLocation);

            // Calculate new width or height depending on screen ratio
            if (workingArea.Height <= workingArea.Width)
            {
                windowHeight = Math.Min(
                    windowHeight,
                    workingArea.Height - 100);
                double dbl = (double)ImageSize.Width / (double)ImageSize.Height;
                windowWidth = Convert.ToInt32(windowHeight * dbl);
            }
            else
            {
                windowWidth = Math.Min(
                    windowWidth,
                    workingArea.Height - 100);
                double dbl = (double)ImageSize.Width / (double)ImageSize.Height;
                windowHeight = Convert.ToInt32(windowWidth * dbl);
            }

            this.ClientSize = new Size(windowWidth, windowHeight);
        }

        private void LoadImage(Stream stream)
        {
            if (Path.GetExtension(this.Filename).ToLower() == ".dds")
            {
                var dds = new DDSImage(stream);
                this.DDSImage = dds;
                this.ImageSize = new Size(dds.BitmapImage.Width, dds.BitmapImage.Height);
                this.ImageBox.Image = dds.BitmapImage;
            }
            else
            {
                var bitmap = new Bitmap(stream);
                this.ImageSize = new Size(bitmap.Width, bitmap.Height);
                this.ImageBox.Image = bitmap;
            }
        }

        public static DialogResult ShowDialog(IWin32Window owner, string filename, byte[] data)
        {
            return ShowDialog(owner, filename, new MemoryStream(data));
        }

        public static DialogResult ShowDialog(IWin32Window owner, string filename, Stream stream)
        {
            var form = new DDSViewer(filename, stream);
            return form.ShowDialog(owner);
        }
    }
}
