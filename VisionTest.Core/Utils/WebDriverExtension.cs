using OpenQA.Selenium;

namespace VisionTest.Core.Utils
{
    public static class WebDriverExtension
    {
        private static readonly Input.Screen screen = new();

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
