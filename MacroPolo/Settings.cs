using Newtonsoft.Json;
using System.IO;

namespace MacroPolo
{
    public class Settings
    {
        public string macrosPath;
        public bool useOneBuffer;

        public static Settings Create(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
