using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace RoboticsTools.UI {
    public class ColourPalette {
        // Backgrounds //
        public Color windowBackground;
        public Color pannelBackground;
        public Color inputBackground;
        
        // Foregrounds //
        public Color textForeground;
        public Color inputForeground;

        // Highlights //
        public Color textHighlight;
        public Color inputHighlight;

        public ColourPalette() {
            // Backgrounds //
            windowBackground = new Color() {
                R = 30,
                G = 30,
                B = 36
            };
            pannelBackground = new Color() {
                R = 40,
                G = 40,
                B = 44
            };
            inputBackground = new Color() {
                R = 35,
                G = 35,
                B = 39
            };

            // Foregrounds //
            textForeground = new Color() {
                R = 210,
                G = 210,
                B = 210
            };
            inputForeground = new Color() {
                R = 50,
                G = 50,
                B = 53
            };

            // Highlights //
            textHighlight = new Color() {
                R = 250,
                G = 250,
                B = 210
            };
            inputHighlight = new Color() {
                R = 60,
                G = 60,
                B = 62
            };
        }
    }
}