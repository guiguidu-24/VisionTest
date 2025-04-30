using POC_Tesseract.UserInterface;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Events;
using static System.Net.Mime.MediaTypeNames;


namespace POC_Tesseract
{
    public class Appli
    {
        private OCREngine ocrEngine;
        private ImgEngine imgEngine;
        private ProcessStartInfo processStartInfo;
        private string processName;

        public Appli(string appPath)
        {
            ocrEngine = new OCREngine("eng");
            imgEngine = new ImgEngine();

            processStartInfo = new ProcessStartInfo
            {
                FileName = appPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false
            };

            processName = String.Empty;
        }

        public Appli(string appPath, string[] args) : this(appPath)
        {
            processStartInfo.Arguments = string.Join(" ", args);
        }


        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open()
        {

            var processInst = new Process() { StartInfo = processStartInfo };
            processInst.Start();
            this.processName = processInst.ProcessName;
        }

        /// <summary>
        /// Gets the screen capture of the primary screen.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetScreen()
        {
            if (System.Windows.Forms.Screen.PrimaryScreen == null)
            {
                throw new InvalidOperationException("Primary screen is not available.");
            }

            Rectangle bounds = new Rectangle(
                0,
                0,
                UserInterface.Screen.Width,
                UserInterface.Screen.Height
            );


            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);


            return bitmap;
        }


        /// <summary>
        /// Simulates a mouse click at the specified screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Click(Point point)
        {
            Simulate.Events().MoveTo((int)(point.X/UserInterface.Screen.GetScaleFactor()), (int)(point.Y / UserInterface.Screen.GetScaleFactor())).Invoke().Wait();
            Simulate.Events().Click(ButtonCode.Left).Invoke().Wait();
        }

        /// <summary>
        /// Simulates a mouse click on a specific text on the screen.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="timeout"></param>
        public void Click(string text, int timeout = 5000)
        {
            var point = WaitFor(text, timeout);
            Click(point);
        }

        /// <summary>
        /// Simulates a mouse click on a specific image on the screen.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="timeout"></param>
        public void Click(Bitmap image, int timeout = 5000)
        {
            var point = WaitFor(image, timeout);
            Click(point);
        }

        /// <summary>
        /// Simulates a mouse click on a specific screen element.
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="timeout"></param>
        public void Click(ScreenElement elt, int timeout = 5000)
        {
            var point = WaitFor(elt, timeout);
            Click(point);
        }


        /// <summary>
        /// Waits for a specific text to appear on the screen.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="timeout">in milliseconds</param>
        public Point WaitFor(string text, int timeout = 5000)
        {
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area;
            DateTime start = DateTime.Now;
            

            // Wait for the text to appear on the screen
            while (!ocrEngine.Find(GetScreen(), text, out area))
            {
                //if (elapsedTime >= timeout)
                if (DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
                {
                    throw new TimeoutException($"The text '{text}' did not appear within the timeout period of {timeout} milliseconds.");
                }

                Wait(interval);
            }

            return new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        } 

        /// <summary>
        /// Waits for a specific image to appear on the screen.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public Point WaitFor(Bitmap image, int timeout = 5000, float threshold = 0.9f) //TODO When you wait for an image, you should not use the path directly and not the bitmap or maybe just a database request
        {
            DateTime start = DateTime.Now;
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area;

            // Wait for the text to appear on the screen
            while (!imgEngine.Find(GetScreen(), image, out area,threshold: threshold))
            {
                if (DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
                {
                    throw new TimeoutException($"The image did not appear within the timeout period of {timeout} milliseconds.");
                }

                Wait(interval);
            }

            return new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        }

        /// <summary>
        /// Waits for a specific screen element to appear on the screen.
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="timeout">The maximum time to wait for</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public Point WaitFor(ScreenElement elt, int timeout = 5000)
        {
            DateTime start = DateTime.Now;
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area = Rectangle.Empty;

            // Wait for either the image or the text to appear on the screen
            while (true)
            {
                // Check for the image
                if (elt.Image != default && imgEngine.Find(GetScreen(), elt.Image, out area))
                {
                    break; // Image found
                }

                // Check for the text
                if (elt.Text != default && ocrEngine.Find(GetScreen(), elt.Text, out area))
                {
                    break; // Text found
                }

                // Check if timeout has been reached
                if (DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
                {
                    throw new TimeoutException($"The element was not found within the timeout period of {timeout} milliseconds.");
                }

                Wait(interval);
            }

            // Return the center point of the found area
            return new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        }

        /// <summary>
        /// Waits for a specified amount of time.
        /// </summary>
        /// <param name="ms"></param>
        public void Wait(int ms)
        {
            Task.Delay(ms).Wait();
        }

        /// <summary>
        /// Closes the application window
        /// </summary>
        public void CloseWindow()
        {
            var processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                //if (!process.CloseMainWindow())
                //    throw new InvalidOperationException($"Failed to close the process {process.ProcessName}.");
                if (!process.CloseMainWindow())
                    process.Kill();
            }
        }

        /// <summary>
        /// Resizes the window of the application to the specified width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ResizeWindow(int width, int height)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                Window.Resize(width, height, process.MainWindowHandle);
            }
        }

        /// <summary>
        /// Maximizes the window of the application.
        /// </summary>
        public void MaximizeWindow()
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                Window.MaximizeWindow(process.MainWindowHandle);
            }
        }

        /// <summary>
        /// Simulates a keyboard input of the specified text.
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            Simulate.Events().Click(text).Invoke().Wait();
        }
    }
}
