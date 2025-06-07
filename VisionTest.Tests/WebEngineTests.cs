using OpenQA.Selenium;
using System.Drawing;
using VisionTest.Core.Input;
using VisionTest.Core.Recognition;
using VisionTest.Core.Utils;

namespace VisionTest.Tests
{
    [TestFixture]
    internal class WebEngineTests
    {
        private WebEngine _webEngine;

        [SetUp]
        public void Setup()
        {
            _webEngine = new WebEngine(Browser.Firefox, "https://lms.isae.fr");
        }

        [Test]
        public void FindElements_ShouldReturnElementsInsideDomain()
        {
            const int expectedX = 990;
            const int expectedY = 36;
            const int tolerancePoint = 10;
            const int expectedWidth = 775;
            const int expectedHeight = 40;
            const int toleranceSize = 10;

            // Arrange
            Rectangle domain = new Rectangle(Point.Empty, Screen.ScreenSize);
            var target = By.XPath("//h1[text() = 'CENTRAL AUTHENTICATION SERVICE (CAS)']");

            Task.Delay(2000).Wait(); // Wait for the page to load
            var keyboard = new Keyboard();
            keyboard.PressKey(WindowsInput.Events.KeyCode.F11);
            
            Task.Delay(1000).Wait();
            // Act
            var results = _webEngine.Find(domain, target);


            // Assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.Count, Is.EqualTo(1));

            var rect = results.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.That(Math.Abs(rect.Center().X - expectedX), Is.LessThanOrEqualTo(tolerancePoint),
                    $"Expected X: {expectedX}, Actual X: {rect.Center().X}");
                Assert.That(Math.Abs(rect.Center().Y - expectedY), Is.LessThanOrEqualTo(tolerancePoint),
                    $"Expected Y: {expectedY}, Actual Y: {rect.Center().Y}");
                Assert.That(Math.Abs(rect.Width - expectedWidth), Is.LessThanOrEqualTo(toleranceSize),
                    $"Expected Width: {expectedWidth}, Actual Width: {rect.Width}");
                Assert.That(Math.Abs(rect.Height - expectedHeight), Is.LessThanOrEqualTo(toleranceSize),
                    $"Expected Height: {expectedHeight}, Actual Height: {rect.Height}");
            });
        }


        [TearDown]
        public void TearDown()
        {
            _webEngine.Dispose();
        }
    }
}
