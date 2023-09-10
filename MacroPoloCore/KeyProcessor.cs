using CliFramework;
using MacroPoloCore.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MacroPoloCore.Utilities;

namespace MacroPoloCore
{
    internal class KeyProcessor
    {
        private Container container;
        public readonly MacroPoloFileManager fileManager;

        private const int WM_KEYDOWN = 0x0100;

        private static readonly object containerlock = new object();
        private static readonly TimeSpan pasteCooldown = TimeSpan.FromSeconds(1);
        private static DateTime lastPasteTime = DateTime.Now;

        public bool awake = true;

        public KeyProcessor(MacroPoloFileManager fileManager)
        {
            this.fileManager = fileManager;
            InitializeContainer();
        }

        public void InitializeContainer() => container = new Container(fileManager, 10, 100);

        public int ClearContainer() => container.Clear();

        public void ProcessKeys(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (awake && nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                lock (containerlock)
                {
                    ProcessMacro(vkCode);
                }
            }
        }

        private string FormatFirstUpper(string input) => char.ToUpper(input[0]) + input[1..];
        private string FormatFirstLower(string input) => char.ToLower(input[0]) + input[1..];

        public IEnumerable<string> GetBufferNames() => container.GetBufferNames();

        private void ProcessMacro(int vkCode)
        {
            var buffer = container.Buffer;
            if (buffer != null)
            {
                if (IsWhiteSpace(vkCode))
                {
                    var str = GetLastWord(buffer.ToString());
                    var macros = fileManager.Macros;
                    var specialMacros = fileManager.SpecialMacros;
                    if (macros != null && specialMacros != null)
                    {
                        string strLower = str.ToLower();
                        if (specialMacros.ContainsKey(strLower))
                        {
                            var specialMacro = specialMacros[strLower];
                            if (specialMacro.type == SpecialMacroType.IgnoreCase)
                            {
                                if (macros.ContainsKey(specialMacro.value))
                                    SendReplacement(macros[specialMacro.value]);
                            }
                            else if (specialMacro.type == SpecialMacroType.FirstCase)
                            {
                                if (macros.ContainsKey(specialMacro.value) && FormatFirstLower(str).Equals(FormatFirstLower(specialMacro.value)))
                                {
                                    string replace = macros[specialMacro.value];
                                    if (char.IsUpper(str[0]))
                                        replace = FormatFirstUpper(replace);
                                    else replace = FormatFirstLower(replace);
                                    SendReplacement(replace);
                                }
                            }
                            else if (specialMacro.type == SpecialMacroType.Pluralize) 
                            {
                                if (macros.ContainsKey(str))
                                    SendReplacement(macros[str]);
                            }
                            else if (specialMacro.type == SpecialMacroType.FirstCasePluralize)
                            {
                                if (macros.ContainsKey(specialMacro.value) && specialMacros.ContainsKey(specialMacro.value))
                                {
                                    var referenceKey = specialMacros[specialMacro.value].value;
                                    if (FormatFirstLower(str).Equals(FormatFirstLower(specialMacros[specialMacro.value].value))) {
                                        string replace = macros[referenceKey];
                                        if (char.IsUpper(str[0]))
                                            replace = FormatFirstUpper(replace);
                                        else replace = FormatFirstLower(replace);
                                        SendReplacement(replace);
                                    }
                                }
                            }
                        }
                        else if (macros.ContainsKey(str))
                            SendReplacement(macros[str]);
                    }
                    buffer.Clear();
                }
                else
                {
                    if (IsLeftArrow(vkCode)) buffer.MoveLeft();
                    else if (IsRightArrow(vkCode)) buffer.MoveRight();
                    else if (IsBackspace(vkCode)) buffer.Remove();
                    else AddToBuffer(buffer, vkCode);
                } 
            }
        }

        private static void SendReplacement(string replace)
        {
            var currentTime = DateTime.Now;
            if (currentTime - lastPasteTime >= pasteCooldown)
            {
                lastPasteTime = currentTime;
                var original = Clipboard.GetText();
                Clipboard.SetText(replace);
                SendKeys.SendWait("^+{LEFT}");
                SendKeys.SendWait("^{V}");
                if (!string.IsNullOrEmpty(original))
                    Clipboard.SetText(original);
            }
        }

        private static string GetLastWord(string input)
        {
            var lastSpaceIndex = input.LastIndexOf(' ');
            if (lastSpaceIndex >= 0)
                return input[(lastSpaceIndex + 1)..];
            else return input;
        }

        private static void AddToBuffer(Buffer<char> buffer, int vkCode)
        {
            if (!IsShift(vkCode))
            {
                var ch = ConvertToChar(vkCode);
                if (ch == '\0') buffer.Clear();
                else buffer.Add(ch);
            }
        }

        private static bool IsWhiteSpace(int vkCode) =>
            vkCode == 13 || vkCode == 9 || vkCode == 32;

        private static bool IsShift(int vkCode) =>
            vkCode == 160 || vkCode == 161 || vkCode == 16;

        private static bool IsLeftArrow(int vkCode) => vkCode == 37;
        private static bool IsRightArrow(int vkCode) => vkCode == 39;
        private static bool IsBackspace(int vkCode) => vkCode == 8;

        private static char ConvertToChar(int vkCode)
        {
            bool shift = (Control.ModifierKeys & Keys.Shift) != Keys.None;
            bool capslock = Control.IsKeyLocked(Keys.CapsLock);
            bool numlock = Control.IsKeyLocked(Keys.NumLock);

            if (vkCode >= 65 && vkCode <= 90)
            {
                var c = (char)vkCode;
                return (capslock ^ shift) ? char.ToUpper(c) : char.ToLower(c);
            }
            else if (vkCode >= 96 && vkCode <= 105)
            {
                if (numlock) return (char)(vkCode - 48);
                else return '\0';
            }
            else return '\0';
            //switch (vkCode)
            //{
            //    case 32: return ' ';

            //    case 48: return shift ? ')' : '0';
            //    case 49: return shift ? '!' : '1';
            //    case 50: return shift ? '@' : '2';
            //    case 51: return shift ? '#' : '3';
            //    case 52: return shift ? '$' : '4';
            //    case 53: return shift ? '%' : '5';
            //    case 54: return shift ? '^' : '6';
            //    case 55: return shift ? '&' : '7';
            //    case 56: return shift ? '*' : '8';
            //    case 57: return shift ? '(' : '9';

            //    case 192: return shift ? '~' : '`';
            //    case 189: return shift ? '_' : '-';
            //    case 187: return shift ? '+' : '=';
            //    case 111: return '/';
            //    case 106: return '*';
            //    case 109: return '-';
            //    case 219: return shift ? '{' : '[';
            //    case 221: return shift ? '}' : ']';
            //    case 220: return shift ? '|' : '\\';
            //    case 107: return '+';
            //    case 186: return shift ? ':' : ';';
            //    case 222: return shift ? '"' : '\'';
            //    case 13: return '\n';
            //    case 188: return shift ? '<' : ',';
            //    case 190: return shift ? '>' : '.';
            //    case 191: return shift ? '?' : '/';
            //    case 9: return '\t';

            //    default: return '\0'; // Unknown key
            //}
        }
    }
}
