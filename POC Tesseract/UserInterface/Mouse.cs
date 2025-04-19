using System;
using System.Runtime.InteropServices;
using System.Threading;

class Mouse
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, IntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    /// <summary>
    /// Simulates a mouse click at the specified screen coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void ClickAt(int x, int y)
    {
        SetCursorPos(x, y);                          // Move cursor
        Thread.Sleep(50);                            // Short delay (optional)
        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero); // Mouse down
        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero); // Mouse up
    }
}
