using System.Drawing;
using POC_Tesseract;
using WindowsInput;


namespace TestTesseract
{
    [TestFixture]
    internal class AppliTestPaint
    {
        private Appli appli;
        private string paintPath = string.Empty;// TestResources.PaintPath;// @"C:\Windows\System32\mspaint.exe";
        private string bigImagePath = @"..\..\..\images\big.png";
        private string smallImagePath = @"..\..\..\images\small.png";

        [SetUp]
        public void Setup()
        {
            //var resourceManager = new ResourceManager("TestTesseract.TestResources", typeof(AppliTestPaint).Assembly);
            var stringPaintPath = TestResources.PaintPath; // resourceManager.GetString("PaintPath");
            Assert.That(stringPaintPath, Is.Not.Null.And.Not.Empty, "'PaintPath' value in resources is empty or null.");
            paintPath = stringPaintPath;

            // Ensure the paths exist  
            if (paintPath.Contains(".exe") && !File.Exists(paintPath))
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
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null.")); //313; //309
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null.")); //722; //577
            const int tolerance = 10;

            // Open Paint with the big image  
            appli.Open();

            appli.Wait(1000); // Wait for the application to open and load the image
            appli.MaximizeWindow();
            appli.Wait(100); // Wait for the application to maximize
            Simulate.Events().Click(WindowsInput.Events.KeyCode.F11).Invoke().Wait();
            appli.Wait(500); // Wait for the application to maximize

            var screen = appli.GetScreen();
            screen.Save(@"C:\Users\guill\Programmation\dotNET_doc\POC_Tesseract\TestTesseract\screenshot.png");

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
            int expectedX = int.Parse(TestResources.bigX ?? throw new NullReferenceException("'bigX' value in resources is empty or null."));//13;
            int expectedY = int.Parse(TestResources.bigY ?? throw new NullReferenceException("'bigY' value in resources is empty or null."));//722;
            const int tolerance = 10;

            // Open Paint with the big image  
            appli.Open();

            appli.Wait(1000); // Wait for the application to open and load the image
            // Maximize the Paint window  
            appli.MaximizeWindow();
            Simulate.Events().Click(WindowsInput.Events.KeyCode.F11).Invoke().Wait();
            appli.Wait(500); // Wait for the application to maximize
            var screen = appli.GetScreen();
            //screen.Save(@"E:\Projects data\POC_Tesseract\TestTesseract\screenshot.png");

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
