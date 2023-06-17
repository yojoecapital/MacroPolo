﻿using System;
using System.Collections.Generic;
using System.Linq;
using WindowsInput;
using WindowsInput.Events;
using WindowsInput.Events.Sources;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using System.Collections;

class Program
{
    static string buffer = string.Empty;
    static int max = 100;
    readonly static List<KeyCode> breaks = new List<KeyCode>() {
        KeyCode.Enter,
        KeyCode.Escape,
        KeyCode.Tab,
        KeyCode.Space,
        KeyCode.Left,
        KeyCode.Right,
        KeyCode.Up,
        KeyCode.Down,
        KeyCode.PageUp,
        KeyCode.PageDown,
        KeyCode.Home,
        KeyCode.End,
    };

    static readonly string macrosFileName = "macros.json";

    [STAThread]
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;

        macros = Macros;
        if (args.Length > 0 && int.TryParse(args[0], out int max) && max > 0)
            Program.max = max;

        using (var Keyboard = WindowsInput.Capture.Global.KeyboardAsync())
        {
            // Capture all events from the keyboard
            Keyboard.KeyEvent += Keyboard_KeyEvent;

            string input;
            while (true)
            {
                Console.Write("> ");
                input = Console.ReadLine().Trim();
                Console.WriteLine(input);
                if (input.Equals("q") || input.Equals("quit")) return;
                var argsArray = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                ProcessArgs(argsArray, Keyboard);
            }
        }
    }

    public static void ProcessArgs(string[] args, IKeyboardEventSource Keyboard)
    {
        if (args.Length == 1 && (args[0].Equals("macros") || args[0].Equals("m")))
        {
            var macros = Macros;
            if (macros == null) Console.WriteLine("Error: unable to parse mappings");
            else
            {
                if (macros.Count == 0) Console.WriteLine("<empty>");
                foreach (var key in macros.Keys)
                    Console.WriteLine(key.ToLower() + " \u2192 " + macros[key]);
            }
        }
        else if (args.Length == 1 && (args[0].Equals("clear") || args[0].Equals("cls")))
        {
            Console.Clear();
        }
        else if (args.Length >= 3 && (args[0].Equals("add") || args[0].Equals("a")))
        {
            var key = args[1];
            var value = string.Join(" ", args.Skip(2));
            if (AddMacro(key, value))
                Console.WriteLine(key.ToLower() + " \u2192 " + value);
            else Console.WriteLine("Error: could not add key");
        }
        else if (args.Length == 2 && (args[0].Equals("remove") || args[0].Equals("r")))
        {
            var key = args[1];
            var value = RemoveMacro(key);
            if (value != null) Console.WriteLine(key.ToLower() + " \u2260 " + value);
            else Console.WriteLine("Error: unable to find " + key);
        }
        else if (args.Length == 1 && (args[0].Equals("stop") || args[0].Equals("-")))
        {
            Keyboard.KeyEvent -= Keyboard_KeyEvent;
            Console.WriteLine("Stopped listening");
        }
        else if (args.Length == 1 && (args[0].Equals("start") || args[0].Equals("+")))
        {
            Keyboard.KeyEvent += Keyboard_KeyEvent;
            Console.WriteLine("Listening...");
        }
        else if (args.Length == 1 && (args[0].Equals("path") || args[0].Equals("p")))
        {
            Clipboard.SetText(MacrosFilePath);
            Console.WriteLine(MacrosFilePath);
        }
        else if (args.Length > 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  macros            - List all macros");
            Console.WriteLine("  add [key] [value] - Add a new macro (key can only contain alphabetical characters)");
            Console.WriteLine("  remove [key]      - Remove an existing macro");
            Console.WriteLine("  clear             - Clear console screen");
            Console.WriteLine("  quit              - Exit program");
            return;
        }
    }

    static string MacrosFilePath
    {
        get
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, macrosFileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "{}");
            }
            return filePath;
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

    static Dictionary<string, string> macros;
    static Dictionary<string, string> Macros
    {
        get { 
            return macros == null ? GetDictionary(MacrosFilePath) : macros;
        }
    }

    static bool AddMacro(string key, string value)
    {
        if (!Regex.IsMatch(key, @"^[a-zA-Z]+$")) return false;
        Macros[key.ToUpper()] = value;
        SetDictionary(MacrosFilePath, Macros);
        return true;
    }

    public static string RemoveMacro(string key)
    {
        key = key.ToUpper();
        if (Macros.ContainsKey(key))
        {
            var value = Macros[key];
            Macros.Remove(key);
            SetDictionary(MacrosFilePath, Macros);
            return value;
        } 
        else return null;
    }

    private static void Keyboard_KeyEvent(object sender, EventSourceEventArgs<KeyboardEvent> e)
    {
        
        var keyCode = e.Data?.KeyDown?.Key;
        if (keyCode != null)
        {
            var key = keyCode.ToString();
            if (buffer.Length > max) 
                buffer = string.Empty;
            else if (key.Length == 1 && char.IsLetter(key[0]))
                buffer += key;

            if (keyCode == KeyCode.Space)
            {
                if (macros.ContainsKey(buffer))
                {
                    Clipboard.SetText(macros[buffer]);
                    Simulate.Events()
                        .ClickChord(KeyCode.Control, KeyCode.Shift, KeyCode.Left)
                        .ClickChord(KeyCode.Control, KeyCode.V)
                        .Invoke().ContinueWith(_ => buffer = string.Empty);
                    buffer = string.Empty;
                } 
                else buffer = string.Empty;
            }
            else if (keyCode == KeyCode.Backspace && buffer.Length > 0)
                buffer = buffer.Substring(0, buffer.Length - 1);
            else if (breaks.Any(k => k == keyCode)) 
                buffer = string.Empty;
        }
    }
}
