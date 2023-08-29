using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroPolo
{
    public class Polo
    {
        private Container container;

        private const int WM_KEYDOWN = 0x0100;

        private static readonly object containerlock = new object();
        private static DateTime lastPasteTime = DateTime.Now;
        private static TimeSpan pasteCooldown = TimeSpan.FromSeconds(1);

        public static bool awake = true;

        public Polo() => CreateContainer();

        public void CreateContainer() => container = new Container(10, 100);

        public int Clear() => container.Clear();

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

        public IEnumerable<string> GetBufferNames() => container.GetBufferNames();

        private void ProcessMacro(int vkCode)
        {
            var buffer = container.Buffer;
            if (buffer != null)
            {
                if (IsWhiteSpace(vkCode))
                {
                    var str = GetLastWord(buffer.ToString());
                    if (Macro.Macros.ContainsKey(str))
                    {
                        var currentTime = DateTime.Now;
                        if (currentTime - lastPasteTime >= pasteCooldown)
                        {
                            lastPasteTime = currentTime;
                            var original = Clipboard.GetText();
                            var replace = Macro.Macros[str];
                            Clipboard.SetText(replace);
                            SendKeys.SendWait("^+{LEFT}");
                            SendKeys.SendWait("^{V}");
                            if (!string.IsNullOrEmpty(original))
                                Clipboard.SetText(original);
                        }
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

        static string GetLastWord(string input)
        {
            var lastSpaceIndex = input.LastIndexOf(' ');
            if (lastSpaceIndex >= 0)
                return input.Substring(lastSpaceIndex + 1);
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
            switch (vkCode)
            {
                case 32: return ' ';

                case 48: return shift ? ')' : '0';
                case 49: return shift ? '!' : '1';
                case 50: return shift ? '@' : '2';
                case 51: return shift ? '#' : '3';
                case 52: return shift ? '$' : '4';
                case 53: return shift ? '%' : '5';
                case 54: return shift ? '^' : '6';
                case 55: return shift ? '&' : '7';
                case 56: return shift ? '*' : '8';
                case 57: return shift ? '(' : '9';

                case 192: return shift ? '~' : '`';
                case 189: return shift ? '_' : '-';
                case 187: return shift ? '+' : '=';
                case 111: return '/';
                case 106: return '*';
                case 109: return '-';
                case 219: return shift ? '{' : '[';
                case 221: return shift ? '}' : ']';
                case 220: return shift ? '|' : '\\';
                case 107: return '+';
                case 186: return shift ? ':' : ';';
                case 222: return shift ? '"' : '\'';
                case 13: return '\n';
                case 188: return shift ? '<' : ',';
                case 190: return shift ? '>' : '.';
                case 191: return shift ? '?' : '/';
                case 9: return '\t';

                default: return '\0'; // Unknown key
            }
        }
    }
}
