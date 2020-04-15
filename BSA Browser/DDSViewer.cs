using Pfim;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BSA_Browser
{
    public partial class DDSViewer : Form
    {
        private const int BackgroundBoxSize = 16;

        public string DDSFormat { get; private set; }
        public string Filename { get; private set; }

        public Size ImageSize { get; private set; }

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
                throw ex;
            }
        }

        private void DDSViewer_Load(object sender, EventArgs e)
        {
            string pixelFormat = string.IsNullOrEmpty(this.DDSFormat) ? string.Empty : this.DDSFormat + " - ";
            this.Text += $" - {pixelFormat}{ImageSize.Width}x{ImageSize.Height}";
        }

        private void DDSViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ImageBox.Image = null;
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
                PixelFormat format;

                Dds image = (Dds)Pfim.Pfim.FromStream(stream);

                switch (image.Format)
                {
                    case Pfim.ImageFormat.Rgb24:
                        format = PixelFormat.Format24bppRgb;
                        break;
                    case Pfim.ImageFormat.Rgba32:
                        format = PixelFormat.Format32bppArgb;
                        break;
                    default:
                        throw new NotImplementedException("Unsupported Pfim image format: " + image.Format.ToString());
                }

                this.DDSFormat = image.Header.PixelFormat.FourCC.ToString();

                var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                try
                {
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                    var bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    this.ImageSize = new Size(bitmap.Width, bitmap.Height);
                    this.ImageBox.Image = bitmap;
                }
                finally
                {
                    handle.Free();
                }
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
