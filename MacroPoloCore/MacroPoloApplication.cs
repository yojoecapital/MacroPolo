using CliFramework;
using MacroPoloCore.Managers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MacroPoloCore
{
    internal static class MacroPoloApplication
    {
        private static KeyProcessor keyProcessor;

        private const int WH_KEYBOARD_LL = 13;
        private static LowLevelKeyboardProc _keyboardProc;
        private static IntPtr _keyboardHookID = IntPtr.Zero;

        [STAThread]
        public static void Build(KeyProcessor keyProcessor, Repl repl)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            MacroPoloApplication.keyProcessor = keyProcessor;

            _keyboardProc = KeyboardHookCallback;

            _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);

            // Start the console input loop in a separate thread
            Thread consoleThread = new(repl.Run);
            consoleThread.Start();

            Application.Run();

            UnhookWindowsHookEx(_keyboardHookID);

        }

        private static IntPtr SetHook(Delegate proc, int hookType)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(hookType, proc, GetModuleHandle(curModule.ModuleName), 0);
        }


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam); // Mouse hook delegate

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            keyProcessor.ProcessKeys(nCode, wParam, lParam);
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }


        #region Win32 API

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }
}
