using CliFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MacroPoloCore.Utilities;

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

        private KeyValuePair<string, string> SpecialMacroPairToAssociation(KeyValuePair<string, SpecialMacro> pair, Dictionary<string, string> macros)
        {
            var key = pair.Value.value;
            var display = pair.Value.ToString();
            if (macros.ContainsKey(key))
                return new KeyValuePair<string, string>(display, macros[key]);
            else return new KeyValuePair<string, string>(display, "<?>");
        }

        public void ListSpecialMacros(string[] args)
        {
            var specialMacros = fileManager.SpecialMacros;
            var macros = fileManager.Macros;
            var header = "Ignore case (!), First case (@), Pluralize ($)";
            if (specialMacros != null && macros != null)
            {
                if (args.Length == 1) 
                    PrettyConsole.PrintPagedListOneLine(specialMacros.Select(pair => SpecialMacroPairToAssociation(pair, macros)), fileManager.Settings.macrosPerPage, header: header);
                else
                {
                    var sortBy = string.Join(" ", args.Skip(1));
                    var sorted = FileManager.SortDictionaryByKeyText(specialMacros, sortBy).Select(pair => SpecialMacroPairToAssociation(pair, macros));
                    PrettyConsole.PrintPagedListOneLine(sorted, fileManager.Settings.macrosPerPage, header: header + ", Sorting by: \"" + sortBy + "\"");
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

        private string TakeMultilineInput(string[] args, int argIndex = 1)
        {
            var settings = fileManager.Settings;
            if (settings != null)
            {
                string value;
                if (args.Length >= argIndex + 1 && args[argIndex].StartsWith(settings.openBlock))
                {
                    value = string.Join(" ", args.Skip(argIndex)).ReplaceFirstInstance(settings.openBlock, string.Empty);
                    if (value.EndsWith(settings.closeBlock))
                        value = value[..^settings.closeBlock.Length];
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
                            if (line.EndsWith(settings.closeBlock))
                            {
                                value = value.ReplaceLastInstance(settings.closeBlock, string.Empty).Trim();
                                break;
                            }
                            isFirstLine = false;
                        }
                    }
                }
                else value = string.Join(" ", args.Skip(argIndex));
                return value;
            }
            return null;
        }

        private (string, string) AddMacroInput(string[] args)
        {
            var key = args[1];
            var value = TakeMultilineInput(args, 2);
            if (value != null) return (key, value);
            else return default;
        }

        public void AddMacro(string[] args)
        {
            var tuple = AddMacroInput(args);
            if (tuple != default)
            {
                (string key, string value) = tuple;
                if (fileManager.AddMacro(key, value))
                    PrettyConsole.PrintKeyValue(key, value);
                else PrettyConsole.PrintError("Could not add key.");
            }
        }

        public void AddTemporaryMacro(string[] args)
        {
            var tuple = AddMacroInput(args);
            if (tuple != default)
            {
                (string key, string value) = tuple;
                if (fileManager.AddTemporaryMacro(key, value))
                    PrettyConsole.PrintKeyValue(key, value);
                else PrettyConsole.PrintError("Could not add key.");
            }
        }

        public void AddIngoreCaseMacro(string[] args)
        {
            var tuple = AddMacroInput(args);
            if (tuple != default)
            {
                (string key, string value) = tuple;
                if (fileManager.AddMacro(key, value) && fileManager.AddSpecialMacro(key, key, SpecialMacroType.IgnoreCase))
                    PrettyConsole.PrintKeyValue(key, value);
                else PrettyConsole.PrintError("Could not add key.");
            }
        }

        public void AddFirstCaseMacro(string[] args)
        {
            var tuple = AddMacroInput(args);
            if (tuple != default)
            {
                (string key, string value) = tuple;
                if (fileManager.AddMacro(key, value) && fileManager.AddSpecialMacro(key, key, SpecialMacroType.FirstCase))
                    PrettyConsole.PrintKeyValue(key, value);
                else PrettyConsole.PrintError("Could not add key.");
            }
        }

        private void AddFirstCaseOrPluralizeMacro(string[] args, bool isJustPlural)
        {
            var tuple = AddMacroInput(args);
            if (tuple != default)
            {
                var type = isJustPlural ? SpecialMacroType.Pluralize : SpecialMacroType.FirstCasePluralize;
                (string key1, string value1) = tuple;
                var key2 = key1 + "s";
                Console.Write("> " + key2 + " ");
                var args2 = Console.ReadLine().Split(' ');
                var value2 = TakeMultilineInput(args2, 0);
                if (string.IsNullOrEmpty(value2)) value2 = value1 + "s";
                if (fileManager.AddMacros((key1, value1), (key2, value2))
                    && fileManager.AddSpecialMacros((key1, key2, type), (key2, key1, type)))
                {
                    PrettyConsole.PrintKeyValue(key1, value1);
                    PrettyConsole.PrintKeyValue(key2, value2);
                }
                else PrettyConsole.PrintError("Could not add key.");
            }
        }

        public void AddPluralizeMacro(string[] args) =>
            AddFirstCaseOrPluralizeMacro(args, true);

        public void AddFirstCasePluralizeMacro(string[] args) =>
            AddFirstCaseOrPluralizeMacro(args, false);

        public void RemoveMacro(string[] args)
        {
            var key = args[1];
            var value = fileManager.RemoveTemporaryMacro(key);
            if (value != null) PrettyConsole.PrintKeyValue(key, value, '\u2260');
            else
            {
                var specialMacro = fileManager.RemoveSpecialMacro(key);
                if (specialMacro != null && (specialMacro.type == SpecialMacroType.Pluralize || specialMacro.type == SpecialMacroType.FirstCasePluralize))
                {
                    fileManager.RemoveMacro(specialMacro.value);
                    fileManager.RemoveSpecialMacro(specialMacro.value);
                }
                value = fileManager.RemoveMacro(key);
                if (value != null) PrettyConsole.PrintKeyValue(key, value, '\u2260');
                else PrettyConsole.PrintError("Could not remove key.");
            }
        }

        public void RemoveTemporaryMacro(string[] args)
        {
            var key = args[1];
            var value = fileManager.RemoveTemporaryMacro(key);
            if (value != null) PrettyConsole.PrintKeyValue(key, value, '\u2260');
            else PrettyConsole.PrintError("Could not remove key.");
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

        public void OpenSpecialMacros(string[] _)
        {
            var path = fileManager.SpecialFilePath;
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