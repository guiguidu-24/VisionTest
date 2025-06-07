using OpenQA.Selenium;
using System.CodeDom;


namespace VisionTest.Core.Utils
{
    public static class WebElementExtension
    {
        public static Rectangle ToRectangle(this IWebElement webElement, IWebDriver driver)
        {
            var size = webElement.Size;
            return new Rectangle(driver.ScreenPosition(webElement), new Size((int)(size.Width * Input.Screen.ScaleFactor), (int)(size.Height * Input.Screen.ScaleFactor)));
        }
    }
}
