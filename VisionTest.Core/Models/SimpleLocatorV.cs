using VisionTest.Core.Recognition;

namespace VisionTest.Core.Models;

public record class SimpleLocatorV
{
    public Bitmap? Image { get; }
    public string? Text { get; }
    public Rectangle? Region { get; }
    public OcrOptions OcrOption { get; }
    public ImgOptions ImgOption { get; }

    public SimpleLocatorV(Bitmap? image = null, string? text = null, Rectangle? region = null, OcrOptions? ocrOption = null, ImgOptions? imgOption = null)
    {
        if (text is not null && image is not null)
            throw new ArgumentException("SimpleLocatorV cannot contain both Image and Text.");

        if (image is null && text is null)
            throw new ArgumentNullException("SimpleLocatorV must contain at least one image or text to search for.");

        if (text is null && ocrOption is not null)
            throw new ArgumentException("OCROption can only be used with Text.");

        if (image is null && imgOption is not null)
            throw new ArgumentException("ImgOption can only be used with Image.");

        Image = image;
        Text = text;
        Region = region;

        // Set required options based on which is used
        if (Text is not null)
        {
            OcrOption = ocrOption ?? new OcrOptions();
            ImgOption = new ImgOptions(); // Provide a default, but not used
        }
        else
        {
            ImgOption = imgOption ?? new ImgOptions();
            OcrOption = new OcrOptions(); // Provide a default, but not used
        }
    }
}