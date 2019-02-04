using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace RoboticsTools.UI
{
    public struct CanvasLayer {
        // FIXME: Ideally, only one of these objects would need to be stored in memory but I don't
        // know how to display an image from SixLabors on a WPF canvas.

        // the 'hidden' image of sorts that hold rbg data in memeory.
        public Image<Rgba32> image;
        // WPF's image class that stores visible image.
        public Rectangle canvasImage;

        // Creates the canvas layer.
        public CanvasLayer(Image<Rgba32> image, Canvas parent) {
            this.image = image;

            canvasImage = new Rectangle() {
                Width = image.Width,
                Height = image.Height,
                Focusable = false,
                IsEnabled = false,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetLeft(canvasImage, 0);
            Canvas.SetTop(canvasImage, 0);

            parent.Children.Add(canvasImage);
            UpdateImage();
        }

        // Renders the hidden image to the canvas.
        public void UpdateImage() {
            Rgba32[] buffer = image.GetPixelSpan().ToArray();
            WriteableBitmap bmp = new WriteableBitmap(image.Width, image.Height, image.MetaData.HorizontalResolution, image.MetaData.VerticalResolution, PixelFormats.Bgra32, null);
            bmp.Lock();

            for (int x = 0; x < image.Width; x++) {
                for (int y = 0; y < image.Height; y++) {
                    int offset = (y * image.Width + x);
                    IntPtr backbuffer = bmp.BackBuffer;
                    backbuffer += offset * 4;

                    int color = 255 << buffer[offset].A | buffer[offset].R << 16 | buffer[offset].G << 8 | buffer[offset].B;

                    System.Runtime.InteropServices.Marshal.WriteInt32(backbuffer, color);
                }
            }

            bmp.AddDirtyRect(new Int32Rect(0, 0, image.Width, image.Height));
            bmp.Unlock();

            ImageBrush renderBrush = new ImageBrush(bmp);
            canvasImage.Fill = new ImageBrush(bmp) { Stretch = Stretch.Uniform };
        }

        // Sets the zoom level of the canvas.
        public void SetZoom(double value) {
            canvasImage.RenderTransform = new ScaleTransform(value, value);
        }

        // Sets the position of the canvas.
        public void SetPosition(double x, double y) {
            Canvas.SetLeft(canvasImage, x);
            Canvas.SetTop(canvasImage, y);
        }

        public void SetPosition(Point vector) {
            SetPosition(vector.X, vector.Y);
        }

        // Gets the position of the canvas.
        public Point GetPosition() {
            return new Point(Canvas.GetLeft(canvasImage), Canvas.GetTop(canvasImage));
        }
    }
}