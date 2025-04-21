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
            // Dimensions connues de l'écran (à adapter selon votre environnement)
            const int expectedWidth = 1920;
            const int expectedHeight = 1080;

            // Vérifie les dimensions de l'écran
            Assert.Multiple(() =>
            {
                Assert.That(Screen.Width, Is.EqualTo(expectedWidth), "La largeur de l'écran ne correspond pas à la valeur attendue.");
                Assert.That(Screen.Height, Is.EqualTo(expectedHeight), "La hauteur de l'écran ne correspond pas à la valeur attendue.");
            });

            // Vérifie les coordonnées du rectangle Bounds
            var expectedBounds = new Rectangle(0, 0, expectedWidth, expectedHeight);
            Assert.That(Screen.Bounds, Is.EqualTo(expectedBounds), "Les dimensions du rectangle Bounds ne correspondent pas à la valeur attendue.");
        }
    }
}
