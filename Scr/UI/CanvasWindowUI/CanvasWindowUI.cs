using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RoboticsTools.UI {
    public class CanvasWindowUI : WindowUI {
        // TODO: Add Comments.
        // FIXME: Offload some functionallity to another class.
        // Canvas movement is good to keep in this class but other things like
        // name specific layers should be defined somewhere else.
        // Ex. fieldLayer and gridLayer should be made in ProgramWidnow's constructor.
        public Canvas canvas;

        public CanvasWindowUI(IAddChild parent) : base(parent) {
            canvas = new Canvas() {
                Width = double.NaN,
                Height = double.NaN,
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = Brushes.AliceBlue
            };

            Panel.SetZIndex(layout, -100);
            layout.Children.Add(canvas);
        }

        // Sets the zoom level of the canvas.
        public void SetZoom(double value) {
            canvas.RenderTransform = new ScaleTransform(value, value);
        }

        // Sets the position of the canvas.
        public void SetPosition(double x, double y) {
            Canvas.SetLeft(canvas, x);
            Canvas.SetTop(canvas, y);
        }

        // Sets the position of the canvas.
        public void SetPosition(Point position) {
            Canvas.SetLeft(canvas, position.X);
            Canvas.SetTop(canvas, position.Y);
        }

        // Gets the position of the canvas.
        public Point GetPosition() {
            return new Point(Canvas.GetLeft(canvas), Canvas.GetTop(canvas));
        }
    }
}