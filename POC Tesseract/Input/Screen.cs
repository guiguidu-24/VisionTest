using System.Runtime.InteropServices;


namespace Core.Input
{
    public class Screen : IScreen       
    {
        public Size ScreenSize => new Size(width, height);

        public float ScaleFactor => GetScaleFactor();

        [Obsolete("⚠️ Not tested — use with caution.", false)]
        public Bitmap CaptureRegion(Rectangle region)
        {
            if (System.Windows.Forms.Screen.PrimaryScreen == null)
            {
                throw new InvalidOperationException("Primary screen is not available.");
            }

            Rectangle bounds = new Rectangle(
                region.X,
                region.Y,
                region.Width,
                region.Height
            );
            Bitmap bitmap = new Bitmap(region.Width, region.Height);
            using Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return bitmap;
        }

        private int width = (int)((System.Windows.Forms.Screen.PrimaryScreen?.Bounds.Width ?? 0) * GetScaleFactor());
        private int height = (int)((System.Windows.Forms.Screen.PrimaryScreen?.Bounds.Height ?? 0) * GetScaleFactor());

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
            if (System.Windows.Forms.Screen.PrimaryScreen == null)
            {
                throw new InvalidOperationException("Primary screen is not available.");
            }

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

}
