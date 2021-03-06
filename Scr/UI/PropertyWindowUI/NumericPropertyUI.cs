﻿using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class NumericPropertyUI : PropertyUI {
        // TODO: Add comments.
        public TextBox input;

        public bool allowDecimal { get; private set; }

        private Regex regex = new Regex("[^0-9]+");

        public NumericPropertyUI(string label, double value) {
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
                MaxLength = 7,
                Text = $"{value}",

                BorderBrush = Brushes.Transparent,
                Foreground = Brushes.DarkGray
            };

            Grid.SetColumn(input, 1);
            container.Children.Add(input);
        }

        public void SetValue(decimal value) {
            input.Text = $"{value}";
        }

        public decimal GetValue() {
            return Convert.ToDecimal(input.Text);
        }

        public void SetAllowDecimal(bool shouldAllow) {
            allowDecimal = shouldAllow;
            regex = shouldAllow ? new Regex("[^0-9.]+") : new Regex("[^0-9]+");
        }

        public void SetCharacterLimit(int length) {
            input.MaxLength = length;
        }
    }
}
