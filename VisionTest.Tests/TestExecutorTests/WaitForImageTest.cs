using System.Drawing;
using VisionTest.Core.Input;
using VisionTest.Core.Models;
using VisionTest.Core.Services;
using VisionTest.Core.Utils;
using WindowsInput;
using WindowsInput.Events;


namespace VisionTest.Tests.TestExecutorTests
{
    [TestFixture]
    internal class WaitForImageTest
    {
        private TestExecutor executor;
        private string paintPath = string.Empty;// TestResources.PaintPath;// @"C:\Windows\System32\mspaint.exe";
        private string bigImagePath = @"..\..\..\images\big.png";
        private string smallImagePath = @"..\..\..\images\small.png";

        [SetUp]
        public void Setup()
        {
            
            //var resourceManager = new ResourceManager("TestTesseract.TestResources", typeof(AppliTestPaint).Assembly);
            var stringPaintPath = TestResources.PaintPath; // resourceManager.GetString("PaintPath");
            Assert.That(stringPaintPath, Is.Not.Null.And.Not.Empty, "'PaintPath' value in resources is empty or null.");
            paintPath = stringPaintPath;

            // Ensure the paths exist  
            if (paintPath.Contains(".exe") && !File.Exists(paintPath))
                throw new FileNotFoundException("Paint application not found.", paintPath);

            if (!File.Exists(bigImagePath))
                throw new FileNotFoundException("Big image not found.", bigImagePath);

            if (!File.Exists(smallImagePath))
                throw new FileNotFoundException("Small image not found.", smallImagePath);

            executor = new TestExecutor();
            executor.AppPath = paintPath;
            executor.Arguments = [bigImagePath];
        }

        [Test]
        public async Task WaitFor_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null.")); //313; //309
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null.")); //722; //577
            const int tolerance = 10;

            // Open Paint with the big image  
            executor.Open();

            await Task.Delay(1000);
            Simulate.Events().Click(WindowsInput.Events.KeyCode.F11).Invoke().Wait();
            await Task.Delay(1000); // Wait for the application to maximize


            //var screenshot = new Screen().CaptureScreen();
            //screenshot.Save("C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\TestTesseract\\screenshot.png");

            // Load the small image  
            Bitmap smallImage = new Bitmap(smallImagePath);

            try
            {
                // Wait for the small image to appear in the big image  
                Point foundPosition = executor.WaitFor(smallImage, timeout: 5000).Center();

                // Assert the position is within the expected bounds with a tolerance
                Assert.That(foundPosition.X, Is.InRange(expectedX - tolerance, expectedX + tolerance),
                    $"X coordinate should be within {tolerance} pixels of {expectedX}.");
                Assert.That(foundPosition.Y, Is.InRange(expectedY - tolerance, expectedY + tolerance),
                    $"Y coordinate should be within {tolerance} pixels of {expectedY}.");
            }
            catch (TimeoutException ex)
            {
                Assert.Fail($"The image was not found within the timeout period. Exception: {ex.Message}");
            }
        }

        [Test]
        public void WaitForElement_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));//13;
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));//722;
            const int tolerance = 10;

            // Open Paint with the big image  
            executor.Open();

            executor.Wait(1000); // Wait for the application to open and load the image
            // Maximize the Paint window  
            Simulate.Events().Click(WindowsInput.Events.KeyCode.F11).Invoke().Wait();
            executor.Wait(500); // Wait for the application to maximize
            var screenshot = new Screen().CaptureScreen();            
            //screen.Save(@"E:\Projects data\POC_Tesseract\TestTesseract\screenshot.png");

            // Load the small image  
            Bitmap smallImage = new Bitmap(smallImagePath);
            var targetElement = new ScreenElement();
            targetElement.Images.Add(smallImage);

            try
            {
                // Wait for the small image to appear in the big image  
                Point foundPosition = executor.WaitFor(targetElement, timeout: 5000).Center();

                // Assert the position is within the expected bounds with a tolerance
                Assert.That(foundPosition.X, Is.InRange(expectedX - tolerance, expectedX + tolerance),
                    $"X coordinate should be within {tolerance} pixels of {expectedX}.");
                Assert.That(foundPosition.Y, Is.InRange(expectedY - tolerance, expectedY + tolerance),
                    $"Y coordinate should be within {tolerance} pixels of {expectedY}.");
            }
            catch (TimeoutException ex)
            {
                Assert.Fail($"The image was not found within the timeout period. Exception: {ex.Message}");
            }
            
        }

        [Test]
        public async Task WaitFor_Text_ImagePath_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(image => executor.WaitFor("hfulhlsq", image), smallImagePath);
        }

        private async Task WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition<T>(Func<T, Rectangle> methodUnderTest, T argument)
        {
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null.")); //313; //309
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null.")); //722; //577
            const int tolerance = 10;

            // Open Paint with the big image  
            executor.Open();

            await Task.Delay(1000);
            Simulate.Events().Click(WindowsInput.Events.KeyCode.F11).Invoke().Wait();
            await Task.Delay(1000); // Wait for the application to maximize


            //var screenshot = new Screen().CaptureScreen();
            //screenshot.Save("C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\TestTesseract\\screenshot.png");

            // Load the small image  
            Bitmap smallImage = new Bitmap(smallImagePath);

            try
            {
                // Wait for the small image to appear in the big image  
                Point foundPosition = methodUnderTest(argument).Center();

                // Assert the position is within the expected bounds with a tolerance
                Assert.That(foundPosition.X, Is.InRange(expectedX - tolerance, expectedX + tolerance),
                    $"X coordinate should be within {tolerance} pixels of {expectedX}.");
                Assert.That(foundPosition.Y, Is.InRange(expectedY - tolerance, expectedY + tolerance),
                    $"Y coordinate should be within {tolerance} pixels of {expectedY}.");
            }
            catch (TimeoutException ex)
            {
                Assert.Fail($"The image was not found within the timeout period. Exception: {ex.Message}");
            }
        }

        [Test]
        public async Task WaitForAsync_ScreenElement_ReturnsCorrectPosition()
        {
            await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
                image => executor.WaitforAsync(new ScreenElement { Images = { image } }).Result ?? Rectangle.Empty,
                new Bitmap(smallImagePath));
        }

        [TearDown]
        public void TearDown()
        {
            Simulate.Events().ClickChord(WindowsInput.Events.KeyCode.Alt, KeyCode.F4).Invoke().Wait();
        }
    }
}
