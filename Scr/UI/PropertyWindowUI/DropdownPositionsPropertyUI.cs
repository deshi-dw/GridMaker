using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RoboticsTools.Pathfinding;

namespace RoboticsTools.UI
{
    public class DropdownPositionsPropertyUI : PropertyUI {
        // TODO: Add comments.

        // FIXME: Add Plus/Minus Button.
        // FIXME: Add Saving data.
        public ComboBox input;

        public Dictionary<string, Position> values;

        public delegate void InputChangeEvent();
        public event InputChangeEvent onInputChanged;

        public DropdownPositionsPropertyUI(string label) {
            SetLabel(label);
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

        public string GetSelected() {
            return (string)((ComboBoxItem)input.Items[input.SelectedIndex]).Content;
        }

        private void SelectedChanged(object sender, SelectionChangedEventArgs e) {
            if(onInputChanged != null) onInputChanged();
        }

        public void Add(Position position) {
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

                Content = position.label
            });
        }

        public void RemoveSelected() {
            input.Items.RemoveAt(input.SelectedIndex);
        }
    }
}
