using System;
using System.Collections.Generic;

namespace RoboticsTools {
    public interface IDataObject {
        string Path { get; set; }
        Dictionary<string, object> Data { get; set; }

        object this[string key] { get; set; }

        void SaveData();
        void LoadData();
        void ClearData();
    }
}