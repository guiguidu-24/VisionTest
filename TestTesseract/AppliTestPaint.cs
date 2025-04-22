using System.Drawing;
using POC_Tesseract;


namespace TestTesseract
{
    [TestFixture]
    internal class AppliTestPaint
    {
        private Appli appli;
        private string paintPath = @"C:\Windows\System32\mspaint.exe";
        private string bigImagePath = @"..\..\..\images\big.png";
        private string smallImagePath = @"..\..\..\images\small.png";

        [SetUp]
        public void Setup()
        {
            // Ensure the paths exist  
            if (!File.Exists(paintPath))
                throw new FileNotFoundException("Paint application not found.", paintPath);

            if (!File.Exists(bigImagePath))
                throw new FileNotFoundException("Big image not found.", bigImagePath);

            if (!File.Exists(smallImagePath))
                throw new FileNotFoundException("Small image not found.", smallImagePath);

            appli = new Appli(paintPath, [bigImagePath]);
        }

        [Test]
        public void WaitFor_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            const int expectedX = 313;
            const int expectedY = 722;
            const int tolerance = 10;

            // Open Paint with the big image  
            appli.Open();

            appli.Wait(1000); // Wait for the application to open and load the image
            // Maximize the Paint window  
            appli.MaximizeWindow();
            var screen = appli.GetScreen();
            screen.Save(@"E:\Projects data\POC_Tesseract\TestTesseract\screenshot.png");

            // Load the small image  
            Bitmap smallImage = new Bitmap(smallImagePath);

            try
            {
                // Wait for the small image to appear in the big image  
                Point foundPosition = appli.WaitFor(smallImage, timeout: 5000);

                // Assert the position is within the expected bounds with a tolerance
                Assert.That(foundPosition.X, Is.InRange(expectedX - tolerance, expectedX + tolerance),
                    $"X coordinate should be within {tolerance} pixels of {expectedX}.");
                Assert.That(foundPosition.Y, Is.InRange(expectedY - tolerance, expectedY + tolerance),
                    $"Y coordinate should be within {tolerance} pixels of {expectedY}.");
            }
            catch (TimeoutException ex)
            {
                Assert.Fail($"The image was not found within the timeout period. Exception: {ex.Message}");
            }
            finally
            {
                // Close Paint  
                appli.CloseWindow();
            }
        }

        [Test]
        public void WaitForElement_ImageAppearsWithinTimeout_ReturnsCorrectPosition()
        {
            const int expectedX = 313;
            const int expectedY = 722;
            const int tolerance = 10;

            // Open Paint with the big image  
            appli.Open();

            appli.Wait(1000); // Wait for the application to open and load the image
            // Maximize the Paint window  
            appli.MaximizeWindow();
            var screen = appli.GetScreen();
            screen.Save(@"E:\Projects data\POC_Tesseract\TestTesseract\screenshot.png");

            // Load the small image  
            Bitmap smallImage = new Bitmap(smallImagePath);
            var targetElement = new ScreenElement() { Image = smallImage };

            try
            {
                // Wait for the small image to appear in the big image  
                Point foundPosition = appli.WaitFor(targetElement, timeout: 5000);

                // Assert the position is within the expected bounds with a tolerance
                Assert.That(foundPosition.X, Is.InRange(expectedX - tolerance, expectedX + tolerance),
                    $"X coordinate should be within {tolerance} pixels of {expectedX}.");
                Assert.That(foundPosition.Y, Is.InRange(expectedY - tolerance, expectedY + tolerance),
                    $"Y coordinate should be within {tolerance} pixels of {expectedY}.");
            }
            catch (TimeoutException ex)
            {
                Assert.Fail($"The image was not found within the timeout period. Exception: {ex.Message}");
            }
            finally
            {
                // Close Paint  
                appli.CloseWindow();
            }
        }


        [TearDown]
        public void TearDown()
        {
            appli.CloseWindow();
        }
    }
}
