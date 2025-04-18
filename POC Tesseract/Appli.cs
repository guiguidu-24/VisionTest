using System.Diagnostics;


namespace POC_Tesseract
{
    internal class Appli
    {
        private Process processInst;
        private OCREngine ocrEngine;
        private ImgEngine imgEngine;

        public Appli(string appPath)
        {
            ocrEngine = new OCREngine("eng");
            imgEngine = new ImgEngine();
            processInst = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                }
            };
        }

        public Appli(string appPath, string[] args) : this(appPath)
        {
            processInst.StartInfo.Arguments = string.Join(" ", args);

        }


        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open()
        {
            if (processInst != null)
            {
                processInst.Start();
            }
            else
            {
                throw new InvalidOperationException($"Impossible to start the process");
            }
        }

        /// <summary>
        /// Gets the screen capture of the primary screen.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;

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
            Mouse.ClickAt(point.X, point.Y);
        }


        /// <summary>
        /// Waits for a specific text to appear on the screen.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="timeout">in milliseconds</param>
        public Point WaitFor(string text, int timeout = 5000) //TODO wait for an element containing the text and the bitmap object
        {
            int elapsedTime = 0;
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area;

            // Wait for the text to appear on the screen
            while (!ocrEngine.Find(GetScreen(), text, out area))
            {
                if (elapsedTime >= timeout)
                {
                    throw new TimeoutException($"The text '{text}' did not appear within the timeout period of {timeout} milliseconds.");
                }

                Wait(interval);
                elapsedTime += interval;
            }

            return new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        }

        public Point WaitFor(Bitmap image, int timeout = 60000) //TODO wait for an element containing the text and the bitmap object
        {
            int elapsedTime = 0;
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area;

            // Wait for the text to appear on the screen
            while (!imgEngine.Find(GetScreen(), image, out area))
            {
                if (elapsedTime >= timeout)
                {
                    throw new TimeoutException($"The image did not appear within the timeout period of {timeout} milliseconds.");
                }

                Wait(interval);
                elapsedTime += interval;
                Console.WriteLine($"Imagee not found.");
            }

            return new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        }


        /// <summary>
        /// Waits for a specified amount of time.
        /// </summary>
        /// <param name="ms"></param>
        public void Wait(int ms)
        {
            Thread.Sleep(ms);
        }


        /// <summary>
        /// Closes the application and releases resources.
        /// </summary>
        public void Close()//TODO fix the closing of the process
        {
            processInst.CloseMainWindow();
            processInst.Close();
            processInst.Dispose();
        }
    }
}
