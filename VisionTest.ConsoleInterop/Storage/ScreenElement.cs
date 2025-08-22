using System.Drawing;

namespace VisionTest.ConsoleInterop.Storage;


public class ScreenElement
{
    public string Id { get; set; } = String.Empty;

    public List<Rectangle> Boxes { get; } = new List<Rectangle>();

    public List<Bitmap> Images { get; } = new List<Bitmap>();

    public List<string> Texts { get; } = new List<string>();
}
