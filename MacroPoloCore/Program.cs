using CliFramework;
using MacroPoloCore.Managers;
using System;
using System.Windows.Forms;

namespace MacroPoloCore
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MacroPoloFileManager fileManager = new();
            KeyProcessor keyProcessor = new(fileManager);
            CommandManager commandManager = new(keyProcessor);
            Repl repl = new()
            {
                preprocessArg = str => str,
                onQuit = Application.Exit,
                pagifyHelp = fileManager.Settings.macrosPerPage
            };
            repl.AddCommand(
                args => args.Length >= 1 && (args[0].Equals("macros") || args[0].Equals("m")),
                commandManager.ListMacros,
                "macros (m) [text]",
                "List all macros sorted by their similarity to [text]."
            );
            repl.AddCommand(
                args => args.Length >= 1 && (args[0].Equals("special") || args[0].Equals("s")),
                commandManager.ListSpecialMacros,
                "special (s) [text]",
                "List all special macros sorted by their similarity to [text]."
            );
            repl.AddCommand(
                args => args.Length == 1 && args[0].Equals("buffers"),
                commandManager.ListBuffers,
                "buffers",
                "List all the active buffers."
            );
            repl.AddCommand(
                args => args.Length == 1 && args[0].Equals("blacklist-current"),
                commandManager.BlackistCurrent,
                "blacklist-current",
                "Blacklist the current process's name."
            );
            repl.AddCommand(
                args => args.Length == 1 && args[0].Equals("blacklist"),
                commandManager.ListBlackist,
                "blacklist",
                "List all blacklisted processes"
            );
            repl.AddCommand(
                args => args.Length == 1 && (args[0].Equals("reload") || args[0].Equals("r")),
                commandManager.Reload,
                "reload (r)",
                "Reload the settings JSON file."
            );
            repl.AddCommand(
                args => args.Length == 1 && args[0].Equals("clean"),
                commandManager.Clean,
                "clean",
                "Clean the buffer."
            );
            repl.AddCommand(
                args => args.Length >= 3 && (args[0].Equals("add") || args[0].Equals("a")),
                commandManager.AddMacro,
                "add (a) [key] [value]",
                "Add a new macro\n(keys can only contain alphabetical characters)"
            );
            repl.AddCommand(
                args => args.Length >= 3 && (args[0].Equals("add-ignore") || args[0].Equals("a!")),
                commandManager.AddIngoreCaseMacro,
                "add-ignorecase (a!) [key] [value]",
                "Add a new macro\nwith its key's casing ignored."
            );
            repl.AddCommand(
                args => args.Length >= 3 && (args[0].Equals("add-first") || args[0].Equals("a@")),
                commandManager.AddFirstCaseMacro,
                "add-first (a@) [key] [value]",
                "Add a new macro\nthat copies the casing of the key's first character."
            );
            repl.AddCommand(
                args => args.Length >= 3 && (args[0].Equals("add-plural") || args[0].Equals("a$")),
                commandManager.AddPluralizeMacro,
                "add-plural (a$) [key] [value]",
                "Add a new macro\nwith both a singular and plural version."
            );
            repl.AddCommand(
                args => args.Length >= 3 && (args[0].Equals("add-first-plural") || args[0].Equals("a@$") || args[0].Equals("a$@")),
                commandManager.AddFirstCasePluralizeMacro,
                "add-first-plural [key] [value]",
                "Add a new macro\nwith both a singular and plural version\nthat copies the casing of the key's first character."
            );
            repl.AddCommand(
                args => args.Length == 2 && (args[0].Equals("remove") || args[0].Equals("rm")),
                commandManager.RemoveMacro,
                "remove (rm) [key]",
                "Remove an existing macro."
            );
            repl.AddCommand(
                args => args.Length == 1 && (args[0].Equals("stop") || args[0].Equals("-")),
                commandManager.StopListening,
                "stop (-)",
                "Stop listening for macros."
            );
            repl.AddCommand(
               args => args.Length == 1 && (args[0].Equals("start") || args[0].Equals("+")),
               commandManager.StartListening,
               "start (+)",
               "Start listening for macros."
            );
            repl.AddCommand(
               args => args.Length == 1 && (args[0].Equals("open") || args[0].Equals("o")),
               commandManager.OpenSettings,
               "open (o)",
               "Open the settings JSON file."
            );
            repl.AddCommand(
               args => args.Length == 2 && (args[0].Equals("open") || args[0].Equals("o")) && (args[1].Equals("macros") || args[1].Equals("m")),
               commandManager.OpenMacros,
               "open macros",
               "Open the macros JSON file."
            );
            repl.AddCommand(
               args => args.Length == 2 && (args[0].Equals("open") || args[0].Equals("o")) && (args[1].Equals("special") || args[1].Equals("s")),
               commandManager.OpenSpecialMacros,
               "open special",
               "Open the special JSON file."
            );
            MacroPoloApplication.Build(keyProcessor, repl);
        }
    }
}