using CliFramework;
using MacroPoloCore.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Dictionary<string, string> temporaryMacros = new();
        public Dictionary<string, string> Macros
        {
            get =>PrivateMacros.Union(temporaryMacros).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private Dictionary<string, string> privateMacros;
        private Dictionary<string , string> PrivateMacros
        {
            get
            {
                privateMacros ??= GetDictionary(MacrosFilePath);
                if (privateMacros == null) PrettyConsole.PrintError("Could not parse macros JSON file.");
                return privateMacros;
            }
            set
            {
                privateMacros = value;
                SetDictionary(MacrosFilePath, privateMacros);
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
        private readonly Dictionary<string, SpecialMacro> temporarySpecialMacros = new();
        public Dictionary<string, SpecialMacro> SpecialMacros
        {
            get
            {
                specialMacros ??= GetDictionary<SpecialMacro>(SpecialFilePath);
                if (specialMacros == null) PrettyConsole.PrintError("Could not parse special JSON file.");
                return specialMacros.Union(temporarySpecialMacros).ToDictionary(pair => pair.Key, pair => pair.Value);
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
            privateMacros = null;
            specialMacros = null;
        }

        public bool AddMacro(string key, string value) =>
            AddMacros((key, value));

        public bool AddMacros(params (string, string)[] input)
        {
            var macros = PrivateMacros;
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
                PrivateMacros = macros;
                return true;
            }
            else return false;
        }

        public bool AddTemporaryMacro(string key, string value) =>
            AddTemporaryMacros((key, value));

        public bool AddTemporaryMacros(params (string, string)[] input)
        {
            var macros = temporaryMacros;
            if (macros != null)
            {
                foreach (var tuple in input)
                {
                    var key = tuple.Item1;
                    if (!Regex.IsMatch(key, @"^[a-zA-Z]+$"))
                        return false;
                    else macros[key] = tuple.Item2;
                }
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
            var macros = PrivateMacros;
            if (macros != null && macros.ContainsKey(key))
            {
                var value = macros[key];
                macros.Remove(key);
                PrivateMacros = macros;
                return value;
            }
            else return null;
        }

        public string RemoveTemporaryMacro(string key)
        {
            var macros = temporaryMacros;
            if (macros != null && macros.ContainsKey(key))
            {
                var value = macros[key];
                macros.Remove(key);
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
    }
}
