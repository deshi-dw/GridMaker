using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class PropertyWindowUI : WindowUI {
        private string name;
        public string Name {
            get { return name; }
            set {
                name = value;
                SetName(value);
            }
        }

        public List<PropertyUI> propertyList;
        
        private ScrollViewer stackScrollViewer;
        private StackPanel stackPanel;
        private Label stackNameLabel;

        public PropertyWindowUI(IAddChild parent) : base(parent) {
            propertyList = new List<PropertyUI>();

            layout.RowDefinitions.Add(
                new RowDefinition() { Height = GridLength.Auto }
            );
            layout.RowDefinitions.Add(
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
            );

            stackScrollViewer = new ScrollViewer() {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = Brushes.White
            };

            stackPanel = new StackPanel() {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            stackNameLabel = new Label() {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 24,
                Content = "null",
                Background = Brushes.White
            };

            Panel.SetZIndex(stackScrollViewer, -10);
            Panel.SetZIndex(stackNameLabel, 10);

            Grid.SetRow(stackNameLabel, 0);
            Grid.SetRow(stackScrollViewer, 1);

            stackScrollViewer.Content = stackPanel;
            layout.Children.Add(stackNameLabel);
            layout.Children.Add(stackScrollViewer);
        }

        public void AddProperty(PropertyUI property) {
            propertyList.Add(property);
            stackPanel.Children.Add(property.container);
        }

        private void SetName(string value) {
            stackNameLabel.Content = value;
        }
    }
}