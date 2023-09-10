using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MacroPoloCore.Managers
{
    public static class WindowManager
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public static Process GetActiveProcess()
        {
            var activeWindowHandle = GetForegroundWindow();
            if (activeWindowHandle != IntPtr.Zero)
            {
                GetWindowThreadProcessId(activeWindowHandle, out uint processId);
                return Process.GetProcessById((int)processId);
            }
            return null;
        }
    }
}
