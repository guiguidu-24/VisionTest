using OpenQA.Selenium;

namespace POC_Tesseract
{
    public struct ScreenElement //TODO : an element can have multiple images of reference and image treatment settings for each one
    {
        public ScreenElement()
        {
            Box = default;
            Image = default;
            Locator = default;
            Text = default;
        }

        public Rectangle Box { get; set; }
        public Bitmap? Image { get; set; }
        public By? Locator { get; set; }
        public string? Text { get; set; }
    }
}
