using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class NumericPropertyUI : PropertyUI {
        public TextBox input;

        public bool allowDecimal { get; private set; }

        private Regex regex = new Regex("[^0-9]+");

        public delegate void InputChangeEvent();
        public event InputChangeEvent onInputChanged;

        public NumericPropertyUI(double value) {
            input = new TextBox() {
                Height = double.NaN,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                MaxLength = 7,
                Text = $"{value}",

                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.DarkGray
            };
            input.PreviewTextInput += new TextCompositionEventHandler(InputChange);

            Grid.SetColumn(input, 1);
            container.Children.Add(input);
        }

        private void InputChange(object sender, TextCompositionEventArgs e) {
            e.Handled = regex.IsMatch(e.Text);
            // Console.WriteLine($"Handeled? = {regex.IsMatch(e.Text)}");
            if(onInputChanged != null) onInputChanged();
        }

        public void SetAllowDecimal(bool shouldAllow) {
            allowDecimal = shouldAllow;
            regex = shouldAllow ? new Regex("[^0-9.]+") : new Regex("[^0-9]+");
        }

        public void SetCharacterLimit(int length) {
            input.MaxLength = length;
        }

        public int GetIntegerValue() {
            return Convert.ToInt32(input.Text);
        }

        public double GetDecimalValue() {
            try {
                return Convert.ToDouble(input.Text);
            }
            catch {
                return double.NaN;
            }
        }
    }
}
