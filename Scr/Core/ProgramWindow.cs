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
        public PathfindingGrid grid;

        // FIXME: This is horrible. There is next to no need for TWO classes with the same function.
        public CanvasLayer2 gridLayer;
        public CanvasLayer2 pathLayer;
        public CanvasLayer fieldLayer;

        public CanvasBrushSquare brush;

        public List<PositionDefinition> positionDefinitions;
        int positionDefinitionsSelectedIndex = 0;

        Pathfinding.Pathfinding pathfinding;

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
            positionDefinitions = new List<PositionDefinition>();

            // foreach(KeyValuePair<string, object> value in Program.data.service.Data) Console.WriteLine($"Key: {value.Key} | Value: {value.Value}");

            try {
                grid.x = (double)Program.data.service["offsetX"];
                grid.y = (double)Program.data.service["offsetY"];
            } catch { Console.WriteLine("offset not found."); }

            try {
                int resX = Convert.ToInt32(Program.data.service["resolutionX"]);
                int resY = Convert.ToInt32(Program.data.service["resolutionY"]);
                grid.SetResolution(resX, resY);
            } catch { Console.WriteLine("resolution not found."); }

            try { grid.SetCmPerNode((double)Program.data.service["unit"]); } catch { Console.WriteLine("unit not found."); }

            // try { positionDefinitions = (List<PositionDefinition>)Program.data.service["positions"]; } catch { Console.WriteLine("positions not found."); }
            // positionDefinitions = (List<PositionDefinition>)Program.data.service["positions"];
            
            positionDefinitions = ((JArray)Program.data.service["positions"]).ToObject<List<PositionDefinition>>();
            // try {
                    // dynamic s = Program.data.service["positions"];
                    // Console.WriteLine(s);
            //     } catch { Console.WriteLine($"positions not found. {positionDefinitions[0].label}"); }

            // Console.WriteLine($"unit = {grid.unitPerNode}");
            brush.value = 255;

            gridLayer = new CanvasLayer2(new Image<Alpha8>(grid.resolutionX, grid.resolutionY), canvasWindowUI.canvas);
            pathLayer = new CanvasLayer2(new Image<Alpha8>(grid.resolutionX, grid.resolutionY), canvasWindowUI.canvas);
            pathLayer.color = new Rgb24(255, 255, 0);
            Canvas.SetZIndex(gridLayer.canvasImage, 10);
            Canvas.SetZIndex(pathLayer.canvasImage, 11);
            fieldLayer = new CanvasLayer(SixLabors.ImageSharp.Image.Load<Rgb24>($"{Program.path}\\data\\2019-field.png"), canvasWindowUI.canvas);
            // FIXME: The Window class you are inherenting has all these control events you idiot.
            canvasWindowUI.onRenderUpdate += UpdateGrid;

            this.MouseMove += new MouseEventHandler(UpdateBrush);
            this.MouseDown += new MouseButtonEventHandler(UpdateBrush);

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

            brushUpdate.onClick += () => {
                brush.size = brushSize.GetIntegerValue();
                brush.value = (byte)brushValue.GetIntegerValue();
            };

            positionDefinitionList.onInputChanged += () => {
                string selectedString = positionDefinitionList.GetSelected();
                for(int i = 0; i <= positionDefinitions.Count-1; i++) {
                    if(positionDefinitions[i].label == selectedString) {
                        positions.SetLabel(selectedString);
                        positions.Set(positionDefinitions[i].position.x, positionDefinitions[i].position.y);
                        positionDefinitionsSelectedIndex = i;
                    }
                }
            };

            positionsUpdate.onClick += () => {
                int x = positions.GetIntegerValue1();
                int y = positions.GetIntegerValue2();
                positionDefinitions[positionDefinitionsSelectedIndex].SetPosition(x, y);
            };

            solveButton.onClick += () => {
                Vector2Int start;
                start.x = solvePositionStart.GetIntegerValue1();
                start.y = solvePositionStart.GetIntegerValue2();
                Vector2Int end;
                end.x = solvePositionEnd.GetIntegerValue1();
                end.y = solvePositionEnd.GetIntegerValue2();
                pathfinding = new Pathfinding.Pathfinding(grid);
                pathfinding.Solve(start, end);
                RenderPathToPathLayer();
            };
            
            Program.onExit += () => {
                Program.data.service["offsetX"] = grid.x;
                Program.data.service["offsetY"] = grid.y;
                
                Program.data.service["resolutionX"] = grid.resolutionX;
                Program.data.service["resolutionY"] = grid.resolutionY;

                Program.data.service["unit"] = grid.unitPerNode/PathfindingGrid.PixelToCentimeterRatio;

                Program.data.service["positions"] = positionDefinitions;

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

        NumericPropertyUI brushSize;
        NumericPropertyUI brushValue;
        ButtonPropertyUI brushUpdate;

        DropdownValuesPropertyUI positionDefinitionList;
        DoubleNumericPropertyUI positions;
        ButtonPropertyUI positionsUpdate;

        DoubleNumericPropertyUI solvePositionStart;
        DoubleNumericPropertyUI solvePositionEnd;
        ButtonPropertyUI solveButton;


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

            space = new PropertyUI();
            space.container.Height = 40;
            space.SetLabel("");
            propertyWindowUI.AddProperty(space);

            brushSize = new NumericPropertyUI(brush.size);
            brushSize.SetLabel("Brush Size");
            brushValue = new NumericPropertyUI(brush.value);
            brushValue.SetLabel("Brush Value");

            brushUpdate = new ButtonPropertyUI();
            brushUpdate.SetButtonText("Update");

            propertyWindowUI.AddProperty(brushSize);
            propertyWindowUI.AddProperty(brushValue);
            propertyWindowUI.AddProperty(brushUpdate);

            space = new PropertyUI();
            space.container.Height = 40;
            space.SetLabel("");
            propertyWindowUI.AddProperty(space);

            positionDefinitionList = new DropdownValuesPropertyUI();
            positionDefinitionList.SetLabel("Position Definitions");
            
            foreach(PositionDefinition position in positionDefinitions) {
                positionDefinitionList.Add(position.label);
            }
            positionDefinitionList.input.SelectedIndex = 0;

            positions = new DoubleNumericPropertyUI(positionDefinitions[0].position.x, positionDefinitions[0].position.y);
            positions.SetLabel(positionDefinitions[0].label);

            positionsUpdate = new ButtonPropertyUI();
            positionsUpdate.SetButtonText("Update");

            propertyWindowUI.AddProperty(positionDefinitionList);
            propertyWindowUI.AddProperty(positions);
            propertyWindowUI.AddProperty(positionsUpdate);

            space = new PropertyUI();
            space.container.Height = 60;
            space.SetLabel("");
            propertyWindowUI.AddProperty(space);

            solvePositionStart = new DoubleNumericPropertyUI(0, 0);
            solvePositionStart.SetLabel("Solve Start");
            solvePositionEnd = new DoubleNumericPropertyUI(25, 25);
            solvePositionEnd.SetLabel("Solve End");
            solveButton = new ButtonPropertyUI();
            solveButton.SetButtonText("Solve Path");

            propertyWindowUI.AddProperty(solvePositionStart);
            propertyWindowUI.AddProperty(solvePositionEnd);
            propertyWindowUI.AddProperty(solveButton);
        }

        public void RenderGridToGridLayer() {
            gridLayer.SetSize(grid.width, grid.height);
            gridLayer.image = new Image<Alpha8>(grid.resolutionX, grid.resolutionY);
            for(int x = 0; x <= grid.resolutionX-1; x++) {
                for(int y = 0; y <= grid.resolutionY-1; y++) {
                    gridLayer.image[x, y] = new Alpha8(grid[x, y]/255.0f);
                    // gridLayer.image[x, y] = new Alpha8(255);
                }   
            }
            gridLayer.Update();
            // gridLayer.canvasImage.Fill = Brushes.DeepPink;
        }

        public void RenderPathToPathLayer() {
            pathLayer.SetSize(grid.width, grid.height);
            pathLayer.image = new Image<Alpha8>(grid.resolutionX, grid.resolutionY);
            foreach(Vector2Int pixel in pathfinding.path) {
                pathLayer.image[pixel.x, pixel.y] = new Alpha8(0.5f);
                Console.WriteLine($"Path : X: {pixel.x}   Y: {pixel.y}");
            }
            pathLayer.image[0, 0] = new Alpha8(1.0f);
            pathLayer.image[grid.resolutionX-1, grid.resolutionY-1] = new Alpha8(1.0f);
            pathLayer.Update();
            // gridLayer.canvasImage.Fill = Brushes.DeepPink;
        }

        public void UpdateBrush(object sender, MouseEventArgs e) {
            canvasWindowUI.OnMouseMove(sender, e);
            Point position = (Point)((canvasWindowUI.mousePositionCurrent - gridLayer.GetPosition()) * (1/grid.unitPerNode*(1/canvasWindowUI.zoom)));
            // Console.WriteLine($"Mouse X = {position.X} : Mouse Y = {position.Y}");
            // position.X = Math.Round(position.X, MidpointRounding.AwayFromZero);
            // position.Y = Math.Round(position.Y, MidpointRounding.AwayFromZero);
            position.X = (int)position.X;
            position.Y = (int)position.Y;
            // Console.WriteLine($"Mouse X = {position.X} : Mouse Y = {position.Y}");
            
            // Console.WriteLine($"Mouse X = {position.X} : Mouse Y = {position.Y}          (Zoom = {canvasWindowUI.zoom})");
            // grid[(int)position.X, (int)position.Y] = 255;
            if(e.LeftButton == MouseButtonState.Pressed) {
                grid.FillPixels(brush.GetPixels(new Vector2Int((int)position.X, (int) position.Y)), brush.value);
            }
            else if(e.RightButton == MouseButtonState.Pressed) {
                grid.FillPixels(brush.GetPixels(new Vector2Int((int)position.X, (int) position.Y)), 0);
            }
            else return;
            // TODO: Add dragging
            // TODO: Start using native window events.
            // TODO: Test pathfinding.
            //
            RenderGridToGridLayer();
        }
        
        public void UpdateGrid() {
            if(canvasWindowUI.lastMouseEvent != null) if(canvasWindowUI.lastMouseEvent.MiddleButton == MouseButtonState.Pressed) {
                Point position = fieldLayer.GetPosition();
                fieldLayer.SetPosition(position.X + canvasWindowUI.mouseMoveDelta.X, position.Y + canvasWindowUI.mouseMoveDelta.Y);
                position = fieldLayer.GetPosition();
                gridLayer.SetPosition(grid.x * canvasWindowUI.zoom + position.X, grid.y * canvasWindowUI.zoom + position.Y);
                pathLayer.SetPosition(grid.x * canvasWindowUI.zoom + position.X, grid.y * canvasWindowUI.zoom + position.Y);
                return;
            }
            // gridLayer.SetPosition(position.X + canvasWindowUI.mouseMoveDelta.X,position.Y + canvasWindowUI.mouseMoveDelta.Y);
            fieldLayer.SetZoom(canvasWindowUI.zoom);
            gridLayer.canvasImage.RenderTransform = fieldLayer.canvasImage.RenderTransform;
            pathLayer.canvasImage.RenderTransform = fieldLayer.canvasImage.RenderTransform;
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
