using OpenQA.Selenium;

namespace Core.Models
{
    public class ScreenElement //TODO : an element can have multiple images of reference and image treatment settings for each one
    {
        public List<Rectangle> Boxes { get; private set; } = new List<Rectangle>();
        public List<Bitmap> Images { get; private set; } = new List<Bitmap>();
        public List<By> Locators { get; private set; } = new List<By>();
        public List<string> Texts { get; private set; } = new List<string>();
    }
}
