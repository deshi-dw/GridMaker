using System;
using System.Collections.Generic;

namespace RoboticsTools {
    public interface IDataObject {
        string path { get; set; }
        Dictionary<string, dynamic> data { get; set; }

        object this[string key] { get; set; }

        void SaveData();
        void LoadData();
        void ClearData();
    }
}