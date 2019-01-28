using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class ButtonPropertyUI : PropertyUI {
        public Button input;

        public bool allowDecimal { get; private set; }

        private Regex regex = new Regex("[^0-9]+");

        public delegate void InputChangeEvent();
        public event InputChangeEvent onClick;

        public ButtonPropertyUI() {
            input = new Button() {
                Height = 20,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(20.0, 0.0, 20.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                Content = "",

                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.Black,
                Background = Brushes.DarkGray
            };
            input.Click += new RoutedEventHandler(OnClick);

            Grid.SetColumnSpan(input, 2);
            container.Children.Add(input);

            SetLabel("");
        }

        public void SetButtonText(string text) {
            input.Content = text;
        }

        private void OnClick(object sender, RoutedEventArgs e) {
            if(onClick != null) onClick();
        }
    }
}
