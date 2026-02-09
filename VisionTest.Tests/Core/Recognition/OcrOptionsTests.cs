using VisionTest.Core.Recognition;

namespace VisionTest.Tests.Core.Recognition;

[TestFixture]
public class OcrOptionsTests
{
    [Test]
    public void DefaultConstructor_ShouldSetAllDefaultValues()
    {
        // Act
        var options = new OcrOptions();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "));
            Assert.That(options.WordList, Is.Not.Null);
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void DefaultConstructor_ShouldInitializeWordWhiteListAsEmptyCollection()
    {
        // Act
        var options = new OcrOptions();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WordList, Is.Not.Null);
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.WordList.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void DefaultConstructor_ShouldBeConsistentAcrossInstances()
    {
        // Act
        var options1 = new OcrOptions();
        var options2 = new OcrOptions();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options1.WhiteListChar, Is.EqualTo(options2.WhiteListChar));
            Assert.That(options1.LTSMOnly, Is.EqualTo(options2.LTSMOnly));
            Assert.That(options1.Lang, Is.EqualTo(options2.Lang));
            Assert.That(options1.UseThresholdFilter, Is.EqualTo(options2.UseThresholdFilter));
            Assert.That(options1.ImproveDPI, Is.EqualTo(options2.ImproveDPI));
        }
    }

    [Test]
    public void DefaultConstructor_WhiteListCharShouldContainExpectedCharacters()
    {
        // Act
        var options = new OcrOptions();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Does.Contain("A"));
            Assert.That(options.WhiteListChar, Does.Contain("Z"));
            Assert.That(options.WhiteListChar, Does.Contain("a"));
            Assert.That(options.WhiteListChar, Does.Contain("z"));
            Assert.That(options.WhiteListChar, Does.Contain(" "));
        }
    }

    [Test]
    public void Constructor_WithCustomWhiteListChar_ShouldSetProperty()
    {
        // Arrange
        string customWhiteList = "0123456789";

        // Act
        var options = new OcrOptions(whiteListChar: customWhiteList);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo(customWhiteList));
            Assert.That(options.LTSMOnly, Is.True); // Other defaults preserved
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
        }
    }

    [Test]
    public void Constructor_WithEmptyWhiteListChar_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(whiteListChar: "");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo(""));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
        }
    }

    [Test]
    public void Constructor_WithNullWhiteListChar_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(whiteListChar: null!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.Null);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
        }
    }

    [Test]
    public void Constructor_WithCustomWhiteListChar_ShouldPreserveOtherDefaults()
    {
        // Act
        var options = new OcrOptions(whiteListChar: "ABC123");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithWordWhiteList_ShouldSetProperty()
    {
        // Arrange
        var words = new List<string> { "HELLO", "WORLD", "TEST" };

        // Act
        var options = new OcrOptions(wordList: words);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WordList, Is.Not.Null);
            Assert.That(options.WordList, Is.EqualTo(words));
            Assert.That(options.WordList.Count(), Is.EqualTo(3));
            Assert.That(options.WordList, Contains.Item("HELLO"));
            Assert.That(options.WordList, Contains.Item("WORLD"));
            Assert.That(options.WordList, Contains.Item("TEST"));
        }
    }

    [Test]
    public void Constructor_WithEmptyWordWhiteList_ShouldSetProperty()
    {
        // Arrange
        var emptyWords = new List<string>();

        // Act
        var options = new OcrOptions(wordList: emptyWords);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WordList, Is.Not.Null);
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.WordList.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void Constructor_WithNullWordWhiteList_ShouldInitializeAsEmpty()
    {
        // Act
        var options = new OcrOptions(wordList: null);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WordList, Is.Not.Null);
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.WordList.Count(), Is.EqualTo(0));
        }
    }

    [Test]
    public void Constructor_WithWordWhiteList_ShouldPreserveOtherDefaults()
    {
        // Arrange
        var words = new[] { "TARGET" };

        // Act
        var options = new OcrOptions(wordList: words);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithLTSMOnlyTrue_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(lTSMOnly: true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithLTSMOnlyFalse_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(lTSMOnly: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.LTSMOnly, Is.False);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithLTSMOnly_ShouldPreserveOtherDefaults()
    {
        // Act
        var options = new OcrOptions(lTSMOnly: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "));
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithLTSMOnlyValue_ShouldOnlyAffectThatProperty()
    {
        // Act
        var optionsTrue = new OcrOptions(lTSMOnly: true);
        var optionsFalse = new OcrOptions(lTSMOnly: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(optionsTrue.LTSMOnly, Is.True);
            Assert.That(optionsFalse.LTSMOnly, Is.False);
            Assert.That(optionsTrue.Lang, Is.EqualTo(optionsFalse.Lang));
            Assert.That(optionsTrue.UseThresholdFilter, Is.EqualTo(optionsFalse.UseThresholdFilter));
        }
    }

    [Test]
    public void Constructor_WithLanguageEnglish_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(lang: Language.English);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithLanguageFrench_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(lang: Language.French);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.Lang, Is.EqualTo(Language.French));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithLanguage_ShouldPreserveOtherDefaults()
    {
        // Act
        var options = new OcrOptions(lang: Language.French);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "));
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithDifferentLanguages_ShouldOnlyAffectLanguageProperty()
    {
        // Act
        var englishOptions = new OcrOptions(lang: Language.English);
        var frenchOptions = new OcrOptions(lang: Language.French);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(englishOptions.Lang, Is.EqualTo(Language.English));
            Assert.That(frenchOptions.Lang, Is.EqualTo(Language.French));
            Assert.That(englishOptions.LTSMOnly, Is.EqualTo(frenchOptions.LTSMOnly));
            Assert.That(englishOptions.UseThresholdFilter, Is.EqualTo(frenchOptions.UseThresholdFilter));
        }
    }

    [Test]
    public void Constructor_WithUseThresholdFilterTrue_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(useThresholdFilter: true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.UseThresholdFilter, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithUseThresholdFilterFalse_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(useThresholdFilter: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.UseThresholdFilter, Is.False);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithUseThresholdFilter_ShouldPreserveOtherDefaults()
    {
        // Act
        var options = new OcrOptions(useThresholdFilter: true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "));
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.ImproveDPI, Is.False);
        }
    }

    [Test]
    public void Constructor_WithUseThresholdFilterValue_ShouldOnlyAffectThatProperty()
    {
        // Act
        var optionsTrue = new OcrOptions(useThresholdFilter: true);
        var optionsFalse = new OcrOptions(useThresholdFilter: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(optionsTrue.UseThresholdFilter, Is.True);
            Assert.That(optionsFalse.UseThresholdFilter, Is.False);
            Assert.That(optionsTrue.Lang, Is.EqualTo(optionsFalse.Lang));
            Assert.That(optionsTrue.LTSMOnly, Is.EqualTo(optionsFalse.LTSMOnly));
        }
    }

    [Test]
    public void Constructor_WithImproveDPITrue_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(improveDPI: true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.ImproveDPI, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.UseThresholdFilter, Is.False);
        }
    }

    [Test]
    public void Constructor_WithImproveDPIFalse_ShouldSetProperty()
    {
        // Act
        var options = new OcrOptions(improveDPI: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.ImproveDPI, Is.False);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.UseThresholdFilter, Is.False);
        }
    }

    [Test]
    public void Constructor_WithImproveDPI_ShouldPreserveOtherDefaults()
    {
        // Act
        var options = new OcrOptions(improveDPI: true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "));
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.Lang, Is.EqualTo(Language.English));
            Assert.That(options.UseThresholdFilter, Is.False);
        }
    }

    [Test]
    public void Constructor_WithImproveDPIValue_ShouldOnlyAffectThatProperty()
    {
        // Act
        var optionsTrue = new OcrOptions(improveDPI: true);
        var optionsFalse = new OcrOptions(improveDPI: false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(optionsTrue.ImproveDPI, Is.True);
            Assert.That(optionsFalse.ImproveDPI, Is.False);
            Assert.That(optionsTrue.Lang, Is.EqualTo(optionsFalse.Lang));
            Assert.That(optionsTrue.LTSMOnly, Is.EqualTo(optionsFalse.LTSMOnly));
        }
    }

    [Test]
    public void Constructor_WithAllCustomParameters_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        string customWhiteList = "0123456789.,-";
        var customWordList = new[] { "NUMBER", "DIGIT", "DECIMAL" };
        bool customLTSMOnly = false;
        var customLang = Language.French;
        bool customUseThresholdFilter = true;
        bool customImproveDPI = true;

        // Act
        var options = new OcrOptions(
            whiteListChar: customWhiteList,
            wordWhiteList: customWordList,
            lTSMOnly: customLTSMOnly,
            lang: customLang,
            useThresholdFilter: customUseThresholdFilter,
            improveDPI: customImproveDPI
        );

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo(customWhiteList));
            Assert.That(options.WordList, Is.EqualTo(customWordList));
            Assert.That(options.LTSMOnly, Is.EqualTo(customLTSMOnly));
            Assert.That(options.Lang, Is.EqualTo(customLang));
            Assert.That(options.UseThresholdFilter, Is.EqualTo(customUseThresholdFilter));
            Assert.That(options.ImproveDPI, Is.EqualTo(customImproveDPI));
        }
    }

    [Test]
    public void Constructor_WithPartialCustomParameters_ShouldSetSpecifiedAndKeepDefaults()
    {
        // Act
        var options = new OcrOptions(
            whiteListChar: "NUMBERS123",
            lang: Language.French,
            improveDPI: true
        );

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options.WhiteListChar, Is.EqualTo("NUMBERS123"));
            Assert.That(options.Lang, Is.EqualTo(Language.French));
            Assert.That(options.ImproveDPI, Is.True);
            // Defaults preserved
            Assert.That(options.WordList, Is.Empty);
            Assert.That(options.LTSMOnly, Is.True);
            Assert.That(options.UseThresholdFilter, Is.False);
        }
    }

    [Test]
    public void RecordEquality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var options1 = new OcrOptions(whiteListChar: "ABC", lang: Language.French);
        var options2 = new OcrOptions(whiteListChar: "ABC", lang: Language.French);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options1, Is.EqualTo(options2));
            Assert.That(options1.Equals(options2), Is.True);
            Assert.That(options1 == options2, Is.True);
            Assert.That(options1 != options2, Is.False);
        }
    }

    [Test]
    public void RecordEquality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var options1 = new OcrOptions(lang: Language.English);
        var options2 = new OcrOptions(lang: Language.French);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(options1, Is.Not.EqualTo(options2));
            Assert.That(options1.Equals(options2), Is.False);
            Assert.That(options1 == options2, Is.False);
            Assert.That(options1 != options2, Is.True);
        }
    }
}
