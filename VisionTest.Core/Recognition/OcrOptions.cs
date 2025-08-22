namespace VisionTest.Core.Recognition;

public record class OcrOptions( //TODO: Check if record class default parameters are the good ones
    string WhiteListChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ",
    IEnumerable<string>? WordWhiteList = null,
    bool LTSMOnly = true,
    Language Lang = Language.English,
    bool UseThresholdFilter = false,
    bool ImproveDPI = false
)
{
    public IEnumerable<string> WordWhiteList { get; init; } = WordWhiteList ?? [];
}
