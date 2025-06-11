using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using VisionTest.Core.Utils;

namespace VisionTest.Core.Recognition
{
    public class WebEngine : IRecognitionEngine<Rectangle, By>
    {
        private readonly IWebDriver _webDriver;

        public WebEngine(Browser browser)
        {
            switch (browser)
            {
                case Browser.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddExcludedArgument("enable-automation");
                    chromeOptions.AddAdditionalOption("useAutomationExtension", false); // optional, disables legacy extension
                    _webDriver = new ChromeDriver(chromeOptions);
                    break;
                case Browser.Firefox:
                    var firefoxOptions = new FirefoxOptions();
                    _webDriver = new FirefoxDriver(firefoxOptions);
                    break;
                case Browser.Edge:
                    var edgeOptions = new EdgeOptions();
                    edgeOptions.AddExcludedArgument("enable-automation");
                    _webDriver = new EdgeDriver(edgeOptions);
                    break;
                default:
                    throw new ArgumentException("Unsupported browser type", nameof(browser));
            }
        }

        public WebEngine(Browser browser, string url) : this(browser)
        {
            _webDriver.Navigate().GoToUrl(url);
        }

        public IEnumerable<Rectangle> Find(Rectangle domain, By target)
        {
            return (IEnumerable<Rectangle>)_webDriver.FindElements(target).Where(elt => elt.Inside(domain));
        }
    }
}
