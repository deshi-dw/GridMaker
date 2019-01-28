using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RoboticsTools {
    public static class Program {
        public delegate void ProgramEvents();
        public static event ProgramEvents onExit;

        public static string path;
        public static Data data;

        public static Dispatcher dispatcher;

        private static ProgramWindow window;
        private static Application application;

        [STAThread]
        private static void Main(string[] args) {
            path = $"{Directory.GetCurrentDirectory()}\\bin\\Debug";
            data = new Data(new JSONDataObject($"{path}\\data\\userData.json"));
            data.service.LoadData();

            application = new Application();
            window = new ProgramWindow();

            application.Run(window);

            if(onExit != null) onExit();
        }
    }
}
