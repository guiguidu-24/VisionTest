namespace VisionTest.Core.Recognition;

public record struct ImgOptions
{
    public float Threshold { get; }
    public bool ColorMatch { get; }

    public ImgOptions(float threshold, bool colorMatch)
    {
        if (threshold < 0 || threshold > 1)
            throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be between 0 and 1.");
        Threshold = threshold;
        ColorMatch = colorMatch;
    }

    public ImgOptions() : this(0.9f, true) { }
    public ImgOptions(bool colorMatch) : this(0.9f, colorMatch) { }
    public ImgOptions(float threshold): this(threshold, true) { }
}