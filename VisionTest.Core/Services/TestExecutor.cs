using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using System.Diagnostics;
using VisionTest.Core.Input;
using VisionTest.Core.Utils;


namespace VisionTest.Core.Services
{
    public class TestExecutor //TODO : for an image, directly use the name of the image instead of the path
    {
        private const float defaultThreshold = 0.9f; // Default threshold for image recognition
        private const int defaultTimeout = 5000; // Default timeout for waiting for elements

        private readonly IMouse _mouse = new Mouse();
        private readonly IScreen _screen = new Input.Screen();
        private readonly IRecognitionEngine<string> ocrEngine;
        private readonly IRecognitionEngine<Bitmap> imgEngine;
        private ProcessStartInfo? processStartInfo;
        private string? appPath;

        public string ProcessName { get; private set; } = string.Empty;
        public float DefaultThreshold { private get; set; } = 0.9f;
        public TimeSpan DefaultTimeout { private get; set; } = TimeSpan.FromMilliseconds(5000); // Default timeout for waiting for elements

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
            ocrEngine = new OcrEngine("eng");
            imgEngine = new ImgEngine();
            ((ImgEngine)imgEngine).Threshold = defaultThreshold; // Set the default threshold for image recognition
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
        public void Click(string text, int timeout = defaultTimeout)
        {
            Rectangle area = WaitFor(text, timeout);
            Click(area.Center());
        }

        /// <summary>
        /// Simulates a mouse click on a specific image on the screen.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="timeout"></param>
        public void Click(Bitmap image, int timeout = defaultTimeout)
        {
            Rectangle area = WaitFor(image, timeout);
            Click(area.Center());
        }

        public void Click(string text, string imagePath, int timeout = defaultTimeout)
        {
            Rectangle area = WaitFor(text, imagePath, timeout);
            Click(area.Center());
        }

        /// <summary>
        /// Simulates a mouse click on a specific screen element.
        /// </summary>
        /// <param name="elt"></param>
        /// <param name="timeout"></param>
        public void Click(ScreenElement elt, int timeout = defaultTimeout)
        {
            Rectangle area = WaitFor(elt, timeout);
            Click(area.Center());
        }


        private Rectangle WaitFor<TTarget>(IRecognitionEngine<TTarget> engine, TTarget target, int timeout)
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
        public Rectangle WaitFor(string target, int timeout = defaultTimeout)
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
        public Rectangle WaitFor(Bitmap image, int timeout = defaultTimeout, float threshold = 0.9f) //TODO When you wait for an image, you should not use the path directly and not the bitmap or maybe just a database id
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
        public Rectangle WaitFor(string text, string imagePath, int timeout = defaultTimeout)
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
        public Rectangle WaitFor(ScreenElement elt, int timeout = defaultTimeout) //TODO : Do the search in parallel asynchronously
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
        public Rectangle WaitFor(ScreenElement[] elts, int timeout = defaultTimeout) //TODO erase if not useful or implement sth to return witch element appeared
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
                if (!process.CloseMainWindow()) // Close the main window of the process
                    process.Kill();

                process.WaitForExit();
            }
        }


        private async Task<Rectangle?> WaitForAsync<TTarget>(IRecognitionEngine<TTarget> engine, TTarget target, Rectangle? box, CancellationToken cancellationToken) //TODO : use the token to manage the timeout
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Target cannot be null.");
            }

            const int interval = 100; // Check every 100 milliseconds

            Bitmap image;

            // Wait for the text to appear on the screen
            IEnumerable<Rectangle> recognitionResult;
            do
            {
                image = _screen.CaptureScreen();
                if (box.HasValue)
                {
                    image = image.Clone(box.Value, image.PixelFormat); // Crop the image to the specified box if provided
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return null; // Return null if cancellation is requested
                }
                await Task.Delay(interval, cancellationToken);
            }
            while (!(recognitionResult = engine.Find(image, target)).Any());


            // TODO: Warning if more than one result is found
            return recognitionResult.First();
            //return (true, area);
        }

        /// <summary>
        /// Waits for a specific screen element to appear on the screen asynchronously.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<Rectangle?> WaitforAsync(ScreenElement target)
        {
            return await WaitForAsync(target, DefaultTimeout);
        }

        /// <summary>
        /// Waits for a specific screen element to appear on the screen asynchronously.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeout">After the timeout, the Task completes</param>
        /// <returns>The area around the target if found, null if not found</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Rectangle?> WaitForAsync(ScreenElement target, TimeSpan timeout)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "ScreenElement cannot be null.");
            }
            if (target.Images.Count + target.Texts.Count == 0)
            {
                throw new ArgumentException("ScreenElement must contain at least one image or text to search for.", nameof(target));
            }
            if (target.Boxes.Any() && !(target.Texts.Count == target.Boxes.Count || target.Images.Count == target.Boxes.Count))
            {
                throw new ArgumentException("The number of boxes must match the number of images or texts in the ScreenElement.", nameof(target));
            }


            bool findInBoxes = target.Boxes.Count != 0;
            var cts = new CancellationTokenSource(timeout);
            var tasks = new Task<Rectangle?>[target.Images.Count + target.Texts.Count];

            for (int i = 0; i < target.Texts.Count; i++)
            {
                var text = target.Texts[i];
                tasks[i] = Task.Run(async () =>
                {
                    return await WaitForAsync(ocrEngine, text, findInBoxes ? target.Boxes[i] : null, cts.Token);
                }, cts.Token);
            }

            for (int i = 0; i < target.Images.Count; i++)
            {
                var img = target.Images[i];
                tasks[i + target.Texts.Count] = Task.Run(async () =>
                {
                    return await WaitForAsync(imgEngine, img, findInBoxes ? target.Boxes[i] : null, cts.Token);
                }, cts.Token);
            }

            var taskFinished = await Task.WhenAny(tasks);
            cts.Cancel(); // Cancel all other tasks once one is finished

            return await taskFinished;
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(string target, out Rectangle? area)
        {
            return TryWaitFor(target, out area, DefaultTimeout);
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(string target, out Rectangle? area, TimeSpan timeout)
        {
            var cst = new CancellationTokenSource(timeout);
            area = WaitForAsync(ocrEngine, target, null, cst.Token).Result;
            return area.HasValue;
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(Bitmap target, out Rectangle? area)
        {
            return TryWaitFor(target, out area, DefaultTimeout);
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(Bitmap target, out Rectangle? area, TimeSpan timeout)
        {
            return TryWaitFor(target, out area, timeout, DefaultThreshold);
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(Bitmap target, out Rectangle? area, float threshold)
        {
            return TryWaitFor(target, out area, DefaultTimeout, threshold);
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(Bitmap target, out Rectangle? area, TimeSpan timeout, float threshold)
        {
            ((ImgEngine)imgEngine).Threshold = threshold;
            var cts = new CancellationTokenSource(timeout);
            area = WaitForAsync(imgEngine, target, null, cts.Token).Result;
            return area.HasValue;
        }

        /// <summary>
        /// Tries to wait for specific targets to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(string text, string imagePath, out Rectangle? area)
        {
            var screenElement = new ScreenElement();
            screenElement.Texts.Add(text);
            screenElement.Images.Add(new Bitmap(imagePath));
            return TryWaitFor(screenElement, out area);
        }

        /// <summary>
        /// Tries to wait for a specific target to appear on the screen.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="area">The area is null if not found</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryWaitFor(ScreenElement target, out Rectangle? area)
        {
            area = WaitforAsync(target).Result;
            return area.HasValue;
        }
    }
}
