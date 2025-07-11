namespace VisionTest.Core.Recognition
{
    public record class OCREngineOptions(
        string WhiteListChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ",
        IEnumerable<string> WordWhiteList = null,
        bool LTSMOnly = true,
        Language Lang = Language.english,
        bool UseThresholdFilter = false,
        bool ImproveDPI = false
    )
    {
        public IEnumerable<string> WordWhiteList { get; init; } = WordWhiteList ?? [];
    }
}
