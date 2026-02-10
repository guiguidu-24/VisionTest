namespace VisionTest.Core.Recognition;

public record class OcrOptions //TODO: Check if record class default parameters are the good ones
{
    public string WhiteListChar { get; private set; }
    public IEnumerable<string> WordList { get; }
    public Language Lang { get; }
    public PageSegmentationMode PSM { get; set; }
    public OcrEngineMode OEM { get; set; }
    public string BlackListChar { get; set; }
    public bool UseDictionnary { get; }
    public string RegexPattern { get; }


    public OcrOptions(
        string whiteListChar = "",
        string blackListChar = "",
        IEnumerable<string>? wordList = null,
        Language lang = Language.English,
        PageSegmentationMode psm = PageSegmentationMode.Auto,
        OcrEngineMode oem = OcrEngineMode.Auto,
        bool useDictionnary = true, 
        string regexPattern = ""
        )
    {
        if (!string.IsNullOrEmpty(whiteListChar) && !string.IsNullOrEmpty(blackListChar))
        {
            throw new ArgumentException("Cannot specify both a whitelist and a blacklist simultaneously.");
        }

        WhiteListChar = whiteListChar;
        WordList = wordList ?? [];
        Lang = lang;
        PSM = psm;
        OEM = oem;
        BlackListChar = blackListChar;
        UseDictionnary = useDictionnary;
        RegexPattern = regexPattern;
    }

    internal string GetMergedWhiteList(string text)
    {
        if (string.IsNullOrEmpty(text))
            return WhiteListChar;

        // Convert current whitelist to HashSet for efficient lookups
        var existingChars = new HashSet<char>(WhiteListChar);

        // Add new unique characters from the text
        foreach (char c in text)
        {
            if (!existingChars.Contains(c))
                existingChars.Add(c);
        }

        // Convert back to string and update CharWhiteList
        return new string(existingChars.ToArray());
    }

    internal string GetMergedBlackList(string text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(BlackListChar))
            return BlackListChar;

        // Convert target text to HashSet for efficient lookup
        var targetChars = new HashSet<char>(text);

        // Filter the blacklist to remove any characters that appear in the target text
        var resultChars = BlackListChar.Where(c => !targetChars.Contains(c)).ToArray();

        return new string(resultChars);
    }
}
