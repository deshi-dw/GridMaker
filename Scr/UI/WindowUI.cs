using System;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RoboticsTools.UI {
    public class WindowUI {
        public Grid layout;

        public WindowUI(IAddChild parent) {
            layout = new Grid() {
                Width = double.NaN,
                Height = double.NaN,
                Focusable = false
            };

            parent.AddChild(layout);
        }
    }
}