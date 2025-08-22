using System.Drawing;
using VisionTest.Core;
using VisionTest.Core.Input;
using VisionTest.Core.Models;
using VisionTest.Core.Utils;
using WindowsInput;
using WindowsInput.Events;

namespace VisionTest.Tests.Core.Models.LocatorVTests
{
    [TestFixture]
    internal class WaitForImageTest
    {
        private string paintPath = string.Empty;
        private string bigImagePath = @"..\..\..\images\big.png";
        private string smallImagePath = @"..\..\..\images\small.png";

        [SetUp]
        public void Setup()
        {
            var stringPaintPath = TestResources.PaintPath;
            Assert.That(stringPaintPath, Is.Not.Null.And.Not.Empty, "'PaintPath' value in resources is empty or null.");
            paintPath = stringPaintPath;

            // Ensure the paths exist  
            if (paintPath.Contains(".exe") && !File.Exists(paintPath))
                throw new FileNotFoundException("Paint application not found.", paintPath);

            if (!File.Exists(bigImagePath))
                throw new FileNotFoundException("Big image not found.", bigImagePath);

            if (!File.Exists(smallImagePath))
                throw new FileNotFoundException("Small image not found.", smallImagePath);
        }

        [Test]
        public async Task WaitFor_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));
            const int tolerance = 10;

            // Start Paint with the big image manually or assume it's already running
            var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = paintPath,
                Arguments = bigImagePath,
                UseShellExecute = false
            });

            await Task.Delay(1000);
            Simulate.Events().Click(KeyCode.F11).Invoke().Wait();
            await Task.Delay(1000); // Wait for the application to maximize

            try
            {
                // Load the small image and use LocatorV to find it
                Bitmap smallImage = new Bitmap(smallImagePath);
                var locator = new LocatorV(smallImage);

                // Wait for the small image to appear in the big image  
                Rectangle foundRect = await locator.WaitForAsync(TimeSpan.FromSeconds(5));
                Point foundPosition = foundRect.Center();

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
            catch (Exception)
            {
                throw;
            }
            finally
            {
                process?.Kill();
            }
        }

        [Test]
        public async Task WaitForElement_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));
            const int tolerance = 10;

            // Start Paint with the big image
            var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = paintPath,
                Arguments = bigImagePath,
                UseShellExecute = false
            });

            await Task.Delay(1000); // Wait for the application to open and load the image
            // Maximize the Paint window  
            Simulate.Events().Click(KeyCode.F11).Invoke().Wait();
            await Task.Delay(500); // Wait for the application to maximize
            var screenshot = new Screen().CaptureScreen();            

            try
            {
                // Load the small image and use LocatorV to find it
                Bitmap smallImage = new Bitmap(smallImagePath);
                var locator = new LocatorV(smallImage);

                // Wait for the small image to appear in the big image  
                Rectangle foundRect = await locator.WaitForAsync(TimeSpan.FromSeconds(5));
                Point foundPosition = foundRect.Center();

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
            finally
            {
                process?.Kill();
            }
        }

        [Test]
        public async Task WaitFor_Text_ImagePath_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            // This test combines text search with image search using LocatorV
            await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
                async imagePath => 
                {
                    var image = new Bitmap(imagePath);
                    var textLocator = new SimpleLocatorV(text: "hfulhlsq"); // Some non-existent text
                    var imageLocator = new SimpleLocatorV(image: image);
                    var locator = new LocatorV(new[] { textLocator, imageLocator });
                    return await locator.WaitForAsync(TimeSpan.FromSeconds(5));
                }, 
                smallImagePath);
        }

        private async Task WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition<T>(Func<T, Task<Rectangle>> methodUnderTest, T argument)
        {
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));
            const int tolerance = 10;

            // Start Paint with the big image
            var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = paintPath,
                Arguments = bigImagePath,
                UseShellExecute = false
            });

            await Task.Delay(1000);
            Simulate.Events().Click(KeyCode.F11).Invoke().Wait();
            await Task.Delay(1000); // Wait for the application to maximize

            try
            {
                // Wait for the small image to appear in the big image  
                Rectangle foundRect = await methodUnderTest(argument);
                Point foundPosition = foundRect.Center();

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
            finally
            {
                process?.Kill();
            }
        }

        [Test]
        public async Task WaitForAsync_ScreenElement_ReturnsCorrectPosition()
        {
            await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
                async image => 
                {
                    var locator = new LocatorV(image);
                    return await locator.WaitForAsync(TimeSpan.FromSeconds(5));
                },
                new Bitmap(smallImagePath));
        }
    }
}
