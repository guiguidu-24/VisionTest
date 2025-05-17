using OpenQA.Selenium;

namespace Core.Models
{
    public class ScreenElement //TODO : an element can have multiple images of reference and image treatment settings for each one
    {
        public IEnumerable<Rectangle> Boxes { get;  set; } 
        public IEnumerable<Bitmap> Images { get;  set; }
        public IEnumerable<By> Locators { get;  set; }
        public IEnumerable<string> Texts { get;  set; } 
    }
}
