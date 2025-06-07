using OpenQA.Selenium;


namespace VisionTest.Core.Utils
{
    public static class WebElementExtension
    {
        /// <summary>
        /// Converts a WebElement to a Rectangle based on its position and size.
        /// </summary>
        /// <param name="webElement"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static Rectangle ToRectangle(this IWebElement webElement, IWebDriver driver)
        {
            var size = webElement.Size;
            return new Rectangle(driver.ScreenPosition(webElement), new Size((int)(size.Width * Input.Screen.ScaleFactor), (int)(size.Height * Input.Screen.ScaleFactor)));
        }
    }
}
