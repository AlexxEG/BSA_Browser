using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BSA_Browser.Properties;
using Pfim;

namespace BSA_Browser.Preview
{
    public partial class DDSViewer : Form
    {
        private const int BackgroundBoxSize = 16;

        public string DDSFormat { get; private set; }
        public string Filename { get; private set; }

        public Size ImageSize { get; private set; }
        public Size ImageSizeOriginal { get; private set; }

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
            catch (Exception)
            {
                throw;
            }

            this.CreateBackground();
        }

        private void DDSViewer_Load(object sender, EventArgs e)
        {
            string pixelFormat = string.IsNullOrEmpty(this.DDSFormat) ? string.Empty : this.DDSFormat + " - ";
            this.Text += $" - {pixelFormat}{ImageSizeOriginal.Width}x{ImageSizeOriginal.Height}";

            if (ImageSize != ImageSizeOriginal)
                this.Text += $" - Scaled {ImageSize.Width}x{ImageSize.Height}";
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

        private void CreateBackground()
        {
            var bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.White, 0, 0, BackgroundBoxSize, BackgroundBoxSize);
                g.FillRectangle(Brushes.LightGray, 16, 0, BackgroundBoxSize, BackgroundBoxSize);
                g.FillRectangle(Brushes.LightGray, 0, 16, BackgroundBoxSize, BackgroundBoxSize);
                g.FillRectangle(Brushes.White, 16, 16, BackgroundBoxSize, BackgroundBoxSize);
            }

            this.BackgroundImage = bitmap;
            this.BackgroundImageLayout = ImageLayout.Tile;
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
                    workingArea.Width - 100);
                double dbl = (double)ImageSize.Width / (double)ImageSize.Height;
                windowHeight = Convert.ToInt32(windowWidth * dbl);
            }

            this.ClientSize = new Size(windowWidth, windowHeight);
        }

        private void LoadImage(Stream stream)
        {
            Size maxReso = Settings.Default.PreviewMaxResolution;

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
                    this.ImageSizeOriginal = new Size(bitmap.Width, bitmap.Height);
                    var resized = this.ResizeBitmap(bitmap, maxReso.Width, maxReso.Height);

                    if (bitmap != resized)
                        bitmap.Dispose();

                    this.ImageSize = new Size(resized.Width, resized.Height);
                    this.ImageBox.Image = resized;
                }
                finally
                {
                    handle.Free();
                }
            }
            else
            {
                var bitmap = new Bitmap(stream);
                this.ImageSizeOriginal = new Size(bitmap.Width, bitmap.Height);
                var resized = this.ResizeBitmap(bitmap, maxReso.Width, maxReso.Height);

                if (bitmap != resized)
                    bitmap.Dispose();

                this.ImageSize = new Size(resized.Width, resized.Height);
                this.ImageBox.Image = resized;
            }
        }

        private Bitmap ResizeBitmap(Bitmap bitmap, int maxWidth, int maxHeight)
        {
            if (bitmap.Width <= maxWidth && bitmap.Height <= maxHeight)
                return bitmap;

            var newSize = this.CalculateNewSize(bitmap.Size, new Size(maxWidth, maxHeight));

            return new Bitmap(bitmap, newSize);
        }

        private Size CalculateNewSize(Size size, Size maxSize)
        {
            if (size.Width <= maxSize.Width && size.Height <= maxSize.Height)
                return size;

            // Taken from: https://stackoverflow.com/questions/1940581/c-sharp-image-resizing-to-different-size-while-preserving-aspect-ratio

            // Figure out the ratio
            double ratioX = (double)maxSize.Width / (double)size.Width;
            double ratioY = (double)maxSize.Height / (double)size.Height;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(size.Height * ratio);
            int newWidth = Convert.ToInt32(size.Width * ratio);

            return new Size(newWidth, newHeight);
        }

        public static void Show(IWin32Window owner, string filename, byte[] data)
        {
            Show(owner, filename, new MemoryStream(data));
        }

        public static void Show(IWin32Window owner, string filename, Stream stream)
        {
            var form = new DDSViewer(filename, stream);
            form.Show(owner);
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
