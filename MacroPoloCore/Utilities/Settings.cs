using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MacroPoloCore.Utilities
{
    public class Settings
    {
        public string macrosPath;
        public string specialPath;
        public bool useOneBuffer;
        public int macrosPerPage = 1;
        public List<string> blacklist;
        public string codeBlock;
        public string openBlock = "${";
        public string closeBlock = "}";
    }
}
