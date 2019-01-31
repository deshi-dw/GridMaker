using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class DropdownValuesPropertyUI : PropertyUI {
        public ComboBox input;

        public bool allowDecimal { get; private set; }

        private Regex regex = new Regex("[^0-9]+");

        public delegate void InputChangeEvent();
        public event InputChangeEvent onInputChanged;

        public DropdownValuesPropertyUI() {
            input = new ComboBox() {
                Height = double.NaN,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),

                Background = Brushes.Gray,
                BorderBrush = Brushes.Transparent
            };
            input.SelectionChanged += new SelectionChangedEventHandler(SelectedChanged);

            Grid.SetColumn(input, 1);
            container.Children.Add(input);
        }

        private void SelectedChanged(object sender, SelectionChangedEventArgs e) {
            // Console.WriteLine(e.AddedItems[0]);

            if(onInputChanged != null) onInputChanged();
        }

        public void Add(string label) {
            input.Items.Add(new ComboBoxItem() {
                Height = double.NaN,
                Width = double.NaN,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0.0, 0.0, 0.0, 0.0),
                Margin = new Thickness(0.0, 0.0, 0.0, 0.0),
                Foreground = Brushes.DarkGray,

                Content = label
            });
        }

        public string GetSelected() {
            return (string)((ComboBoxItem)input.Items[input.SelectedIndex]).Content;
        }
    }
}
