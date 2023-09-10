using CliFramework;
using MacroPoloCore.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    PrettyConsole.PrintError("Could not parse settings JSON file.");
                    return null;
                }
                else
                {
                    settings.blacklist ??= new List<string>();
                    if (!string.IsNullOrEmpty(settings.codeBlock) && settings.codeBlock.Length >= 8 && settings.codeBlock.Contains("[text]"))
                        (settings.openBlock, settings.closeBlock) = settings.codeBlock.SplitStringFromFirstInstance("[text]");
                    return settings;
                }
            }
            set 
            {
                settings = value;
                SetObject(SettingsFilePath, settings, Formatting.Indented);
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
                if (macros == null) PrettyConsole.PrintError("Could not parse macros JSON file.");
                return macros;
            }
            private set
            {
                macros = value;
                SetDictionary(MacrosFilePath, macros);
            }
        }

        public string SpecialFilePath
        {
            get
            {
                var settings = Settings;
                if (settings != null && !string.IsNullOrEmpty(settings.specialPath))
                    return GetDictionaryFilePath(settings.specialPath) ?? GetDictionaryFilePath("special.json");
                else return GetDictionaryFilePath("special.json");
            }
        }

        private Dictionary<string, SpecialMacro> specialMacros;
        public Dictionary<string, SpecialMacro> SpecialMacros
        {
            get
            {
                specialMacros ??= GetDictionary<SpecialMacro>(SpecialFilePath);
                if (specialMacros == null) PrettyConsole.PrintError("Could not parse special JSON file.");
                return specialMacros;
            }
            private set
            {
                specialMacros = value;
                SetDictionary(SpecialFilePath, specialMacros);
            }
        }

        public void Reload()
        {
            settings = null;
            macros = null;
            specialMacros = null;
        }

        public bool AddMacro(string key, string value) =>
            AddMacros((key, value));

        public bool AddMacros(params (string, string)[] input)
        {
            var macros = Macros;
            if (macros != null)
            {
                foreach (var tuple in input)
                {
                    var key = tuple.Item1;
                    if (!Regex.IsMatch(key, @"^[a-zA-Z]+$"))
                        return false;
                    else
                    {
                        macros[key] = tuple.Item2;
                    }
                }
                Macros = macros;
                return true;
            }
            else return false;
        }

        public bool AddSpecialMacro(string key, string value, SpecialMacroType type) => 
            AddSpecialMacros((key, value, type));

        public bool AddSpecialMacros(params (string, string, SpecialMacroType)[] input)
        {
            var specialMacros = SpecialMacros;
            if (specialMacros != null)
            {
                foreach (var tuple in input)
                {
                    var key = tuple.Item1.ToLower();
                    if (!Regex.IsMatch(key, @"^[a-z]+$"))
                        return false;
                    else
                    {
                        specialMacros[key] = new SpecialMacro()
                        {
                            value = tuple.Item2,
                            type = tuple.Item3
                        };
                    }
                }
                SpecialMacros = specialMacros;
                return true;
            }
            else return false;
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

        public string RemoveMacros(params string[] keys)
        {
            var macros = Macros;
            string value = null;
            if (macros != null)
            {
                foreach (var key in keys)
                {
                    if (macros.ContainsKey(key))
                    {
                        value = macros[key];
                        macros.Remove(key);
                    } 
                    else return null;
                }
                Macros = macros;
                return value;
            }
            else return null;
        }

        public SpecialMacro RemoveSpecialMacro(string key)
        {
            var specialMacros = SpecialMacros;
            if (specialMacros != null && specialMacros.ContainsKey(key))
            {
                var value = specialMacros[key];
                specialMacros.Remove(key);
                SpecialMacros = specialMacros;
                return value;
            }
            else return null;
        }

        public SpecialMacro RemoveSpecialMacros(params string[] keys)
        {
            var specialMacros = SpecialMacros;
            SpecialMacro value = null;
            if (specialMacros != null)
            {
                foreach (var key in keys)
                {
                    if (specialMacros.ContainsKey(key))
                    {
                        value = specialMacros[key];
                        specialMacros.Remove(key);
                    }
                    else return null;
                }
                SpecialMacros = specialMacros;
                return value;
            }
            else return null;
        }
    }
}
