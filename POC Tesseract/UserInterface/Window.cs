using System.Runtime.InteropServices;

namespace POC_Tesseract.UserInterface
{
    internal class Window
    {
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const int SW_MAXIMIZE = 3;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// Resizes the window with the specified handle to the given width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="windowHandle"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Resize(int width, int height, IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Invalid window handle.");
            }

            // Resize the window
            if (!SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, width, height, SWP_NOZORDER | SWP_NOMOVE))
            {
                throw new InvalidOperationException("Failed to resize the window.");
            }
        }


        /// <summary>
        /// Maximizes the window with the specified handle.
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void MaximizeWindow(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Invalid window handle.");
            }

            // Maximize the window
            if (!ShowWindow(windowHandle, SW_MAXIMIZE))
            {
                throw new InvalidOperationException("Failed to maximize the window.");
            }
        }

    }
}
