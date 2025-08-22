using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using WindowsInput.Events;

namespace VisionTest.Core.Input;

public class Window : IWindow
{
    private readonly nint _windowHandle;
    private Rectangle _bounds = new Rectangle();


    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const int SW_MAXIMIZE = 3;


    public string Title => GetWindowTitle();

    public Rectangle Bounds
    {
        get
        {
            GetWindowRect(_windowHandle, ref _bounds);
            return _bounds;
        }
    }

    public Window(nint windowHandle)
    {
        _windowHandle = windowHandle;
    }

    public Window(string processName)
    {
        var process = Process.GetProcessesByName(processName).FirstOrDefault();
        if (process == null)
        {
            throw new InvalidOperationException($"Process '{processName}' not found.");
        }
        _windowHandle = process.MainWindowHandle;
    }


    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    /// <summary>
    /// The GetWindowRect function retrieves the dimensions of the bounding rectangle of the specified window.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="lpRect"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern long GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

    [DllImport("user32.dll")]
    public static extern int SetForegroundWindow(IntPtr hWnd);

    private string GetWindowTitle()
    {
        const int nChars = 256;
        StringBuilder Buff = new StringBuilder(nChars);
        IntPtr handle = _windowHandle;

        if (GetWindowText(handle, Buff, nChars) > 0)
        {
            return Buff.ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// Resizes the window with the specified handle to the given width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="windowHandle"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Resize(int width, int height)
    {
        if (_windowHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Invalid window handle.");
        }

        // Resize the window
        if (!SetWindowPos(_windowHandle, IntPtr.Zero, 0, 0, width, height, SWP_NOZORDER | SWP_NOMOVE))
        {
            throw new InvalidOperationException("Failed to resize the window.");
        }
    }


    /// <summary>
    /// Maximizes the window with the specified handle.
    /// </summary>
    /// <param name="windowHandle"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Maximize()
    {
        if (_windowHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Invalid window handle.");
        }

        // Maximize the window
        if (!ShowWindow(_windowHandle, SW_MAXIMIZE))
        {
            throw new InvalidOperationException("Failed to maximize the window.");
        }
    }

    public void Activate()
    {
        SetForegroundWindow(_windowHandle);
    }

    public void Close()
    {
        Activate();
        var keyboard = new Keyboard();
        keyboard.SendModifiedKeyStroke([KeyCode.Alt], KeyCode.F4);
    }

}
