using System;
using System.Runtime.InteropServices;

public static class Window
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    public static int? GetActiveWindowId()
    {
        var handle = GetForegroundWindow();
        if (handle == IntPtr.Zero) return null;
        return (int)handle;
    }

    public static string GetWindowName(int windowId)
    {
        const int maxWindowTitleLength = 256;
        var windowTitle = new System.Text.StringBuilder(maxWindowTitleLength);
        GetWindowText((IntPtr)windowId, windowTitle, maxWindowTitleLength);
        return windowTitle.ToString();
    }
}
