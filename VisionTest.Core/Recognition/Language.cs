namespace VisionTest.Core.Recognition;

public enum Language
{
    English,
    French
}

public static class LanguageExtensions
{
    public static string ToCode(this Language language)
    {
        return language switch
        {
            Language.English => "eng",
            Language.French => "fra",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}
