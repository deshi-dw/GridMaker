using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class TextPropertyUI : PropertyUI {
        // TODO: Add comments.
        public TextBox input;

        public TextPropertyUI(string label, string value) {
            SetLabel(label);
            input = new TextBox() {
                Height = double.NaN,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                Text = value,

                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.DarkGray
            };

            Grid.SetColumn(input, 1);
            container.Children.Add(input);
        }

        public void SetValue(string value) {
            input.Text = value;
        }

        public string GetValue() {
            return input.Text;
        }
    }
}
