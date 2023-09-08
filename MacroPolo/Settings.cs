using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MacroPolo
{
    public class Settings
    {
        public string macrosPath;
        public bool useOneBuffer;
        public int macrosPerPage;
        public List<string> blacklist;
        public string codeBlock;
        public string openBlock = "${";
        public string closeBlock = "}";

        public static Settings Create(string path)
        {
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            if (settings.blacklist == null) settings.blacklist = new List<string>();
            if (!string.IsNullOrEmpty(settings.codeBlock) && settings.codeBlock.Length >= 2)
            {
                settings.openBlock = settings.codeBlock.Substring(0, settings.codeBlock.Length - 1);
                settings.closeBlock = settings.codeBlock[settings.codeBlock.Length - 1].ToString();
            }
            return settings;
        }

        public static void SaveSettings(string path, Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
