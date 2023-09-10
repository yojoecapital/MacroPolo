using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MacroPoloCore.Utilities
{
    public class Settings
    {
        public string macrosPath;
        public bool useOneBuffer;
        public int macrosPerPage = 1;
        public List<string> blacklist;
        public string codeBlock;
        public string openBlock = "${";
        public string closeBlock = "}";

        public static Settings Create(string path)
        {
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            
            return settings;
        }

        public static void SaveSettings(string path, Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
