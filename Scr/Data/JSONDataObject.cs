using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RoboticsTools {
    public struct JSONDataObject : IDataObject {
        public string path { get; set; }
        public Dictionary<string, object> data { get; set; }

        public object this [string key] {
            get {
                if (data.ContainsKey(key)) return data[key];
                data.Add(key, null);
                return null;
            }
            set {
                if (data.ContainsKey(key)) {
                    data[key] = value;
                    return;
                }

                data.Add(key, value);
            }
        }

        public JSONDataObject(string path) {
            this.path = path;
            data = new Dictionary<string, object>();
        }

        public void SaveData() {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText($"{path}", json);
        }

        public void LoadData() {
            string json = File.ReadAllText($"{path}");
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json,
                new JsonSerializerSettings() {
                    CheckAdditionalContent = true
                });
        }

        public void ClearData() {
            data.Clear();
            // File.WriteAllText($"{path}\\UserData.json", "{}");
        }
    }
}