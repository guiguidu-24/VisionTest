using VisionTest.Core.Recognition;

namespace VisionTest.Core.Recognition
{
    public enum Language
    {
        english,
        french
    }
}

public static class LanguageExtensions
{
    public static string ToCode(this Language language)
    {
        return language switch
        {
            Language.english => "eng",
            Language.french => "fra",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}
