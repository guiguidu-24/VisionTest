using NUnit.Framework;
using POC_Tesseract.UserInterface;
using System.Drawing;

namespace TestTesseract
{
    [TestFixture]
    public class ScreenTests
    {
        [Test]
        public void Screen_ShouldHaveCorrectBounds()
        {
            // Dimensions connues de l'�cran (� adapter selon votre environnement)
            const int expectedWidth = 1920;
            const int expectedHeight = 1080;

            // V�rifie les dimensions de l'�cran
            Assert.Multiple(() =>
            {
                Assert.That(Screen.Width, Is.EqualTo(expectedWidth), "La largeur de l'�cran ne correspond pas � la valeur attendue.");
                Assert.That(Screen.Height, Is.EqualTo(expectedHeight), "La hauteur de l'�cran ne correspond pas � la valeur attendue.");
            });

            // V�rifie les coordonn�es du rectangle Bounds
            var expectedBounds = new Rectangle(0, 0, expectedWidth, expectedHeight);
            Assert.That(Screen.Bounds, Is.EqualTo(expectedBounds), "Les dimensions du rectangle Bounds ne correspondent pas � la valeur attendue.");
        }
    }
}
