using Newtonsoft.Json;
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
            if (args.Length == 1 && (args[0].Equals("macros") || args[0].Equals("m")))
            {
                var macros = Macros;
                if (macros == null) Console.WriteLine("Error: unable to parse mappings");
                else
                {
                    if (macros.Count == 0) Console.WriteLine("<empty>");
                    foreach (var key in macros.Keys)
                        Console.WriteLine(key + " \u2192 " + macros[key]);
                }
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
            {
                var key = args[1];
                var value = string.Join(" ", args.Skip(2));
                if (AddMacro(key, value))
                    Console.WriteLine(key + " \u2192 " + value);
                else Console.WriteLine("Error: could not add key");
            }
            else if (args.Length == 2 && (args[0].Equals("remove") || args[0].Equals("rm") || args[0].Equals("r")))
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
                Console.WriteLine("  macros (m)                   - List all macros");
                Console.WriteLine("  open (o)                     - Open the settings JSON file");
                Console.WriteLine("  open macros                  - Open the macros JSON file");
                Console.WriteLine("  add (a) [key] [value]        - Add a new macro (key can only contain alphabetical characters)");
                Console.WriteLine("  remove (rm) [key]            - Remove an existing macro");
                Console.WriteLine("  clean (c)                    - Clean the buffer");
                Console.WriteLine("  start (+)                    - Start listening for macros");
                Console.WriteLine("  stop (-)                     - Stop listening for macros");
                Console.WriteLine("  clear (cls)                  - Clear the console screen");
                Console.WriteLine("  quit (q)                     - Exit the program");
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

        static Dictionary<string, string> GetDictionary(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
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
    }
}
