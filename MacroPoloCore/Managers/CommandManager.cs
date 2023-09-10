using CliFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MacroPoloCore.Managers
{
    internal class CommandManager
    {
        private readonly KeyProcessor keyProcessor;
        private readonly MacroPoloFileManager fileManager;

        public CommandManager(KeyProcessor keyProcessor)
        {
            this.keyProcessor = keyProcessor;
            fileManager = this.keyProcessor.fileManager;
        }

        public void ListMacros(string[] args)
        {
            var macros = fileManager.Macros;
            if (macros != null) 
            {
                if (args.Length == 1) PrettyConsole.PrintPagedListOneLine(macros, fileManager.Settings.macrosPerPage);
                else 
                {
                    string sortBy;
                    IEnumerable<KeyValuePair<string, string>> sorted;
                    if (args[1].Equals("by-key"))
                    {
                        sortBy = string.Join(" ", args.Skip(2));
                        sorted = FileManager.SortDictionaryByKeyText(macros, sortBy);
                    }
                    else if (args[1].Equals("by-value")) 
                    {
                        sortBy = string.Join(" ", args.Skip(2));
                        sorted = FileManager.SortDictionaryByValueText(macros, sortBy);
                    }
                    else
                    {
                        sortBy = string.Join(" ", args.Skip(1));
                        sorted = FileManager.SortDictionaryByValueText(macros, sortBy);
                    }
                    PrettyConsole.PrintPagedListOneLine(sorted, fileManager.Settings.macrosPerPage, header: "Sorting by: \"" + sortBy + "\"");
                }
            }
        }

        public void ListBuffers(string[] _) => PrettyConsole.PrintList(keyProcessor.GetBufferNames().Select(item => item.Bullet()));

        public void BlackistCurrent(string[] _) 
        {
            var settings = fileManager.Settings;
            if (settings != null)
            {
                var name = WindowManager.GetActiveProcess()?.ProcessName;
                if (name != null && !settings.blacklist.Contains(name))
                {
                    settings.blacklist.Add(name);
                    keyProcessor.ClearContainer();
                    fileManager.Settings = settings;
                    Console.WriteLine("Added \"" + name + "\" to blacklist.");
                }
            }
        }

        public void ListBlackist(string[] _)
        {
            var settings = fileManager.Settings;
            if (settings != null) PrettyConsole.PrintList(settings.blacklist.Select(item => item.Bullet()));
        }

        public void Reload(string[] _)
        {
            var awake = keyProcessor.awake;
            keyProcessor.awake = false;
            fileManager.Reload();
            keyProcessor.ClearContainer();
            keyProcessor.awake = true;
            Console.WriteLine("Reloaded settings.");
        }

        public void Clean(string[] _) =>
            Console.WriteLine("Cleaned up " + keyProcessor.ClearContainer() + " buffer(s).");

        public void AddMacro(string[] args)
        {
            var settings = fileManager.Settings;
            if (settings != null)
            {
                var key = args[1];
                string value;
                if (args.Length >= 3 && args[2].StartsWith(settings.openBlock))
                {
                    value = args[2].Replace(settings.openBlock, string.Empty);
                    if (args[2].EndsWith(settings.closeBlock))
                        value = value.Substring(0, value.Length - settings.closeBlock.Length);
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
                            if (line.Contains(settings.closeBlock))
                            {
                                value = value.Replace(settings.closeBlock, string.Empty).Trim();
                                break;
                            }
                            isFirstLine = false;
                        }
                    }
                }
                else value = string.Join(" ", args.Skip(2));
                if (fileManager.AddMacro(key, value)) 
                    PrettyConsole.PrintKeyValue(key, value);
                else PrettyConsole.PrintError("Could not add key.");
            }
        }

        public void RemoveMacro(string[] args)
        {
            var key = args[1];
            var value = fileManager.RemoveMacro(key);
            if (value != null) PrettyConsole.PrintKeyValue(key, value, '\u2260');
            else PrettyConsole.PrintError("Unable to remove key.");
        }

        public void StopListening(string[] _)
        {
            keyProcessor.awake = false;
            Console.WriteLine("Stopped listening.");
        }

        public void StartListening(string[] _)
        {
            keyProcessor.awake = false;
            Console.WriteLine("Listening...");
        }

        public void OpenSettings(string[] _)
        {
            var path = MacroPoloFileManager.SettingsFilePath;
            ProcessStartInfo psi = new()
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
            Console.WriteLine(path);
        }

        public void OpenMacros(string[] _)
        {
            var path = fileManager.MacrosFilePath;
            ProcessStartInfo psi = new()
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
            Console.WriteLine(path);
        }
    }
}