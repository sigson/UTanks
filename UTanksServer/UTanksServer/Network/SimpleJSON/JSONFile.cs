using System;
using System.IO;

namespace SimpleJSON
{
    public class JSONFile
    {
        public string path { get; private set; } = string.Empty;
        public JSONNode data = new JSONObject();
        public JSONNode this[string index]
        {
            get => data[index];
            set => data[index] = value;
        }
        
        public JSONFile(string path, bool force = false) =>
            this.path = force ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

        public JSONFile Load()
        {
            try { data = JSONNode.Parse(File.ReadAllText(path)); }
            catch { data = new JSONObject(); }
            return this;
        }

        public void Save(bool formatted = false) =>
            File.WriteAllText(path, formatted ? data.ToFormattedString() : data.ToString());
    }
}
