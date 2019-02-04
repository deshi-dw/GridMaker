using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class SpacePropertyUI : PropertyUI {
        public SpacePropertyUI(double space) {
            SetLabel("");
            container.Height = space;
        }

        public void SetSpace(double space) {
            container.Height = space;
        }
    }
}
