using VisionTest.Core.Recognition;

namespace VisionTest.Tests.Core.Recognition;

[TestFixture]
public class ImgOptionsTests
{
    [Test]
    public void Constructor_WithThresholdAndColorMatch_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        float expectedThreshold = 0.75f;
        bool expectedColorMatch = true;

        // Act
        var options = new ImgOptions(expectedThreshold, expectedColorMatch);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.Threshold, Is.EqualTo(expectedThreshold));
            Assert.That(options.ColorMatch, Is.EqualTo(expectedColorMatch));
        }
    }

    [Test]
    public void Constructor_WithThresholdAndColorMatchFalse_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        float expectedThreshold = 0.85f;
        bool expectedColorMatch = false;

        // Act
        var options = new ImgOptions(expectedThreshold, expectedColorMatch);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.Threshold, Is.EqualTo(expectedThreshold));
            Assert.That(options.ColorMatch, Is.EqualTo(expectedColorMatch));
        }
    }

    [Test]
    public void Constructor_WithThresholdAndColorMatch_ShouldThrowForInvalidThreshold()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImgOptions(-0.1f, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ImgOptions(1.1f, false));
    }

    [Test]
    public void Constructor_WithThresholdAndColorMatch_ShouldAcceptBoundaryValues()
    {
        // Arrange & Act
        var options1 = new ImgOptions(0.0f, true);
        var options2 = new ImgOptions(1.0f, false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options1.Threshold, Is.EqualTo(0.0f));
            Assert.That(options1.ColorMatch, Is.True);
            Assert.That(options2.Threshold, Is.EqualTo(1.0f));
            Assert.That(options2.ColorMatch, Is.False);
        }
    }

    [Test]
    public void DefaultConstructor_ShouldSetDefaultValues()
    {
        // Act
        var options = new ImgOptions();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.Threshold, Is.EqualTo(0.9f));
            Assert.That(options.ColorMatch, Is.True);
        }
    }

    [Test]
    public void DefaultConstructor_ShouldBeConsistentAcrossInstances()
    {
        // Act
        var options1 = new ImgOptions();
        var options2 = new ImgOptions();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options1.Threshold, Is.EqualTo(options2.Threshold));
            Assert.That(options1.ColorMatch, Is.EqualTo(options2.ColorMatch));
        }
    }

    [Test]
    public void DefaultConstructor_ShouldCreateValidOptions()
    {
        // Act
        var options = new ImgOptions();

        // Assert
        Assert.That(options.Threshold, Is.GreaterThanOrEqualTo(0.0f));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Threshold, Is.LessThanOrEqualTo(1.0f));
            Assert.That(options.ColorMatch, Is.True);
        }
    }

    [Test]
    public void DefaultConstructor_PropertiesShouldMatchExpectedDefaults()
    {
        // Act
        var options = new ImgOptions();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.Threshold, Is.EqualTo(0.9f), "Default threshold should be 0.9f");
            Assert.That(options.ColorMatch, Is.True, "Default ColorMatch should be true");
        }
    }

    [Test]
    public void Constructor_WithColorMatchOnly_ShouldSetDefaultThreshold()
    {
        // Act
        var optionsTrue = new ImgOptions(true);
        var optionsFalse = new ImgOptions(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(optionsTrue.Threshold, Is.EqualTo(0.9f));
            Assert.That(optionsTrue.ColorMatch, Is.True);
            Assert.That(optionsFalse.Threshold, Is.EqualTo(0.9f));
            Assert.That(optionsFalse.ColorMatch, Is.False);
        }
    }

    [Test]
    public void Constructor_WithColorMatchOnlyTrue_ShouldPreserveColorSetting()
    {
        // Act
        var options = new ImgOptions(true);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.ColorMatch, Is.True);
            Assert.That(options.Threshold, Is.EqualTo(0.9f));
        }
    }

    [Test]
    public void Constructor_WithColorMatchOnlyFalse_ShouldPreserveColorSetting()
    {
        // Act
        var options = new ImgOptions(false);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.ColorMatch, Is.False);
            Assert.That(options.Threshold, Is.EqualTo(0.9f));
        }
    }

    [Test]
    public void Constructor_WithColorMatchOnly_ShouldUseDefaultThresholdValue()
    {
        // Arrange
        const float defaultThreshold = 0.9f;

        // Act
        var optionsWithColorMatch = new ImgOptions(true);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(optionsWithColorMatch.Threshold, Is.EqualTo(defaultThreshold));
            Assert.That(optionsWithColorMatch.ColorMatch, Is.True);
        }
    }

    [Test]
    public void Constructor_WithThresholdOnly_ShouldSetDefaultColorMatch()
    {
        // Arrange
        float customThreshold = 0.65f;

        // Act
        var options = new ImgOptions(customThreshold);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(options.Threshold, Is.EqualTo(customThreshold));
            Assert.That(options.ColorMatch, Is.True);
        }
    }

    [Test]
    public void Constructor_WithThresholdOnly_ShouldValidateThresholdRange()
    {
        // Arrange & Act & Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ImgOptions(-0.5f));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ImgOptions(2.0f));
        }
    }

    [TestCase(0.0f)]
    [TestCase(0.5f)]
    [TestCase(1.0f)]
    public void Constructor_WithThresholdOnly_ShouldAcceptValidThresholds(float customThreshold)
    {
        // Arrange & Act
        var options1 = new ImgOptions(customThreshold);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options1.Threshold, Is.EqualTo(customThreshold));
            Assert.That(options1.ColorMatch, Is.True);
        }
    }

    [Test]
    public void Constructor_WithThresholdOnly_ShouldUseDefaultColorMatchValue()
    {
        // Arrange
        bool defaultColorMatch = true;
        float customThreshold = 0.8f;

        // Act
        var optionsWithThreshold = new ImgOptions(customThreshold);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(optionsWithThreshold.ColorMatch, Is.EqualTo(defaultColorMatch));
            Assert.That(optionsWithThreshold.Threshold, Is.EqualTo(customThreshold));
        }
    }
}
