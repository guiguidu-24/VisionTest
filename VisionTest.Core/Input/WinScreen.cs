using System.Runtime.InteropServices;

namespace VisionTest.Core.Input;

public class WinScreen : IScreen       
{
    public Size ScreenSize => new Size(width, height);

    private int width = (int)(GetPrimaryScreenWidth() * GetScaleFactor());
    private int height = (int)(GetPrimaryScreenHeight() * GetScaleFactor());

    [DllImport("User32.dll")]
    private static extern int GetSystemMetrics(SystemMetric smIndex);

    [DllImport("Shcore.dll")]
    private static extern int GetScaleFactorForMonitor(IntPtr hMonitor, out DEVICE_SCALE_FACTOR scale);

    [DllImport("User32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, MONITOR_DEFAULTTO dwFlags);

    private enum SystemMetric : int
    {
        SM_CXSCREEN = 0,  // Width of the screen of the primary display monitor, in pixels
        SM_CYSCREEN = 1,  // Height of the screen of the primary display monitor, in pixels
    }

    private enum MONITOR_DEFAULTTO : uint
    {
        MONITOR_DEFAULTTONULL = 0,
        MONITOR_DEFAULTTOPRIMARY = 1,
        MONITOR_DEFAULTTONEAREST = 2
    }

    private enum DEVICE_SCALE_FACTOR
    {
        SCALE_100_PERCENT = 100,
        SCALE_125_PERCENT = 125,
        SCALE_150_PERCENT = 150,
        SCALE_175_PERCENT = 175,
        SCALE_200_PERCENT = 200,
        // etc.
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    private static int GetPrimaryScreenWidth()
    {
        return GetSystemMetrics(SystemMetric.SM_CXSCREEN);
    }

    private static int GetPrimaryScreenHeight()
    {
        return GetSystemMetrics(SystemMetric.SM_CYSCREEN);
    }

    public float ScaleFactor => GetScaleFactor();

    private static float GetScaleFactor()
    {
        POINT pt = new POINT { X = 1, Y = 1 }; // coin haut gauche
        IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

        if (GetScaleFactorForMonitor(hMonitor, out DEVICE_SCALE_FACTOR scale) == 0)
        {
            return (float)scale / 100f;
        }

        return 1.0f; // fallback
    }

    public Bitmap CaptureScreen()
    {
        Rectangle bounds = new Rectangle(
            0,
            0,
            width,
            height
        );

        Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
        using Graphics g = Graphics.FromImage(bitmap);
        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

        return bitmap;
    }
}
