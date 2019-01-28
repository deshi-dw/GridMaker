using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using RoboticsTools.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace RoboticsTools.UI {
    public struct CanvasLayer {
        // TODO: Add Comments.
        public Image<Rgb24> image;
        public Rectangle canvasImage;

        public CanvasLayer(int width, int height, Canvas parent) {
            this.image = new Image<Rgb24>(width, height);
            canvasImage = new Rectangle() {
                Width = width,
                Height = height,
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetLeft(canvasImage, 0);
            Canvas.SetTop(canvasImage, 0);

            parent.Children.Add(canvasImage);
            Update();
        }

        public CanvasLayer(string relativeImagePath, Canvas parent) {
            // FIXME: Something like the program data path should probably be saved in a json file.
            this.image = SixLabors.ImageSharp.Image.Load<Rgb24>($"{Program.path}\\data\\{relativeImagePath}");
            // Console.WriteLine($"Loaded image. width={image.Width}, height={image.Height}");

            canvasImage = new Rectangle() {
                Width = image.Width,
                Height = image.Height,
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetLeft(canvasImage, 0);
            Canvas.SetTop(canvasImage, 0);

            parent.Children.Add(canvasImage);
            Update();
        }

        public CanvasLayer(Image<Rgb24> image, Canvas parent) {
            this.image = image;

            canvasImage = new Rectangle() {
                Width = image.Width,
                Height = image.Height,
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetLeft(canvasImage, 0);
            Canvas.SetTop(canvasImage, 0);

            parent.Children.Add(canvasImage);
            Update();
        }

        public void DrawRect(Vector2Int start, Vector2Int end, Rgb24 fill) {
            Vector2Int direction = (start - end).Direction();

            for (int x = start.x; x != end.x - direction.x; x -= direction.x) {
                for (int y = start.y; y != end.y - direction.x; y -= direction.y) {
                    image[x, y] = fill;
                }
            }

            Program.dispatcher.Invoke(Update);
        }

        public void Update() {
            // image.Save<Rgb24>($"{Program.path}\\data\\render.bmp");
            // ImageBrush renderBrush = new ImageBrush(new BitmapImage(new Uri($"{Program.path}\\data\\render.bmp")));
            // renderBrush.Stretch = Stretch.None;
            // canvasImage.Fill = renderBrush;

            Rgb24[] buffer = image.GetPixelSpan().ToArray();

            WriteableBitmap bmp = new WriteableBitmap(image.Width, image.Height, image.MetaData.HorizontalResolution, image.MetaData.VerticalResolution, PixelFormats.Bgra32, null);
            bmp.Lock();

            for (int x = 0; x < image.Width; x++) {
                for (int y = 0; y < image.Height; y++) {
                    int offset = (y * image.Width + x);

                    IntPtr backbuffer = bmp.BackBuffer;
                    backbuffer += offset * 4;

                    // int color = a << 24 | r << 16 | g << 8 | b;
                    // int color = buffer[offset].B << 16 | buffer[offset].G << 8 | buffer[offset].R;
                    int color = 255 << 24 | buffer[offset].R << 16 | buffer[offset].G << 8 | buffer[offset].B;
                    // Console.WriteLine($"R:{buffer[offset].R} | G:{buffer[offset].G} | B:{buffer[offset].B}");
                    //  Console.WriteLine($"Color: {Convert.ToString(color, 2)}");

                    System.Runtime.InteropServices.Marshal.WriteInt32(backbuffer, color);
                }
            }

            bmp.AddDirtyRect(new Int32Rect(0, 0, image.Width, image.Height));
            bmp.Unlock();

            ImageBrush renderBrush = new ImageBrush(bmp);
            canvasImage.Fill = new ImageBrush(bmp) { Stretch = Stretch.Uniform };
        }

        public void SetZoom(double value, Point center) {
            canvasImage.RenderTransform = new ScaleTransform(value, value, center.X, center.Y);
        }

        public void SetZoom(double value) {
            canvasImage.RenderTransform = new ScaleTransform(value, value);
        }

        public void SetSize(double width, double height) {
            canvasImage.Width = width;
            canvasImage.Height = height;
        }

        public void SetPosition(double x, double y) {
            Canvas.SetLeft(canvasImage, x);
            Canvas.SetTop(canvasImage, y);
        }

        public Point GetPosition() {
            return new Point(Canvas.GetLeft(canvasImage), Canvas.GetTop(canvasImage));
        }
    }
}