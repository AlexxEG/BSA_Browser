using System;
using System.Drawing;
using System.Windows.Forms;
using S16.Drawing;
using SharpBSABA2;

namespace BSA_Browser
{
    public partial class DDSViewer : Form
    {
        private const int BackgroundBoxSize = 16;

        public DDSImage DDSImage { get; private set; }

        private DDSViewer(ArchiveEntry entry)
        {
            InitializeComponent();

            var stream = entry.GetDataStream();
            var dds = new DDSImage(stream);

            this.DDSImage = dds;
            this.ImageBox.Image = dds.BitmapImage;
            this.CalculateStartingSize();
            this.Text = entry.FileName;
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
            if (ImageBox.Size.Height < DDSImage.BitmapImage.Height || ImageBox.Size.Width < DDSImage.BitmapImage.Width)
            {
                ImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                ImageBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void CalculateStartingSize()
        {
            int windowHeight = DDSImage.BitmapImage.Height;
            int windowWidth = DDSImage.BitmapImage.Width;
            var workingArea = Screen.GetWorkingArea(this.DesktopLocation);

            // Calculate new width or height depending on screen ratio
            if (workingArea.Height <= workingArea.Width)
            {
                windowHeight = Math.Min(
                    windowHeight,
                    workingArea.Height - 100);
                double dbl = (double)DDSImage.BitmapImage.Width / (double)DDSImage.BitmapImage.Height;
                windowWidth = Convert.ToInt32(windowHeight * dbl);
            }
            else
            {
                windowWidth = Math.Min(
                    windowWidth,
                    workingArea.Height - 100);
                double dbl = (double)DDSImage.BitmapImage.Width / (double)DDSImage.BitmapImage.Height;
                windowHeight = Convert.ToInt32(windowWidth * dbl);
            }

            this.ClientSize = new Size(windowWidth, windowHeight);
        }

        public static DialogResult ShowDialog(IWin32Window owner, ArchiveEntry entry)
        {
            var form = new DDSViewer(entry);
            return form.ShowDialog(owner);
        }
    }
}
