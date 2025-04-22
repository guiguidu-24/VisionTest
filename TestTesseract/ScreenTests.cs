using POC_Tesseract.UserInterface;
using System.Resources;

namespace TestTesseract
{
    [TestFixture]
    public class ScreenTests
    {
        [Test]
        public void Screen_ShouldHaveCorrectBounds()
        {
            // Load expected dimensions from TestResources.resx
            var resourceManager = new ResourceManager("TestTesseract.TestResources", typeof(ScreenTests).Assembly);
            var widthString = resourceManager.GetString("ScreenWidth");
            var heightString = resourceManager.GetString("ScreenHeight");

            Assert.Multiple(() =>
            {
                Assert.That(widthString, Is.Not.Null.And.Not.Empty, "'ScreenWidth' value in resources is empty or null.");
                Assert.That(heightString, Is.Not.Null.And.Not.Empty, "'ScreenHeight' value in resources is empty or null.");
            });

            var expectedWidth = int.Parse(widthString);
            var expectedHeight = int.Parse(heightString);

            // Verify screen dimensions
            Assert.Multiple(() =>
            {
                Assert.That(Screen.Width, Is.EqualTo(expectedWidth), "The screen width does not match the expected value.");
                Assert.That(Screen.Height, Is.EqualTo(expectedHeight), "The screen height does not match the expected value.");
            });
        }

        [Test]
        public void GetScaleFactor_ShouldMatchResourceValue()
        {
            // Load the expected value from TestResources.resx  
            var resourceManager = new ResourceManager("TestTesseract.TestResources", typeof(ScreenTests).Assembly);
            var scaleFactorString = resourceManager.GetString("ScreenScale");
            Assert.That(scaleFactorString, Is.Not.Null.And.Not.Empty, "'ScreenScale' value in resources is empty or null.");
            var expectedScaleFactor = float.Parse(scaleFactorString.TrimEnd('%')) / 100;

            // Call the method and verify the value  
            var actualScaleFactor = Screen.GetScaleFactor();
            Assert.That(actualScaleFactor, Is.EqualTo(expectedScaleFactor), "The returned scale factor does not match the expected value in resources.");
        }
    }
}
