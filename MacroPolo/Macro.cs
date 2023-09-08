﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MacroPolo
{
    public static class Macro
    {
        public static void ProcessArgs(string[] args, Polo polo)
        {
            if (args.Length >= 1 && (args[0].Equals("macros") || args[0].Equals("m")))
            {
                var macros = Macros;
                if (macros == null) Console.WriteLine("Error: unable to parse mappings");
                else
                {
                    if (macros.Count == 0) Console.WriteLine("<empty>");
                    else if (args.Length == 1) Search.Run(macros);
                    else
                    {
                        var search = string.Join(" ", args.Skip(1));
                        Search.Run(macros, search);
                    }
                }
            }
            else if (args.Length == 1 && args[0].Equals("buffers"))
            {
                foreach (var names in polo.GetBufferNames())
                    Console.WriteLine(" \u2022 " + names);
            }
            else if (args.Length == 1 && args[0].Equals("blacklist-current"))
            {
                var name = Window.GetActiveProcess()?.ProcessName;
                var settings = Settings;
                if (name != null && !settings.blacklist.Contains(name))
                {
                    settings.blacklist.Add(name);
                    polo.Clear();
                    Settings.SaveSettings(SettingsFilePath, settings);
                }
            }
            else if (args.Length == 1 && args[0].Equals("blacklist"))
            {
                if (Settings.blacklist.Count == 0) Console.WriteLine("<empty>");
                foreach (var process in Settings.blacklist)
                    Console.WriteLine(" \u2022 " + process);
            }
            else if (args.Length == 1 && (args[0].Equals("reload") || args[0].Equals("r")))
            {
                var awake = Polo.awake;
                Polo.awake = false;
                ReloadSettings();
                polo.CreateContainer();
                Polo.awake = awake;
                Console.WriteLine("Reloaded settings");
            }
            else if (args.Length == 1 && (args[0].Equals("clear") || args[0].Equals("cls")))
            {
                Console.Clear();
            }
            else if (args.Length == 1 && args[0].Equals("clean"))
            {
                Console.WriteLine("Cleaned up " + polo.Clear() + " buffer(s)");
            }
            else if (args.Length >= 3 && (args[0].Equals("add") || args[0].Equals("a")))
                ProcessAddMacro(args);
            else if (args.Length == 2 && (args[0].Equals("remove") || args[0].Equals("rm")))
            {
                var key = args[1];
                var value = RemoveMacro(key);
                if (value != null) Console.WriteLine(key + " \u2260 " + value);
                else Console.WriteLine("Error: unable to find '" + key + "'");
            }
            else if (args.Length == 1 && (args[0].Equals("stop") || args[0].Equals("-")))
            {
                Polo.awake = false;
                Console.WriteLine("Stopped listening");
            }
            else if (args.Length == 1 && (args[0].Equals("start") || args[0].Equals("+")))
            {
                Polo.awake = true;
                Console.WriteLine("Listening...");
            }
            else if (args.Length == 1 && (args[0].Equals("open") || args[0].Equals("o")))
            {
                System.Diagnostics.Process.Start(SettingsFilePath);
                Console.WriteLine(SettingsFilePath);
            }
            else if (args.Length == 2 && (args[0].Equals("open") || args[0].Equals("o")) && (args[1].Equals("macros") || args[1].Equals("m")))
            {
                System.Diagnostics.Process.Start(MacrosFilePath);
                Console.WriteLine(MacrosFilePath);
            }
            else if (args.Length > 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine(" \u2022 macros (m) [text]            - List all macros sorted by their similarity to [text]");
                Console.WriteLine(" \u2022 open (o)                     - Open the settings JSON file");
                Console.WriteLine(" \u2022 open macros                  - Open the macros JSON file");
                Console.WriteLine(" \u2022 reload (r)                   - Reload the settings JSON file");
                Console.WriteLine(" \u2022 buffers                      - List all the active buffers");
                Console.WriteLine(" \u2022 clean                        - Clean the buffer");
                Console.WriteLine(" \u2022 blacklist-current            - Blacklist the current process's name");
                Console.WriteLine(" \u2022 blacklist                    - List all blacklisted processes");
                Console.WriteLine(" \u2022 add (a) [key] [value]        - Add a new macro (key can only contain alphabetical characters)");
                Console.WriteLine(" \u2022 remove (rm) [key]            - Remove an existing macro");
                Console.WriteLine(" \u2022 start (+)                    - Start listening for macros");
                Console.WriteLine(" \u2022 stop (-)                     - Stop listening for macros");
                Console.WriteLine(" \u2022 clear (cls)                  - Clear the console screen");
                Console.WriteLine(" \u2022 quit (q)                     - Exit the program");
                return;
            }
        }

        static Settings settings;
        public static Settings Settings
        {
            get
            {
                if (settings == null) settings = Settings.Create(SettingsFilePath);
                return settings;
            }
        }
        public static void ReloadSettings() => settings = Settings.Create(SettingsFilePath);


        static readonly string settingsFileName = "settings.json";
        static string SettingsFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFileName);
            }
        }

        static string MacrosFilePath
        {
            get
            {
                var macrosPath = Settings.macrosPath;
                if (!Path.IsPathRooted(macrosPath))
                    macrosPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, macrosPath);
                if (!File.Exists(macrosPath))
                    File.WriteAllText(macrosPath, "{}");
                return macrosPath;
            }
        }

        static Dictionary<string, string> GetDictionary(string filePath) => GetDictionary<string>(filePath);

        enum SpecialMacro
        {
            IgnoreCase, // ignore the key's case
            FirstCase, // copy the key's first letter's case in the value's first letter
            Pluralize // creates two macros for "{key}" and "{key}s" - remember to remove both
        }

        static Dictionary<string, T> GetDictionary<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
        }

        static void SetDictionary(string filePath, Dictionary<string, string> dictionary)
        {
            string json = JsonConvert.SerializeObject(dictionary);
            File.WriteAllText(filePath, json);
        }

        private static Dictionary<string, string> macros;
        public static Dictionary<string, string> Macros
        {
            get
            {
                if (macros == null) macros = GetDictionary(MacrosFilePath);
                return macros;
            }
        }

        static void ProcessAddMacro(string[] args) 
        {
            var key = args[1];
            string value;
            if (args.Length >= 3 && args[2].StartsWith(Settings.openBlock))
            {
                value = args[2].Replace(Settings.openBlock, string.Empty);
                if (args[2].EndsWith(Settings.closeBlock))
                    value = value.Substring(0, value.Length - Settings.closeBlock.Length);
                else
                {
                    value += "\n";
                    bool isFirstLine = true;
                    while (true)
                    {
                        Console.Write("...");
                        string line = Console.ReadLine();
                        if (!isFirstLine) value += "\n";
                        value += line;
                        if (line.Contains(Settings.closeBlock))
                        {
                            value = value.Replace(Settings.closeBlock, string.Empty).Trim();
                            break;
                        }
                        isFirstLine = false;
                    }
                }
            }
            else value = string.Join(" ", args.Skip(2));
            if (AddMacro(key, value)) PrettyPrint(key + " \u2192 " + value);
            else Console.WriteLine("Error: could not add key");
        }

        static bool AddMacro(string key, string value)
        {
            if (!Regex.IsMatch(key, @"^[a-zA-Z]+$")) return false;
            Macros[key] = value;
            SetDictionary(MacrosFilePath, Macros);
            return true;
        }

        public static string RemoveMacro(string key)
        {
            if (Macros.ContainsKey(key))
            {
                var value = Macros[key];
                Macros.Remove(key);
                SetDictionary(MacrosFilePath, Macros);
                return value;
            }
            else return null;
        }

        public static void PrettyPrint(string input)
        {
            string trimmedInput = input.Trim();
            int newlineIndex = trimmedInput.IndexOf('\n');
            if (newlineIndex != -1)
            {
                string subString = trimmedInput.Substring(0, newlineIndex);
                subString += "...";
                Console.WriteLine(subString);
            }
            else Console.WriteLine(trimmedInput);
        }
    }
}
