using OpenQA.Selenium;

namespace VisionTest.Core.Utils
{
    public static class WebDriverExtension
    {
        /// <summary>
        /// Gets the screen position of a WebElement in the browser window.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Point ScreenPosition(this IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var result = js.ExecuteScript(@"
                const rect = arguments[0].getBoundingClientRect();
                return {
                    x: rect.left + window.screenX + window.outerWidth - window.innerWidth,
                    y: rect.top + window.screenY + window.outerHeight - window.innerHeight
                };
            ", element);

            if (result is not Dictionary<string, object> position)
                throw new InvalidOperationException("Script did not return expected dictionary.");

            return new Point(
                (int) (Convert.ToInt32(position["x"]) * Input.Screen.ScaleFactor),
                (int) (Convert.ToInt32(position["y"]) * Input.Screen.ScaleFactor)
            );
        }
    }
}
