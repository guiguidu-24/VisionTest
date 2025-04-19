using OpenQA.Selenium;

namespace POC_Tesseract
{
    namespace ScreenElementDetection
    {
        public class ScreenElement
        {
            public Rectangle Box { get; set; }
            public string ReferenceImagePath { get; set; }
            public By Locator { get; set; }
            public string Text { get; set; }

            public ScreenElement(Rectangle box, string referenceImagePath, By locator, string text = "")
            {
                Box = box;
                ReferenceImagePath = referenceImagePath;
                Locator = locator;
                Text = text;
            }
        }
    }

}
