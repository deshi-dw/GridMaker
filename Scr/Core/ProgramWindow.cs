using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using RoboticsTools.Pathfinding;
using RoboticsTools.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RoboticsTools {
    public class ProgramWindow : Window {
        // The layout of the window.

        // FIXME: Make this a generic class and put program specific things into their own class under their own directory.
        public Grid layout;

        public PropertyWindowUI propertyWindowUI;
        public CanvasWindowUI canvasWindowUI;
        public PathfindingGrid grid;

        // FIXME: This is horrible. There is next to no need for TWO classes with the same function.
        public CanvasLayer2 gridLayer;
        public CanvasLayer fieldLayer;

        public ProgramWindow() {
            Program.dispatcher = Dispatcher;

            // Create and assign window layout.
            layout = new Grid() { };
            Content = layout;

            // Setup the window layout.
            SetLayout();
            
            // Create and setup the properties window.
            propertyWindowUI = new PropertyWindowUI(layout);
            propertyWindowUI.Name = "Property List";

            Grid.SetColumn(propertyWindowUI.layout, 2);

            //FIXME: Decide what goes in the class itself and what goes in programWindow constructor.
            // What makes sense to me is to have all generic class methods inside the class iteself and 
            // then make program specific functionally go into programWindow constructor.
            // This will require a bit of refactoring (esspecially with CanvasWindowUI) but is probably worth it.

            // for(int i = 0; i <= 100; i++) propertyWindowUI.AddProperty(new PropertyUI());

            // Create and setup the canvas window.
            canvasWindowUI = new CanvasWindowUI(layout);
            Grid.SetColumn(canvasWindowUI.layout, 0);

            // CanvasLayer fieldLayer = new CanvasLayer("2019-field.png", canvasWindowUI.canvas);
            grid = new PathfindingGrid(300, 150);

            // foreach(KeyValuePair<string, object> value in Program.data.service.Data) Console.WriteLine($"Key: {value.Key} | Value: {value.Value}");

            try { grid.x = (double)Program.data.service["grid.x"]; } catch { Console.WriteLine("grid.x not found."); }
            try { grid.y = (double)Program.data.service["grid.y"]; } catch { Console.WriteLine("grid.y not found."); }

            try {
                int resolutionX = 300;
                int resolutionY = 150;
                try { resolutionX = Convert.ToInt32(Program.data.service["grid.resolutionX"]); } catch { Console.WriteLine("grid.resolutionX not found."); }
                try { resolutionY = Convert.ToInt32(Program.data.service["grid.resolutionY"]); } catch { Console.WriteLine("grid.resolutionY not found."); }

                try { grid.SetResolution(resolutionX, resolutionY); } catch{ }
            } catch { Console.WriteLine("grid.resolution not found."); }

            try { grid.SetCmPerNode((double)Program.data.service["grid.unit"]); } catch { Console.WriteLine("grid.unit not found."); }

            Console.WriteLine($"unit = {grid.unitPerNode}");

            gridLayer = new CanvasLayer2(new Image<Alpha8>(grid.resolutionX, grid.resolutionY), canvasWindowUI.canvas);
            Canvas.SetZIndex(gridLayer.canvasImage, 10);
            fieldLayer = new CanvasLayer(SixLabors.ImageSharp.Image.Load<Rgb24>($"{Program.path}\\data\\2019-field.png"), canvasWindowUI.canvas);
            // FIXME: The Window class you are inherenting has all these control events... idiot.
            canvasWindowUI.onRenderUpdate += UpdateGrid;
            canvasWindowUI.onMouseDown += OnMouseDown;

            SetupProerties();

            RenderGridToGridLayer();
            UpdateGrid();

            gridResolutionUpdate.onClick += () => {
                grid.SetResolution(gridResolutionX.GetIntegerValue(), gridResolutionY.GetIntegerValue());
                grid.SetCmPerNode(1646.0/grid.resolutionX/2.0);
                cmPerNode.input.Text = $"{grid.unitPerNode/PathfindingGrid.PixelToCentimeterRatio}";
                RenderGridToGridLayer();
                Console.WriteLine($"ResolutionX({grid.resolutionX}), ResolutionY({grid.resolutionY})");
            };

            cmPerNodeResolutionUpdate.onClick += () => {
                double value = cmPerNode.GetDecimalValue();
                if(value == double.NaN) return;
                grid.SetCmPerNode(value);
                RenderGridToGridLayer();
            };

            gridPositionUpdate.onClick += () => {
                grid.x = gridX.GetDecimalValue();
                grid.y = gridY.GetDecimalValue();
                UpdateGrid();
            };
            
            Program.onExit += () => {
                Program.data.service["grid.x"] = grid.x;
                Program.data.service["grid.y"] = grid.y;
                
                Program.data.service["grid.resolutionX"] = grid.resolutionX;
                Program.data.service["grid.resolutionY"] = grid.resolutionY;

                Program.data.service["grid.unit"] = grid.unitPerNode/PathfindingGrid.PixelToCentimeterRatio;

                Program.data.service.SaveData();
            };
        }

        NumericPropertyUI gridResolutionX;
        NumericPropertyUI gridResolutionY;

        ButtonPropertyUI cmPerNodeResolutionUpdate;
        NumericPropertyUI cmPerNode;
        ButtonPropertyUI gridResolutionUpdate;

        NumericPropertyUI gridX;
        NumericPropertyUI gridY;
        ButtonPropertyUI gridPositionUpdate;

        public void SetupProerties() {
            gridResolutionX = new NumericPropertyUI(grid.resolutionX);
            gridResolutionX.SetLabel("Grid Resolution X");
            gridResolutionY = new NumericPropertyUI(grid.resolutionY);
            gridResolutionY.SetLabel("Grid Resolution Y");

            gridResolutionUpdate = new ButtonPropertyUI();
            gridResolutionUpdate.SetButtonText("Update");

            propertyWindowUI.AddProperty(gridResolutionX);
            propertyWindowUI.AddProperty(gridResolutionY);
            propertyWindowUI.AddProperty(gridResolutionUpdate);

            PropertyUI space = new PropertyUI();
            space.container.Height = 40;
            space.SetLabel("");
            propertyWindowUI.AddProperty(space);

            cmPerNode = new NumericPropertyUI(grid.unitPerNode/PathfindingGrid.PixelToCentimeterRatio);
            cmPerNode.SetLabel("Centimeter Per Node");
            cmPerNode.SetAllowDecimal(true);
            
            cmPerNodeResolutionUpdate = new ButtonPropertyUI();
            cmPerNodeResolutionUpdate.SetButtonText("Update");

            propertyWindowUI.AddProperty(cmPerNode);
            propertyWindowUI.AddProperty(cmPerNodeResolutionUpdate);

            space = new PropertyUI();
            space.container.Height = 40;
            space.SetLabel("");
            propertyWindowUI.AddProperty(space);

            gridX = new NumericPropertyUI(grid.x);
            gridX.SetLabel("Grid X");
            gridY = new NumericPropertyUI(grid.y);
            gridY.SetLabel("Grid Y");

            gridPositionUpdate = new ButtonPropertyUI();
            gridPositionUpdate.SetButtonText("Update");

            propertyWindowUI.AddProperty(gridX);
            propertyWindowUI.AddProperty(gridY);
            propertyWindowUI.AddProperty(gridPositionUpdate);
        }

        public void RenderGridToGridLayer() {
            gridLayer.SetSize(grid.width, grid.height);
            gridLayer.image = new Image<Alpha8>(grid.resolutionX, grid.resolutionY);
            for(int x = 0; x <= grid.resolutionX-1; x++) {
                for(int y = 0; y <= grid.resolutionY-1; y++) {
                    gridLayer.image[x, y] = new Alpha8(grid[x, y]);
                    // gridLayer.image[x, y] = new Alpha8(255);
                }   
            }
            gridLayer.Update();
            // gridLayer.canvasImage.Fill = Brushes.DeepPink;
        }

        public void OnMouseDown() {
            Point position = (Point)((canvasWindowUI.mousePositionCurrent - gridLayer.GetPosition()) * (1/grid.unitPerNode*(1/canvasWindowUI.zoom)));
            position.X = Math.Round(position.X);
            position.Y = Math.Round(position.Y);
            Console.WriteLine($"Mouse X = {position.X} : Mouse Y = {position.Y}          (Zoom = {canvasWindowUI.zoom})");
            grid[(int)position.X, (int)position.Y] = 255;
            // TODO: Pick up here where you left off.
            // Currently implementing a "brush" mechanic so the user can paint in the different areas.
            // TODO: Add intensity property
            // TODO: Add radius property
            // TODO: Add out of bounds check
            // TODO: Add dragging
            // TODO: Start using native window events.
            // TODO: Test pathfinding.
            RenderGridToGridLayer();
        }
        
        public void UpdateGrid() {
            Point position = fieldLayer.GetPosition();
            fieldLayer.SetPosition(position.X + canvasWindowUI.mouseMoveDelta.X, position.Y + canvasWindowUI.mouseMoveDelta.Y);
            position = fieldLayer.GetPosition();
            gridLayer.SetPosition(grid.x * canvasWindowUI.zoom + position.X, grid.y * canvasWindowUI.zoom + position.Y);
            // gridLayer.SetPosition(position.X + canvasWindowUI.mouseMoveDelta.X,position.Y + canvasWindowUI.mouseMoveDelta.Y);
            
            if(Math.Abs(canvasWindowUI.zoomDelta) < 10) return;
            fieldLayer.SetZoom(canvasWindowUI.zoom);
            gridLayer.canvasImage.RenderTransform = fieldLayer.canvasImage.RenderTransform;
        }

        // TODO: Add Comments 
        public void AddToLayout(UIElement element, int colunmNumber) {
            layout.Children.Add(element);
            Grid.SetColumn(element, colunmNumber);
        }

        // TODO: Add Comments
        private void SetLayout() {
            // Create, define, and assign colunms.
            layout.ColumnDefinitions.Add(new ColumnDefinition() {
                Width = new GridLength(0.5, GridUnitType.Star)
            });

            layout.ColumnDefinitions.Add(new ColumnDefinition() {
                Width = new GridLength(5, GridUnitType.Pixel)
            });

            layout.ColumnDefinitions.Add(new ColumnDefinition() {
                Width = GridLength.Auto
            });

            // Create and define the grid splitter.
            GridSplitter split = new GridSplitter() {
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            AddToLayout(split, 1);
        }
    }
}
