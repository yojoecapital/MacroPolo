using CliFramework;
using MacroPoloCore.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MacroPoloCore.Managers
{
    internal class MacroPoloFileManager : FileManager
    {
        public static string SettingsFilePath
        {
            get => GetDictionaryFilePath("settings.json");
        }

        private Settings settings;
        public Settings Settings
        {
            get
            {
                settings ??= GetObject<Settings>(SettingsFilePath);
                if (settings == null)
                {
                    PrettyConsole.PrintError("Could not parse settings file.");
                    return null;
                }
                else
                {
                    settings.blacklist ??= new List<string>();
                    if (!string.IsNullOrEmpty(settings.codeBlock) && settings.codeBlock.Length >= 2)
                    {
                        settings.openBlock = settings.codeBlock.Substring(0, settings.codeBlock.Length - 1);
                        settings.closeBlock = settings.codeBlock[settings.codeBlock.Length - 1].ToString();
                    }
                    return settings;
                }
            }
            set 
            {
                settings = value;
                SetObject(SettingsFilePath, settings);
            }
        }

        public string MacrosFilePath
        {
            get {
                var settings = Settings;
                if (settings != null && !string.IsNullOrEmpty(settings.macrosPath))
                    return GetDictionaryFilePath(settings.macrosPath) ?? GetDictionaryFilePath("macros.json");
                else return GetDictionaryFilePath("macros.json");
            }
        }

        private Dictionary<string, string> macros;
        public Dictionary<string, string> Macros
        {
            get
            {
                macros ??= GetDictionary(MacrosFilePath);
                if (macros == null) PrettyConsole.PrintError("Could not parse macros file.");
                return macros;
            }
            private set
            {
                macros = value;
                SetDictionary(MacrosFilePath, macros);
            }
        }

        public void Reload()
        {
            settings = null;
            macros = null;
        }

        public bool AddMacro(string key, string value)
        {
            var macros = Macros;
            if (macros == null || !Regex.IsMatch(key, @"^[a-zA-Z]+$"))
                return false;
            else
            {
                macros[key] = value;
                Macros = macros;
                return true;
            }
        }

        public string RemoveMacro(string key)
        {
            var macros = Macros;
            if (macros != null && macros.ContainsKey(key))
            {
                var value = macros[key];
                macros.Remove(key);
                Macros = macros;
                return value;
            }
            else return null;
        }
    }
}
