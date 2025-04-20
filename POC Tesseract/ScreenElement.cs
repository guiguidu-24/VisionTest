using OpenQA.Selenium;

namespace POC_Tesseract
{
    public struct ScreenElement
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
