using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class DoubleNumericPropertyUI : PropertyUI {
        public TextBox input1;
        public TextBox input2;

        public bool allowDecimal { get; private set; }

        private Regex regex = new Regex("[^0-9]+");

        public DoubleNumericPropertyUI(double value1, double value2) {
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
            input1.PreviewTextInput += new TextCompositionEventHandler(InputChange);

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
            input2.PreviewTextInput += new TextCompositionEventHandler(InputChange);

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

        private void InputChange(object sender, TextCompositionEventArgs e) {
            e.Handled = regex.IsMatch(e.Text);
        }

        public void SetAllowDecimal(bool shouldAllow) {
            allowDecimal = shouldAllow;
            regex = shouldAllow ? new Regex("[^0-9.]+") : new Regex("[^0-9]+");
        }

        public void SetCharacterLimit1(int length) {
            input1.MaxLength = length;
        }

        public int GetIntegerValue1() {
            return Convert.ToInt32(input1.Text);
        }

        public double GetDecimalValue1() {
            try { return Convert.ToDouble(input1.Text); }
            catch { return double.NaN; }
        }

        public void SetCharacterLimit2(int length) {
            input2.MaxLength = length;
        }

        public int GetIntegerValue2() {
            return Convert.ToInt32(input2.Text);
        }

        public double GetDecimalValue2() {
            try { return Convert.ToDouble(input2.Text); }
            catch { return double.NaN; }
        }
    }
}
