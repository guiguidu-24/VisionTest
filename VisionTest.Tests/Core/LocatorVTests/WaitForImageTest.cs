using System.Drawing;
using System.Reflection;
using VisionTest.Core;
using VisionTest.Core.Input;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
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

    [SetUp]
    public void Setup()
    {
        if (!File.Exists(_bigImagePath))
            throw new FileNotFoundException("Big image not found.", _bigImagePath);

        if (!File.Exists(_smallImagePath))
            throw new FileNotFoundException("Small image not found.", _smallImagePath);

        bigImage = new Bitmap(_bigImagePath);
        smallImage = new Bitmap(_smallImagePath);
    }

    [TearDown]
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
        var locator = new LocatorV(smallImage, screen: screen);

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
    public async Task WaitForAsync_Text_ImagePath_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
    {
        // This test combines text search with image search using LocatorV
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async image =>
            {
                IScreen screen = new ScreenSimulator
                {
                    NextCapture = bigImage
                };
                var textLocator = new SimpleLocatorV(text: "hfulhlsq"); // Some non-existent text
                var imageLocator = new SimpleLocatorV(image: image);
                var locator = new LocatorV([textLocator, imageLocator], screen: screen);
                return await locator.WaitForAsync(TimeSpan.FromSeconds(5));
            },
            smallImage);
    }

    private static async Task WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition<T>(Func<T, Task<Rectangle>> methodUnderTest, T argument)
    {
        int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));
        int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));
        const int tolerance = 10;

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
    }

    [Test]
    public async Task WaitForAsync_Image_ReturnsCorrectPosition()
    {
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async image =>
            {
                IScreen screen = new ScreenSimulator
                {
                    NextCapture = bigImage
                };
                var locator = new LocatorV(image, screen: screen);
                return await locator.WaitForAsync(TimeSpan.FromSeconds(5));
            },
            smallImage);
    }

    [Test]
    public async Task WaitForAsync_SimpleLocatorVArrayConstructor_WithMultipleLocators_ReturnsFirstMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var textLocator = new SimpleLocatorV(text: "nonexistent_text"); // This will not match
        var imageLocator = new SimpleLocatorV(image: smallImage); // This should match
        var simpleLocators = new[] { textLocator, imageLocator };
        var locator = new LocatorV(simpleLocators, screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public async Task WaitForAsync_SimpleLocatorVArrayConstructor_WithSingleLocator_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imageLocator = new SimpleLocatorV(image: smallImage);
        var simpleLocators = new[] { imageLocator };
        var locator = new LocatorV(simpleLocators, screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public void Constructor_SimpleLocatorVArray_WithNullArray_ThrowsArgumentNullException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LocatorV((SimpleLocatorV[])null!, screen));
    }

    [Test]
    public void Constructor_SimpleLocatorVArray_WithEmptyArray_ThrowsArgumentException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var emptyArray = new SimpleLocatorV[0];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new LocatorV(emptyArray, screen));
    }

    [Test]
    public async Task WaitForAsync_SingleSimpleLocatorVConstructor_WithImageLocator_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imageLocator = new SimpleLocatorV(image: smallImage);
        var locator = new LocatorV(imageLocator, screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public void WaitForAsync_SingleSimpleLocatorVConstructor_WithTextLocator_ThrowsTimeoutException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var textLocator = new SimpleLocatorV(text: "nonexistent_text");
        var locator = new LocatorV(textLocator, screen);

        // Act & Assert
        Assert.ThrowsAsync<TimeoutException>(
            async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void WaitForAsync_StringConstructor_WithDefaultOptions_ThrowsTimeoutException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var locator = new LocatorV("nonexistent_text", screen: screen);

        // Act & Assert
        Assert.ThrowsAsync<TimeoutException>(
            async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void WaitForAsync_StringConstructor_WithCustomOcrOptions_ThrowsTimeoutException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var ocrOptions = new OcrOptions(LTSMOnly: false, Lang: Language.English);
        var locator = new LocatorV("nonexistent_text", ocrOptions, screen: screen);

        // Act & Assert
        Assert.ThrowsAsync<TimeoutException>(
            async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void WaitForAsync_StringConstructor_WithRegion_ThrowsTimeoutException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var region = new Rectangle(0, 0, 100, 100);
        var locator = new LocatorV("nonexistent_text", region: region, screen: screen);

        // Act & Assert
        Assert.ThrowsAsync<TimeoutException>(
            async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void WaitForAsync_StringConstructor_WithAllParameters_ThrowsTimeoutException()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var ocrOptions = new OcrOptions(Lang: Language.French);
        var region = new Rectangle(0, 0, 200, 200);
        var locator = new LocatorV("texte_inexistant", ocrOptions, region, screen);

        // Act & Assert
        Assert.ThrowsAsync<TimeoutException>(
            async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public async Task WaitForAsync_BitmapConstructor_WithDefaultOptions_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var locator = new LocatorV(smallImage, screen: screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public async Task WaitForAsync_BitmapConstructor_WithCustomImgOptions_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imgOptions = new ImgOptions(threshold: 0.8f, colorMatch: true);
        var locator = new LocatorV(smallImage, imgOptions, screen: screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public async Task WaitForAsync_BitmapConstructor_WithRegion_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var region = new Rectangle(0, 0, bigImage.Width, bigImage.Height); // Full region
        var locator = new LocatorV(smallImage, region: region, screen: screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public async Task WaitForAsync_BitmapConstructor_WithAllParameters_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imgOptions = new ImgOptions(threshold: 0.9f, colorMatch: false);
        var region = new Rectangle(0, 0, bigImage.Width / 2, bigImage.Height / 2);
        
        // Only test if the small image is in the specified region
        var expectedX = int.Parse(TestResources.bigX ?? "0");
        var expectedY = int.Parse(TestResources.bigY ?? "0");
        
        if (expectedX < region.Width && expectedY < region.Height)
        {
            var locator = new LocatorV(smallImage, imgOptions, region, screen);

            // Act & Assert
            await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
                async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
                smallImage);
        }
        else
        {
            var locator = new LocatorV(smallImage, imgOptions, region, screen);
            // Should timeout since image is outside the region
            Assert.ThrowsAsync<TimeoutException>(
                async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
        }
    }

    [Test]
    public void Constructor_WithNullScreen_UsesDefaultScreen()
    {
        // Arrange & Act
        var locator = new LocatorV(smallImage, screen: null);

        // Assert - should not throw, constructor should work with default screen
        Assert.That(locator, Is.Not.Null);
    }

    [Test]
    public void WaitForAsync_BitmapConstructor_WithVeryLowThreshold_ShouldTimeout() //FIXIT use another image that is not in the big image
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imgOptions = new ImgOptions(threshold: 0.99f, colorMatch: true); // Very high threshold
        using var failTargetImage = new Bitmap(@"images\cottonLike.png"); // An image that definitely won't match
        var locator = new LocatorV(failTargetImage, imgOptions, screen: screen);

        // Act & Assert - Should timeout due to very high threshold
        Assert.ThrowsAsync<TimeoutException>(
            async () => await locator.WaitForAsync(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public async Task WaitForAsync_BitmapConstructor_WithColorMatchFalse_ReturnsMatch()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imgOptions = new ImgOptions(threshold: 0.8f, colorMatch: false); // Grayscale matching
        var locator = new LocatorV(smallImage, imgOptions, screen: screen);

        // Act & Assert
        await WaitFor_Generic_ImageAppearsWithinTimeout_ReturnsCorrectPosition(
            async _ => await locator.WaitForAsync(TimeSpan.FromSeconds(5)),
            smallImage);
    }

    [Test]
    public async Task WaitForAsync_MultipleLocatorsConstructor_FirstOneMatches_ReturnsQuickly()
    {
        // Arrange
        IScreen screen = new ScreenSimulator { NextCapture = bigImage };
        var imageLocator = new SimpleLocatorV(image: smallImage); // This should match first
        var textLocator = new SimpleLocatorV(text: "nonexistent_text"); // This won't match
        var locators = new[] { imageLocator, textLocator };
        var locator = new LocatorV(locators, screen);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await locator.WaitForAsync(TimeSpan.FromSeconds(5));
        stopwatch.Stop();

        // Assert
        Assert.That(result.Width, Is.GreaterThan(0));
        Assert.That(result.Height, Is.GreaterThan(0));
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000), 
            "Should return quickly when first locator matches");
    }
}
