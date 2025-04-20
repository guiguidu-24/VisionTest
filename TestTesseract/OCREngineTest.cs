

using POC_Tesseract;
using System.Configuration;
using System.Drawing;
using System.Xml.Linq;

namespace TestTesseract
{
    internal class OCREngineTest
    {
        private OCREngine ocrEngine;

        [SetUp]
        public void Setup()
        {
            // Charger le fichier XML
            var configFilePath = @"..\..\..\..\POC Tesseract\App.config"; // Chemin relatif vers le fichier XML
            var configXml = XDocument.Load(configFilePath);

            // Extraire la valeur de la clé "TesseractDataPath"
            var tessDataPath = configXml
                .Descendants("appSettings")
                .Descendants("add")
                .FirstOrDefault(e => e.Attribute("key")?.Value == "TesseractDataPath")
                ?.Attribute("value")?.Value;

            if (string.IsNullOrEmpty(tessDataPath))
            {
                throw new InvalidOperationException("La clé 'TesseractDataPath' est introuvable ou vide dans le fichier App.config.");
            }

            // Initialiser OCREngine avec le chemin extrait
            ocrEngine = new OCREngine("eng", tessDataPath);
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
            var result = ocrEngine.Find(image, expectedText, out Rectangle actualRectangle);

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
            var result = ocrEngine.Find(image, expectedText, out Rectangle actualRectangle);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.False, "Le texte a pas trouvé dans l'image.");
                Assert.That(actualRectangle.IsEmpty, Is.True, "Le rectangle retourné n'est pas vide.\nRectangle :" + actualRectangle.ToString());
            });
        }

        [Test]
        public void TestFindTextInImage_TextEmpty()
        {
            var imagePath = @"..\..\..\images\LoginPage.png";
            var expectedText = String.Empty;

            using var image = new Bitmap(imagePath);

            // Act
            try
            {
                var result = ocrEngine.Find(image, expectedText, out Rectangle actualRectangle);
            }
            catch (ArgumentException ex)
            {
                return; // Test réussi
            }

            Assert.That(false,Is.True, "Une exception ArgumentException aurait dû être levée pour le texte vide.");
        }


    }
}
