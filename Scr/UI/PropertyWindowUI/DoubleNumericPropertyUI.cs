using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class DoubleNumericPropertyUI : PropertyUI {
        // TODO: Add comments.
        public TextBox input1;
        public TextBox input2;

        public bool allowDecimal { get; private set; }

        private Regex regex = new Regex("[^0-9]+");

        public DoubleNumericPropertyUI(string label, double value1, double value2) {
            SetLabel(label);

            input1 = new TextBox() {
                Height = double.NaN,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                MaxLength = 7,
                Text = $"{value1}",

                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.DarkGray
            };

            input2 = new TextBox() {
                Height = double.NaN,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                MaxLength = 7,
                Text = $"{value2}",

                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.DarkGray
            };

            container.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            container.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            Grid.SetColumn(input1, 1);
            container.Children.Add(input1);
            Grid.SetColumn(input2, 2);
            container.Children.Add(input2);
        }

        public void Set(decimal value1, decimal value2) {
            input1.Text = $"{value1}";
            input2.Text = $"{value2}";
        }

        public decimal GetValue1() {
            return Convert.ToDecimal(input1.Text);
        }

        public decimal GetValue2() {
            return Convert.ToDecimal(input2.Text);
        }

        public void SetAllowDecimal(bool shouldAllow) {
            allowDecimal = shouldAllow;
            regex = shouldAllow ? new Regex("[^0-9.]+") : new Regex("[^0-9]+");
        }

        public void SetCharacterLimit(int length) {
            input1.MaxLength = length;
            input2.MaxLength = length;
        }
    }
}
