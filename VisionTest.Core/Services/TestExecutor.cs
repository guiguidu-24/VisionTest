using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using System.Diagnostics;
using VisionTest.Core.Input;
using VisionTest.Core.Utils;


namespace VisionTest.Core.Services
{
    public class TestExecutor //TODO : for an image, directly use the name of the image instead of the path
    {
        private readonly IMouse _mouse = new Mouse();
        private readonly IScreen _screen = new Input.Screen();
        private readonly IRecognitionEngine<string> ocrEngine;
        private readonly IRecognitionEngine<Bitmap> imgEngine;
        private ProcessStartInfo? processStartInfo;
        private string? appPath;

        public string ProcessName { get; private set; } = string.Empty;

        public string AppPath
        {
            set 
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = value,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                };
                appPath = value; 
            }
        }

        public string[] Arguments
        {
            set 
            {
                if (processStartInfo == null)
                {
                    throw new InvalidOperationException("ProcessStartInfo is not initialized. Set AppPath first.");
                }

                if (value.Length != 0)
                    processStartInfo.Arguments = string.Join(" ", value);
            }
        }

        public TestExecutor()
        {
            ocrEngine = new OCREngine("eng");
            imgEngine = new ImgEngine();
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open()
        {
            if (string.IsNullOrEmpty(appPath))
            {
                throw new InvalidOperationException("Application path is not set.");
            }
            if (processStartInfo == null)
            {
                throw new InvalidOperationException("ProcessStartInfo is not initialized. Set AppPath first.");
            }

            var processInst = new Process() { StartInfo = processStartInfo };
            processInst.Start();
            ProcessName = processInst.ProcessName;
        }

        /// <summary>
        /// Simulates a mouse click at the specified screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Click(Point point)
        {
            _mouse.MoveTo(point.X, point.Y); //(point.X / _screen.ScaleFactor), (int)(point.Y / _screen.ScaleFactor));
            _mouse.LeftClick();
        }

        /// <summary>
        /// Simulates a mouse click on a specific text on the screen.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="timeout"></param>
        public void Click(string text, int timeout = 5000)
        {
            Rectangle area = WaitFor(text, timeout);
            Click(area.Center());
        }

        /// <summary>
        /// Simulates a mouse click on a specific image on the screen.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="timeout"></param>
        public void Click(Bitmap image, int timeout = 5000)
        {
            Rectangle area = WaitFor(image, timeout);
            Click(area.Center());
        }

        //public void Click<TTarget>(TTarget target, int timeout = 5000)
        //{
        //    if (target == null)
        //    {
        //        throw new ArgumentNullException(nameof(target), "Target cannot be null.");
        //    }

        //    Rectangle area = Rectangle.Empty;

        //    if (typeof(TTarget) == typeof(string))
        //    {
        //        area = WaitFor(ocrEngine, target as string, timeout);
        //    }
        //    else if (typeof(TTarget) == typeof(Bitmap))
        //    {
        //        area = WaitFor(imgEngine, target as Bitmap, timeout);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Unsupported target type.", nameof(target));
        //    }

        //    if (area == Rectangle.Empty)
        //    {
        //        throw new TimeoutException($"The target '{target}' did not appear within the timeout period of {timeout} milliseconds.");
        //    }
        //    Click(area.Center());
        //}

        public void Click(string text, string imagePath, int timeout = 5000)
        {
            var screenElement = new ScreenElement();
            screenElement.Texts.Add(text);
            var img = new Bitmap(imagePath);
            screenElement.Images.Add(img);
            Click(screenElement, timeout);
        }

        /// <summary>
        /// Simulates a mouse click on a specific screen element.
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="timeout"></param>
        public void Click(ScreenElement elt, int timeout = 5000)
        {
            Rectangle area = WaitFor(elt, timeout);
            Click(area.Center());
        }

        private Rectangle WaitFor<TTarget>(IRecognitionEngine<TTarget> engine, TTarget target,  int timeout)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Target cannot be null.");
            }

            const int interval = 100; // Check every 100 milliseconds
            DateTime start = DateTime.Now;


            // Wait for the text to appear on the screen
            IEnumerable<Rectangle> recognitionResult;
            while (!(recognitionResult = engine.Find(_screen.CaptureScreen(), target)).Any())
            {
                if (DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
                {
                    throw new TimeoutException($"The {target.GetType()} '{target}' did not appear within the timeout period of {timeout} milliseconds.");
                }

                Wait(interval);
            }

            // TODO: Warning if more than one result is found
            return recognitionResult.First();
        }

        /// <summary>
        /// Waits for a specific text to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeout">in milliseconds</param>
        public Rectangle WaitFor(string target, int timeout = 5000)
        {
            return WaitFor(ocrEngine, target, timeout);
        }

        /// <summary>
        /// Waits for a specific image to appear on the screen.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public Rectangle WaitFor(Bitmap image, int timeout = 5000, float threshold = 0.9f) //TODO When you wait for an image, you should not use the path directly and not the bitmap or maybe just a database id
        {
            ((ImgEngine)imgEngine).Threshold = threshold;
            return WaitFor(imgEngine, image, timeout);
        }

        /// <summary>
        /// Waits for a specific text and image to appear on the screen.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="imagePath">The path of the png image</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Rectangle WaitFor(string text, string imagePath, int timeout = 5000) 
        {
            var screenElement = new ScreenElement();
            screenElement.Texts.Add(text);
            var img = new Bitmap(imagePath);
            screenElement.Images.Add(img);

            return WaitFor(screenElement, timeout);
        }

        /// <summary>
        /// Waits for a specific screen element to appear on the screen.
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="timeout">The maximum time to wait for</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public Rectangle WaitFor(ScreenElement elt, int timeout = 5000) //TODO : Do the search in parallel asynchronously
        {
            DateTime start = DateTime.Now;
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area = Rectangle.Empty;
            bool elementFound = false;

            // Wait for either the image or the text to appear on the screen
            while (!elementFound)
            {
                if (!elt.Boxes.Any())
                {
                    foreach (var img in elt.Images)
                    {
                        IEnumerable<Rectangle> recognitionResult;
                        if ((recognitionResult = imgEngine.Find(_screen.CaptureScreen(), img)).Any())
                        {
                            elementFound = true; // Image found
                            area = recognitionResult.First(); // Get the first match
                        }

                        if (elementFound)
                            break; // If image was found, no need to check for text
                    }

                    // Check for the text
                    foreach (var text in elt.Texts)
                    {
                        IEnumerable<Rectangle> recognitionResult;
                        if ((recognitionResult = ocrEngine.Find(_screen.CaptureScreen(), text)).Any())
                        {
                            elementFound = true; // Text found
                            area = recognitionResult.First(); // Get the first match
                        }

                        if (elementFound)
                            break; // If image was found, no need to check for text
                    }

                    // Check if timeout has been reached
                    if (!elementFound && DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
                    {
                        throw new TimeoutException($"The element was not found within the timeout period of {timeout} milliseconds.");
                    }
                }
                else
                {
                    foreach (var box in elt.Boxes)
                    {
                        foreach (var img in elt.Images)
                        {
                            IEnumerable<Rectangle> recognitionResult;
                            if ((recognitionResult = imgEngine.Find(_screen.CaptureScreen(), img)).Any())
                            {
                                elementFound = true; // Image found
                                area = recognitionResult.First(); // Get the first match
                            }

                            if (elementFound)
                                break; // If image was found, no need to check for text
                        }

                        // Check for the text
                        foreach (var text in elt.Texts)
                        {
                            IEnumerable<Rectangle> recognitionResult;
                            if ((recognitionResult = ocrEngine.Find(_screen.CaptureScreen(), text)).Any())
                            {
                                elementFound = true; // Text found
                                area = recognitionResult.First(); // Get the first match
                            }

                            if (elementFound)
                                break; // If image was found, no need to check for text
                        }

                        // Check if timeout has been reached
                        if (!elementFound && DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
                        {
                            throw new TimeoutException($"The element was not found within the timeout period of {timeout} milliseconds.");
                        }
                    }
                }

                Wait(interval);
            }

            // Return the center point of the found area
            return area;
        }

        /// <summary>
        /// Waits for any of the specified screen elements to appear on the screen.
        /// </summary>
        /// <param name="elts"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public Rectangle WaitFor(ScreenElement[] elts, int timeout = 5000) //TODO erase if not useful or implement sth to return witch element appeared
        {
            var start = DateTime.Now;
            while (DateTime.Now.Subtract(start).TotalMilliseconds >= timeout)
            {
                foreach (var elt in elts)
                {
                    try
                    {
                        return WaitFor(elt, 0);
                    }
                    catch (TimeoutException)
                    {
                        // Ignore and continue to the next element
                    }
                }
            }

            throw new TimeoutException($"None of the elements were found within the timeout period of {timeout} milliseconds.");
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
        /// Closes the application.
        /// </summary>
        public void Close()
        {
            var processes = Process.GetProcessesByName(ProcessName);
            foreach (var process in processes)
            {
                if(!process.CloseMainWindow()) // Close the main window of the process
                    process.Kill();
               
                process.WaitForExit();
            }
        }
    }
}
