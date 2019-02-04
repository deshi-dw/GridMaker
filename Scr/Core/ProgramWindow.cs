using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoboticsTools.Pathfinding;
using RoboticsTools.UI;
using RoboticsTools.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RoboticsTools {
    public class ProgramWindow : Window {
        // The layout of the window.

        // FIXME: Make this a generic class and put program specific things into their own class under their own directory.
        public Grid layout;

        public PropertyWindowUI propertyWindowUI;
        public CanvasWindowUI canvasWindowUI;

        public CanvasLayer gridLayer;
        public CanvasLayer pathLayer;
        public CanvasLayer fieldLayer;

        public CanvasBrushSquare brush;

        public PathfindingGrid grid;
        Pathfinding.Pathfinding pathfinding;
        Dictionary<string, Position> positions;

        public double zoom = 1.0;
        private double minZoom = 0.5;
        private double maxZoom = 4.0;
        private double zoomSensitivity = 0.001;

        Point mousePositionPrevious;
        public int mouseWheelDelta;
        public Point mouseMoveDelta;

        public ProgramWindow() {
            Program.dispatcher = Dispatcher;

            // Create and assign window layout.
            layout = new Grid() { };
            layout.Focusable = false;
            Content = layout;

            // Setup the window layout.
            SetLayout();

            // Create and setup the properties window.
            propertyWindowUI = new PropertyWindowUI(layout);
            propertyWindowUI.Name = "Property List";

            Grid.SetColumn(propertyWindowUI.layout, 2);

            // Create and setup the canvas window.
            canvasWindowUI = new CanvasWindowUI(layout);
            Grid.SetColumn(canvasWindowUI.layout, 0);

            positions = new Dictionary<string, Position>();

            SetSaveData();

            fieldLayer = new CanvasLayer(SixLabors.ImageSharp.Image.Load<Rgba32>($"{Program.path}\\data\\2019-field.png"), canvasWindowUI.canvas);
            gridLayer = new CanvasLayer(new SixLabors.ImageSharp.Image<Rgba32>(grid.resolutionX, grid.resolutionY), canvasWindowUI.canvas);
            pathLayer = new CanvasLayer(new SixLabors.ImageSharp.Image<Rgba32>(grid.resolutionX, grid.resolutionY), canvasWindowUI.canvas);

            Canvas.SetZIndex(gridLayer.canvasImage, 10);
            Canvas.SetZIndex(pathLayer.canvasImage, 11);

            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.MouseDown += new MouseButtonEventHandler(OnMouseDown);
            this.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);

            this.Focusable = true;

            // Mouse.AddMouseDownHandler(Content,)
            // FIXME: Come back to this.

            SetupProerties();

            RenderGrid();
            UpdatePosition();

            gridResUpdateUI.onClick += () => {
                grid.SetResolution((int) gridResUI.GetValue1(), (int) gridResUI.GetValue2());
                grid.SetUnitSize(1646.0 / grid.resolutionX / 2.0);
                gridUnitUI.SetValue((decimal) grid.unit);
                RenderGrid();
            };

            gridPosUpdateUI.onClick += () => {
                grid.x = (double) gridPosUI.GetValue1();
                grid.y = (double) gridPosUI.GetValue2();
                UpdatePosition();
            };

            gridUnitUpdateUI.onClick += () => {
                double value = (double) gridUnitUI.GetValue();
                if (value == double.NaN) return;
                grid.SetUnitSize(value);
                RenderGrid();
                UpdatePosition();
            };

            brushUpdateUI.onClick += () => {
                brush.size = (int) brushSizeUI.GetValue();
            };

            gridPositionsSetUI.onClick += () => {
                string key = gridPositionsUI.GetSelected();
                //  string label = gridPositionsLabelUI.GetValue();
                Vector2Int position = new Vector2Int((int) gridPositionsPosUI.GetValue1(), (int) gridPositionsPosUI.GetValue2());
                double rotation = (double) gridPositionsRotUI.GetValue();

                positions[key].Set(key, position, rotation);
            };

            gridPositionsAddUI.onClick += () => {
                string label = gridPositionsLabelUI.GetValue();
                Vector2Int position = new Vector2Int((int) gridPositionsPosUI.GetValue1(), (int) gridPositionsPosUI.GetValue2());
                double rotation = (double) gridPositionsRotUI.GetValue();

                gridPositionsUI.Add(new Position(label, position, rotation));
                positions.Add(label, new Position(label, position, rotation));
            };

            gridPositionsRemoveUI.onClick += () => {
                positions.Remove(gridPositionsUI.GetSelected());
                gridPositionsUI.RemoveSelected();
            };

            pathSolveUI.onClick += () => {
                Vector2Int start;
                start.x = (int) pathPosStartUI.GetValue1();
                start.y = (int) pathPosStartUI.GetValue2();
                Vector2Int end;
                end.x = (int) pathPosEndUI.GetValue1();
                end.y = (int) pathPosEndUI.GetValue2();

                pathfinding = new Pathfinding.Pathfinding(grid);
                pathfinding.Solve(start, end);
                RenderPath();
            };

            // gridResolutionUpdate.onClick += () => {
            //     // grid.SetResolution(gridResolutionX.GetIntegerValue(), gridResolutionY.GetIntegerValue());
            //     // grid.SetCmPerNode(1646.0 / grid.resolutionX / 2.0);
            //     // cmPerNode.input.Text = $"{grid.unitPerNode/PathfindingGrid.PixelToCentimeterRatio}";
            //     // RenderGridToGridLayer();
            //     // Console.WriteLine($"ResolutionX({grid.resolutionX}), ResolutionY({grid.resolutionY})");
            // };

            // cmPerNodeResolutionUpdate.onClick += () => {
            //     // double value = cmPerNode.GetDecimalValue();
            //     // if (value == double.NaN) return;
            //     // grid.SetCmPerNode(value);
            //     // RenderGridToGridLayer();
            // };

            // gridPositionUpdate.onClick += () => {
            //     // grid.x = gridX.GetDecimalValue();
            //     // grid.y = gridY.GetDecimalValue();
            //     UpdateGrid();
            // };

            // brushUpdate.onClick += () => {
            //     // brush.size = brushSize.GetIntegerValue();
            //     // brush.value = (byte) brushValue.GetIntegerValue();
            // };

            // positionDefinitionList.onInputChanged += () => {
            //     // string selectedString = positionDefinitionList.GetSelected();
            //     // for (int i = 0; i <= positionDefinitions.Count - 1; i++) {
            //     //     if (positionDefinitions[i].label == selectedString) {
            //     //         positions.SetLabel(selectedString);
            //     //         positions.Set(positionDefinitions[i].position.x, positionDefinitions[i].position.y);
            //     //         positionDefinitionsSelectedIndex = i;
            //     //     }
            //     // }
            // };

            // positionsUpdate.onClick += () => {
            //     // int x = positions.GetIntegerValue1();
            //     // int y = positions.GetIntegerValue2();
            //     // positionDefinitions[positionDefinitionsSelectedIndex].SetPosition(x, y);
            // };

            // solveButton.onClick += () => {
            //     // Vector2Int start;
            //     // start.x = (int)solvePositionStart.GetValue1();
            //     // start.y = (int)solvePositionStart.GetValue2();
            //     // Vector2Int end;
            //     // end.x = (int)solvePositionEnd.GetValue1();
            //     // end.y = (int)solvePositionEnd.GetValue2();

            //     // pathfinding = new Pathfinding.Pathfinding(grid);
            //     // pathfinding.Solve(start, end);
            //     // RenderPathToPathLayer();
            // };

            // Program.onExit += () => {
            //     // Program.data.service["offsetX"] = grid.x;
            //     // Program.data.service["offsetY"] = grid.y;

            //     // Program.data.service["resolutionX"] = grid.resolutionX;
            //     // Program.data.service["resolutionY"] = grid.resolutionY;

            //     // Program.data.service["unit"] = grid.unitPerNode / PathfindingGrid.PixelToCentimeterRatio;

            //     // // Program.data.service["positions"] = positionDefinitions;

            //     // Program.data.service.SaveData();
            // };
        }

        public void SetSaveData() {
            grid = new PathfindingGrid(16, 8);
            try {
                grid.x = (double) Program.data.service["offsetX"];
                grid.y = (double) Program.data.service["offsetY"];
            } catch { Console.WriteLine("offset not found."); }

            try {
                int resX = Convert.ToInt32(Program.data.service["resolutionX"]);
                int resY = Convert.ToInt32(Program.data.service["resolutionY"]);
                grid.SetResolution(resX, resY);
            } catch { Console.WriteLine("resolution not found."); }

            try { grid.SetUnitSize((double) Program.data.service["unit"]); } catch { Console.WriteLine("unit not found."); }

            try {
                positions = ((JArray) Program.data.service["positions"]).ToObject<Dictionary<string, Position>>();
            } catch { Console.WriteLine($"positions not found."); }
        }

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

        DoubleNumericPropertyUI gridResUI;
        ButtonPropertyUI gridResUpdateUI;

        DoubleNumericPropertyUI gridPosUI;
        ButtonPropertyUI gridPosUpdateUI;

        NumericPropertyUI gridUnitUI;
        ButtonPropertyUI gridUnitUpdateUI;

        NumericPropertyUI brushSizeUI;
        ButtonPropertyUI brushUpdateUI;

        DropdownPositionsPropertyUI gridPositionsUI;
        TextPropertyUI gridPositionsLabelUI;
        DoubleNumericPropertyUI gridPositionsPosUI;
        NumericPropertyUI gridPositionsRotUI;
        ButtonPropertyUI gridPositionsSetUI;
        ButtonPropertyUI gridPositionsAddUI;
        ButtonPropertyUI gridPositionsRemoveUI;

        DoubleNumericPropertyUI pathPosStartUI;
        DoubleNumericPropertyUI pathPosEndUI;
        ButtonPropertyUI pathSolveUI;

        public void SetupProerties() {
            gridResUI = new DoubleNumericPropertyUI("Resolution", grid.resolutionX, grid.resolutionY);
            gridResUpdateUI = new ButtonPropertyUI("Resolution Update");

            gridPosUI = new DoubleNumericPropertyUI("Position", grid.x, grid.y);
            gridPosUpdateUI = new ButtonPropertyUI("Position Update");

            gridUnitUI = new NumericPropertyUI("Unit", grid.unit);
            gridUnitUpdateUI = new ButtonPropertyUI("Unit Update");

            brushSizeUI = new NumericPropertyUI("Brush Size", 0);
            brushUpdateUI = new ButtonPropertyUI("Brush Update");

            gridPositionsUI = new DropdownPositionsPropertyUI("Positions");
            gridPositionsLabelUI = new TextPropertyUI("Position Label", "New Position");
            gridPositionsPosUI = new DoubleNumericPropertyUI("Position Coords", 0, 0);
            gridPositionsRotUI = new NumericPropertyUI("Position Rotation", 0);
            gridPositionsSetUI = new ButtonPropertyUI("Positions Set");
            gridPositionsAddUI = new ButtonPropertyUI("Positions Add");
            gridPositionsRemoveUI = new ButtonPropertyUI("Positions Remove");

            pathPosStartUI = new DoubleNumericPropertyUI("Path Start", 0, 0);
            pathPosEndUI = new DoubleNumericPropertyUI("Path End", 0, 0);
            pathSolveUI = new ButtonPropertyUI("Solve Path");

            propertyWindowUI.AddProperty(gridResUI);
            propertyWindowUI.AddProperty(gridResUpdateUI);
            propertyWindowUI.AddProperty(new SpacePropertyUI(20));
            propertyWindowUI.AddProperty(gridPosUI);
            propertyWindowUI.AddProperty(gridPosUpdateUI);
            propertyWindowUI.AddProperty(new SpacePropertyUI(20));
            propertyWindowUI.AddProperty(gridUnitUI);
            propertyWindowUI.AddProperty(gridUnitUpdateUI);
            propertyWindowUI.AddProperty(new SpacePropertyUI(20));
            propertyWindowUI.AddProperty(brushSizeUI);
            propertyWindowUI.AddProperty(brushUpdateUI);
            propertyWindowUI.AddProperty(new SpacePropertyUI(20));
            propertyWindowUI.AddProperty(gridPositionsUI);
            propertyWindowUI.AddProperty(gridPositionsLabelUI);
            propertyWindowUI.AddProperty(gridPositionsPosUI);
            propertyWindowUI.AddProperty(gridPositionsRotUI);
            propertyWindowUI.AddProperty(gridPositionsSetUI);
            propertyWindowUI.AddProperty(gridPositionsAddUI);
            propertyWindowUI.AddProperty(gridPositionsRemoveUI);
            propertyWindowUI.AddProperty(new SpacePropertyUI(40));
            propertyWindowUI.AddProperty(pathPosStartUI);
            propertyWindowUI.AddProperty(pathPosEndUI);
            propertyWindowUI.AddProperty(pathSolveUI);
        }

        public void RenderGrid() {
            gridLayer.canvasImage.Width = grid.width;
            gridLayer.canvasImage.Height = grid.height;
            gridLayer.image = new Image<Rgba32>(grid.resolutionX, grid.resolutionY);
            for (int x = 0; x <= grid.resolutionX - 1; x++) {
                for (int y = 0; y <= grid.resolutionY - 1; y++) {
                    if (grid.GetPixel(x, y) == true) gridLayer.image[x, y] = new Rgba32(100, 250, 100, 255);
                    else gridLayer.image[x, y] = new Rgba32(0, 0, 0, 0);
                }
            }
            gridLayer.UpdateImage();
        }

        public void RenderPath() {
            pathLayer.canvasImage.Width = grid.width;
            pathLayer.canvasImage.Height = grid.height;
            pathLayer.image = new Image<Rgba32>(grid.resolutionX, grid.resolutionY);
            foreach (Vector2Int pixel in pathfinding.path) {
                pathLayer.image[pixel.x, pixel.y] = new Rgba32(60, 60, 255);
            }
            pathLayer.UpdateImage();
        }

        public void OnMouseDown(object sender, MouseEventArgs e) {
            UpdateBrush(sender, e);
        }

        public void OnMouseMove(object sender, MouseEventArgs e) {
            UpdateBrush(sender, e);
            UpdatePosition();
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e) {
            mouseWheelDelta = e.Delta;
            UpdatePosition();
        }

        public void OnMouseUp(object sender, MouseEventArgs e) {

        }

        public void UpdateBrush(object sender, MouseEventArgs e) {
            // Point position = (Point) ((e.GetPosition(canvasWindowUI.canvas) - gridLayer.GetPosition()) * (1 / grid.unitPerNode * (1 / zoom)));
            double unitM = 1 / grid.unitPerNode;
            double zoomM = 1 / zoom;
            Point position = (Point) ((e.GetPosition(canvasWindowUI.canvas) - gridLayer.GetPosition()) * (zoomM) / grid.unitPerNode);

            position.X = (int) (position.X);
            position.Y = (int) (position.Y);

            if (e.LeftButton == MouseButtonState.Pressed) {
                grid.FillPixels(brush.GetPixels(new Vector2Int((int) position.X, (int) position.Y)), true);
            } else if (e.RightButton == MouseButtonState.Pressed) {
                grid.FillPixels(brush.GetPixels(new Vector2Int((int) position.X, (int) position.Y)), false);
            } else return;

            RenderGrid();
        }

        public void UpdatePosition() {
            if (Math.Abs(mouseWheelDelta) > 60) {
                zoom = Math.Max(Math.Min(zoom + mouseWheelDelta * zoomSensitivity, maxZoom), minZoom);

                fieldLayer.SetZoom(zoom);
                gridLayer.canvasImage.RenderTransform = fieldLayer.canvasImage.RenderTransform;
                pathLayer.canvasImage.RenderTransform = fieldLayer.canvasImage.RenderTransform;

                mouseWheelDelta = 0;
            }

            Point mousePositionCurrent = Mouse.GetPosition(canvasWindowUI.canvas);

            // if(Math.Abs((mousePositionCurrent.X - mousePositionPrevious.X) + (mousePositionCurrent.Y - mousePositionPrevious.Y)) > 80) return;
            if (Mouse.MiddleButton == MouseButtonState.Pressed) {
                mouseMoveDelta = (Point) (mousePositionCurrent - mousePositionPrevious);
            } else mouseMoveDelta = new Point(0, 0);

            mousePositionPrevious = mousePositionCurrent;

            // if (Mouse.MiddleButton == MouseButtonState.Pressed) {
                fieldLayer.SetPosition(fieldLayer.GetPosition() + (System.Windows.Vector) mouseMoveDelta);
                Point position = fieldLayer.GetPosition();
                gridLayer.SetPosition(grid.x * zoom + position.X, grid.y * zoom + position.Y);
                pathLayer.SetPosition(grid.x * zoom + position.X, grid.y * zoom + position.Y);
            // }
        }

        // TODO: Add Comments 
        public void AddToLayout(UIElement element, int colunmNumber) {
            layout.Children.Add(element);
            Grid.SetColumn(element, colunmNumber);
        }

        // TODO: Add Comments

    }
}