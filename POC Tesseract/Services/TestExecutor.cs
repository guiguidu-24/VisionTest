using Core.Models;
using Core.Recognition;
using Core.UserInterface;
using System.Diagnostics;
using Core.Input;

/*
Responsibilities of TestExecutor

✅ Does:

    Encapsulates coordination logic

    Abstracts direct engine and input use

    Provides high-level actions: "click on this element", "type into this box"

🚫 Does not:

    Manage databases (use ScreenElementService for that)

    Do raw image/ocr logic (delegates that to engines)

    Directly interact with WPF/Visual Studio UI
*/

namespace Core.Services
{
    //TODO add all the interfaces
    //TODO clean this class
    //TODO complete the other classes 
    public class TestExecutor
    {
        private readonly IMouse _mouse = new Mouse();
        private readonly IScreen _screen = new Input.Screen();
        private OCREngine ocrEngine;
        private ImgEngine imgEngine;
        private ProcessStartInfo processStartInfo;
        private string processName;

        public TestExecutor(string appPath)
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

            processName = string.Empty;
        }

        public TestExecutor(string appPath, string[] args) : this(appPath)
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
            processName = processInst.ProcessName;
        }

        /// <summary>
        /// Simulates a mouse click at the specified screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Click(Point point)
        {
            _mouse.MoveTo((int)(point.X / _screen.ScaleFactor), (int)(point.Y / _screen.ScaleFactor));
            _mouse.LeftClick();
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
            while (!ocrEngine.Find(_screen.CaptureScreen(), text, out area))
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
        public Point WaitFor(Bitmap image, int timeout = 5000, float threshold = 0.9f) //TODO When you wait for an image, you should not use the path directly and not the bitmap or maybe just a database id
        {
            DateTime start = DateTime.Now;
            const int interval = 100; // Check every 100 milliseconds
            Rectangle area;

            // Wait for the text to appear on the screen
            while (!imgEngine.Find(_screen.CaptureScreen(), image, out area, threshold: threshold))
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
            bool elementFound = false;

            // Wait for either the image or the text to appear on the screen
            while (!elementFound)
            {
                if (elt.Boxes.Count == 0)
                {
                    foreach (var img in elt.Images)
                    {
                        if (imgEngine.Find(_screen.CaptureScreen(), img, out area))
                            elementFound = true; // Image found

                        if (elementFound)
                            break; // If image was found, no need to check for text
                    }

                    // Check for the text
                    foreach (var text in elt.Texts)
                    {
                        if (ocrEngine.Find(_screen.CaptureScreen(), text, out area))
                            elementFound = true; // Text found

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
                            if (imgEngine.Find(_screen.CaptureScreen(), img, box, out area))
                                elementFound = true; // Image found

                            if (elementFound)
                                break; // If image was found, no need to check for text
                        }

                        // Check for the text
                        foreach (var text in elt.Texts)
                        {
                            if (ocrEngine.Find(_screen.CaptureScreen(), text, box, out area))
                                elementFound = true; // Text found

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
            return new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        }

        /// <summary>
        /// Waits for any of the specified screen elements to appear on the screen.
        /// </summary>
        /// <param name="elts"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public Point WaitFor(ScreenElement[] elts, int timeout = 5000) //TODO erase if not useful or implement sth to return witch element appeared
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

    }
}
