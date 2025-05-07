using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace VSCaptureExtension
{
    public class Screen
    {
        public static int Width => (int)(SystemParameters.PrimaryScreenWidth * GetScaleFactor());
        public static int Height => (int)(SystemParameters.PrimaryScreenWidth * GetScaleFactor());

        [DllImport("Shcore.dll")]
        private static extern int GetScaleFactorForMonitor(IntPtr hMonitor, out DEVICE_SCALE_FACTOR scale);

        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, MONITOR_DEFAULTTO dwFlags);

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

        public static float GetScaleFactor()
        {
            POINT pt = new POINT { X = 1, Y = 1 }; // coin haut gauche
            IntPtr hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTO.MONITOR_DEFAULTTONEAREST);

            if (GetScaleFactorForMonitor(hMonitor, out DEVICE_SCALE_FACTOR scale) == 0)
            {
                return (float)scale / 100f;
            }

            return 1.0f; // fallback
        }

        public static Bitmap Shoot()
        {
            Rectangle bounds = new Rectangle(
                0,
                0,
                Width,
                Height
            );


            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);


            return bitmap;
        }

        public static Bitmap Shoot(Rectangle zone)
        {
            var image = Shoot();
            return image.Clone(zone, image.PixelFormat);
        }

    }
}
