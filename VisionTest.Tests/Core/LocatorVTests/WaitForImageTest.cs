using System.Drawing;
using System.Reflection;
using VisionTest.Core;
using VisionTest.Core.Input;
using VisionTest.Core.Models;
using VisionTest.Core.Utils;
using VisionTest.Tests.Core.TestHarness;
using WindowsInput;
using WindowsInput.Events;

namespace VisionTest.Tests.Core.LocatorVTests;

[TestFixture]
public class WaitForImageTest
{
    private string paintPath = string.Empty;
    private static string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("The assembly path is null");

    private readonly string _bigImagePath = Path.Combine(assemblyDir, "images/big.png");
    private readonly string _smallImagePath = Path.Combine(assemblyDir, "images/small.png");
    private Bitmap bigImage = null!;
    private Bitmap smallImage = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        if (!File.Exists(_bigImagePath))
            throw new FileNotFoundException("Big image not found.", _bigImagePath);

        if (!File.Exists(_smallImagePath))
            throw new FileNotFoundException("Small image not found.", _smallImagePath);

        bigImage = new Bitmap(_bigImagePath);
        smallImage = new Bitmap(_smallImagePath);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        bigImage.Dispose();
        smallImage.Dispose();
    }



    [Test]
    public async Task WaitFor_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
    {
        int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));
        int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));
        const int tolerance = 10;

        IScreen screen = new ScreenSimulator
        {
            NextCapture = bigImage
        };
        //use LocatorV to find the small image
        var locator = new LocatorV(smallImage, screen:screen);

        try
        {
            // Wait for the small image to appear in the big image  
            Rectangle foundRect = await locator.WaitForAsync(TimeSpan.FromSeconds(5));
            Point foundPosition = foundRect.Center();

            using (Assert.EnterMultipleScope())
            {
                // Assert the position is within the expected bounds with a tolerance
                Assert.That(foundPosition.X, Is.InRange(expectedX - tolerance, expectedX + tolerance),
                    $"X coordinate should be within {tolerance} pixels of {expectedX}.");
                Assert.That(foundPosition.Y, Is.InRange(expectedY - tolerance, expectedY + tolerance),
                    $"Y coordinate should be within {tolerance} pixels of {expectedY}.");
            }
        }
        catch (TimeoutException ex)
        {
            Assert.Fail($"The image was not found within the timeout period. Exception: {ex.Message}");
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
            _smallImagePath);
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
            Arguments = _bigImagePath,
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
            new Bitmap(_smallImagePath));
    }
}
