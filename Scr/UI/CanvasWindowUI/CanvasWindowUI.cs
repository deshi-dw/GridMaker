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

        public double zoom = 1.0;
        public int zoomDelta = 1;
        private double minZoom = 0.5;
        private double maxZoom = 4.0;
        private double zoomSensitivity = 0.001;

        public Point mousePositionCurrent;
        public Point mousePositionPrevious;

        public Point mouseMoveDelta;

        public delegate void RenderEvent();
        public event RenderEvent onRenderUpdate;
        public event RenderEvent onMouseDown;

        public MouseEventArgs lastMouseEvent;

        public CanvasWindowUI(IAddChild parent) : base(parent) {
            canvas = new Canvas() {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = Brushes.AliceBlue
            };

            canvas.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);
            canvas.MouseDown += new MouseButtonEventHandler(OnMouseDown);
            canvas.MouseMove += new MouseEventHandler(OnMouseMove);

            Panel.SetZIndex(layout, -100);
            layout.Children.Add(canvas);

            // CanvasLayer layer = new CanvasLayer(new Image<Rgb24>(Configuration.Default, 1600, 800, new Rgb24(255, 0, 0)), canvas);
            //  fieldLayer = new CanvasLayer("2019-field.png", canvas);
            //  gridLayer = new CanvasLayer(new Image<Rgb24>(Configuration.Default, 300, 150, new Rgb24(255, 0, 0)), canvas);
        }

        public void OnMouseMove(object sender, MouseEventArgs e) {
            lastMouseEvent = e;
            // if(e.MiddleButton == MouseButtonState.Released) return;

            mousePositionCurrent = e.GetPosition(canvas);

            if(Math.Abs((mousePositionCurrent.X - mousePositionPrevious.X) + (mousePositionCurrent.Y - mousePositionPrevious.Y)) > 50) return;

            mouseMoveDelta = (Point)(mousePositionCurrent - mousePositionPrevious);

            // Point position = fieldLayer.GetPosition();
            // fieldLayer.SetPosition(position.X + mouseMoveDelta.X, position.Y + mouseMoveDelta.Y);

            // Console.Write($"Position = {position} | MoveDelta = {mouseMoveDelta}   ( Current[{mousePositionCurrent}] - Previous[{mousePositionPrevious}] )");

            // position = gridLayer.GetPosition();
            // gridLayer.SetPosition(position.X + mouseMoveDelta.X, position.Y + mouseMoveDelta.Y);
            if(onRenderUpdate != null) onRenderUpdate();
            mousePositionPrevious = mousePositionCurrent;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e) {
            mousePositionCurrent = e.GetPosition(canvas);
            mousePositionPrevious = mousePositionCurrent;
             if(onMouseDown != null) onMouseDown();
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
            zoomDelta = e.Delta;
            if(Math.Abs(zoomDelta) < 10) return;
            zoom = Math.Max(Math.Min(zoom + e.Delta * zoomSensitivity, maxZoom), minZoom);
            // Console.WriteLine($"Zoom: {zoom} ( Delta {e.Delta} )");

            // fieldLayer.SetZoom(zoom);
            // gridLayer.SetZoom(zoom);
            if(onRenderUpdate != null) onRenderUpdate();
        }
    }
}