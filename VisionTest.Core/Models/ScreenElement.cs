using NUnit.Framework;

namespace VisionTest.Core.Models
{
    public class ScreenElement //TODO : an element can have multiple images of reference and image treatment settings for each one
    {
        public string Id { get; set; } = String.Empty;
        public List<Rectangle> Boxes { get; } = new List<Rectangle>();
        public List<Bitmap> Images { get; } = new List<Bitmap>();
        public List<string> Texts { get; } = new List<string>();
        //TODO add Selenium capabilities

    }
}
