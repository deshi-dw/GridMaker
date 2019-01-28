using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RoboticsTools.UI {
    public class PropertyUI {
        public Grid container;
        private Label label;

        public PropertyUI() {
            label = new Label() {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,

                Content = "null"
            };

            container = new Grid() {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            container.ColumnDefinitions.Add( new ColumnDefinition() {
                    Width = new GridLength(2.5, GridUnitType.Star)
            });
            container.ColumnDefinitions.Add( new ColumnDefinition() {
                    Width = new GridLength(1, GridUnitType.Star)
            });
            Grid.SetColumn(label, 0);

            container.Children.Add(label);
        }
        public void SetLabel(string text) {
            label.Content = text;
        }
    }
}