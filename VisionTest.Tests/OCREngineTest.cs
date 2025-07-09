using VisionTest.Core.Recognition;
using System.Drawing;

namespace VisionTest.Tests
{
    internal class OCREngineTest
    {
        private OCREngine ocrEngine;

        [SetUp]
        public void Setup()
        {
            // Initialiser OCREngine avec le chemin extrait
            ocrEngine = new OCREngine("eng");
            ocrEngine.ImproveDpi = true; // Activer le filtre de seuil pour les tests
        }

        [Test]
        public void TestFindEdit_In_Visual_studio_image()
        {
            var imagePath = @"..\..\..\images\visualStudio.png";
            var expectedText = "Edit";
            var expectedCenter = new Point(132, 20); // Centre attendu (x, y)
            const int positionTolerance = 10; // Tolérance en pixels pour la position du centre

            using var image = new Bitmap(imagePath);

            // Act
            var list = ocrEngine.Find(image, expectedText);
            var result = list.Count() > 0;
            var actualRectangle = list.FirstOrDefault();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.True, "Le texte n'a pas été trouvé dans l'image.");
                Assert.That(actualRectangle.IsEmpty, Is.False, "Le rectangle retourné est vide.");
            });

            // Calculer le centre du rectangle retourné
            var actualCenter = new Point(
                actualRectangle.X + actualRectangle.Width / 2,
                actualRectangle.Y + actualRectangle.Height / 2
            );

            // Vérification de la position du centre avec une tolérance
            Assert.That(
                Math.Abs(expectedCenter.X - actualCenter.X) <= positionTolerance &&
                Math.Abs(expectedCenter.Y - actualCenter.Y) <= positionTolerance, Is.True,
                $"La position du centre est incorrecte. Attendu : {expectedCenter}, Obtenu : {actualCenter}"
            );
        }

        [Test]
        public void TestFindTextInImage_Area()
        {
            var imagePath = @"..\..\..\images\LoginPage.png";
            var expectedText = "Apple music";
            var expectedCenter = new Point(735, 148); // Centre attendu (x, y)
            const int positionTolerance = 10; // Tolérance en pixels pour la position du centre

            using var image = new Bitmap(imagePath);

            // Act
            var list = ocrEngine.Find(image, expectedText);
            var result = list.Count() > 0;
            var actualRectangle = list.FirstOrDefault();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.True, "Le texte n'a pas été trouvé dans l'image.");
                Assert.That(actualRectangle.IsEmpty, Is.False, "Le rectangle retourné est vide.");
            });

            // Calculer le centre du rectangle retourné
            var actualCenter = new Point(
                actualRectangle.X + actualRectangle.Width / 2,
                actualRectangle.Y + actualRectangle.Height / 2
            );

            // Vérification de la position du centre avec une tolérance
            Assert.That(
                Math.Abs(expectedCenter.X - actualCenter.X) <= positionTolerance &&
                Math.Abs(expectedCenter.Y - actualCenter.Y) <= positionTolerance, Is.True,
                $"La position du centre est incorrecte. Attendu : {expectedCenter}, Obtenu : {actualCenter}"
            );
        }

        [Test]
        public void TestFindTextInImage_NotFound()
        {
            var imagePath = @"..\..\..\images\LoginPage.png";
            var expectedText = "ellpa";
            
            using var image = new Bitmap(imagePath);

            // Act
            var result = ocrEngine.Find(image, expectedText);
            var actualRectangle = result.FirstOrDefault();
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Count, Is.EqualTo(0), "Le texte a pas trouvé dans l'image.");
                Assert.That(actualRectangle.IsEmpty, Is.True, "Le rectangle retourné n'est pas vide.\nRectangle :" + actualRectangle.ToString());
            });
        }

        [Test]
        public void TestFindTextInImage_TextEmpty()
        {
            var imagePath = @"..\..\..\images\LoginPage.png";
            var expectedText = string.Empty;

            using var image = new Bitmap(imagePath);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var results = ocrEngine.Find(image, expectedText);
                var firstRect = results.FirstOrDefault();
            }, "An ArgumentException should be thrown for empty text.");
        }


    }
}
