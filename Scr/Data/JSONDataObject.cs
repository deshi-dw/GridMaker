using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RoboticsTools {
    public class JSONDataObject : IDataObject {
        private string path;
        private Dictionary<string, object> data;
        public string Path { get => path; set => path = value; }
        public Dictionary<string, object> Data { get => data; set => data = value; }

        public object this[string key] { get {
            if(data.ContainsKey(key)) return data[key];
            data.Add(key, null);
            return null;
        }
        set {
            if(data.ContainsKey(key)) {
                data[key] = value;
                return;
            }

            data.Add(key, value);
        }}

        public JSONDataObject(string path) {
            this.path = path;
            data = new Dictionary<string, object>();
        }

        public void SaveData() {
            string jsonDataString = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText($"{path}", jsonDataString);
        }

        public void LoadData() {
            string jsonDataString = File.ReadAllText($"{path}");
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonDataString);
        }

        public void ClearData() {
            data.Clear();
            // File.WriteAllText($"{path}\\UserData.json", "{}");
        }
    }
}