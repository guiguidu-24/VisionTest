using System.Resources;
using VisionTest.Core.Input;

namespace VisionTest.Tests.Core.Input;

[TestFixture]
public class ScreenTests
{
    [Test]
    public void GetScreen_ShouldCaptureFullScreenWithCorrectDimensions()
    {
        int expectedWidth = int.Parse(TestResources.ScreenWidth ?? throw new FileFormatException("The value with the key ScreenWidth is empty in the file TestResources.resx"));
        int expectedHeight = int.Parse(TestResources.ScreenHeight ?? throw new FileFormatException("The value with the key ScreenHeight is empty in the file TestResources.resx"));

        // Act
        var screenshot = new Screen().CaptureScreen();

        // Assert  
        Assert.IsNotNull(screenshot, "The screenshot should not be null.");
        Assert.That(screenshot.Width, Is.EqualTo(expectedWidth), $"The screenshot width should be {expectedWidth}.");
        Assert.That(screenshot.Height, Is.EqualTo(expectedHeight), $"The screenshot height should be {expectedHeight}.");
    }

    [Test]
    public void Screen_ShouldHaveCorrectBounds()
    {
        // Load expected dimensions from TestResources.resx
        var widthString = TestResources.ScreenWidth;
        var heightString = TestResources.ScreenHeight;

        Assert.Multiple(() =>
        {
            Assert.That(widthString, Is.Not.Null.And.Not.Empty, "'ScreenWidth' value in resources is empty or null.");
            Assert.That(heightString, Is.Not.Null.And.Not.Empty, "'ScreenHeight' value in resources is empty or null.");
        });

        var expectedWidth = int.Parse(widthString);
        var expectedHeight = int.Parse(heightString);

        // Verify screen dimensions
        Assert.Multiple(() =>
        {
            Assert.That(new Screen().ScreenSize.Width, Is.EqualTo(expectedWidth), "The screen width does not match the expected value.");
            Assert.That(new Screen().ScreenSize.Height, Is.EqualTo(expectedHeight), "The screen height does not match the expected value.");
        });
    }

    [Test]
    public void GetScaleFactor_ShouldMatchResourceValue()
    {
        // Load the expected value from TestResources.resx  
        var scaleFactorString = TestResources.ScreenScale;
        Assert.That(scaleFactorString, Is.Not.Null.And.Not.Empty, "'ScreenScale' value in resources is empty or null.");
        var expectedScaleFactor = float.Parse(scaleFactorString.TrimEnd('%')) / 100;

        // Call the method and verify the value  
        var actualScaleFactor = new Screen().ScaleFactor;
        Assert.That(actualScaleFactor, Is.EqualTo(expectedScaleFactor), "The returned scale factor does not match the expected value in resources.");
    }
}
