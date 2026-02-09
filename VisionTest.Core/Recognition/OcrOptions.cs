namespace VisionTest.Core.Recognition;

public record class OcrOptions //TODO: Check if record class default parameters are the good ones
{
    public string WhiteListChar { get; }
    public IEnumerable<string> WordWhiteList { get; }
    public bool LTSMOnly { get; }
    public Language Lang { get; }
    public bool UseThresholdFilter { get; }
    public bool ImproveDPI { get; }
    public PageSegmentationMode PSM { get; set; }
    public OcrEngineMode OEM { get; set; }
    public string BlackListChar { get; set; }


    public OcrOptions(
        string whiteListChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ",
        string blackListChar = "",
        IEnumerable<string>? wordWhiteList = null,
        bool lTSMOnly = true,
        Language lang = Language.English,
        bool useThresholdFilter = false,
        bool improveDPI = false,
        PageSegmentationMode psm = PageSegmentationMode.Auto,
        OcrEngineMode oem = OcrEngineMode.Auto
        )
    {
        WhiteListChar = whiteListChar;
        WordWhiteList = wordWhiteList ?? [];
        LTSMOnly = lTSMOnly;
        Lang = lang;
        UseThresholdFilter = useThresholdFilter;
        ImproveDPI = improveDPI;
        PSM = psm;
        OEM = oem;
        BlackListChar = blackListChar;
    }
}
