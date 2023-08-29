using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MacroPolo
{
    public class Settings
    {
        public string macrosPath;
        public bool useOneBuffer;
        public List<string> blacklist;

        public static Settings Create(string path)
        {
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            if (settings.blacklist == null) settings.blacklist = new List<string>();
            return settings;
        }

        public static void SaveSettings(string path, Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
