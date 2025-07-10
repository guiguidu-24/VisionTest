using System.Drawing;
using System.Xml.Linq;
using VisionTest.Core.Recognition;

namespace VisionTest.Tests.OcrBenchmark
{
    internal class OcrBenchmarkTest
    {
        private static string ocrBenchmarkDirectory = @"..\..\..\OcrBenchmark\";

        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        [TestCase("5")]
        [TestCase("6")]
        [TestCase("7")]
        [TestCase("8")]
        [TestCase("9")]
        [TestCase("10")]
        public void RunOCROnAnImageAndCheckThePositionOfTheRectangle(string imageUnderTestNameWithoutExtension)
        {
            string imagePath = Path.Combine(ocrBenchmarkDirectory, "Images", imageUnderTestNameWithoutExtension + ".png");
            string labelPath = Path.Combine(ocrBenchmarkDirectory, "Labels", imageUnderTestNameWithoutExtension + ".xml");

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Image file not found: {imagePath}");
            if (!File.Exists(labelPath))
                throw new FileNotFoundException($"Label file not found: {labelPath}");

            var labels = XDocument.Load(labelPath);

            foreach (var obj in labels.Descendants("object"))
            {
                var name = obj.Element("name")?.Value;
                if (string.IsNullOrEmpty(name))
                    throw new InvalidOperationException("Object name cannot be null or empty.");
                var bndbox = obj.Element("bndbox");
                if (bndbox == null)
                    throw new InvalidOperationException("Bounding box element is missing.");

                var xmin = int.Parse(bndbox.Element("xmin")?.Value ?? "0");
                var xmax = int.Parse(bndbox.Element("xmax")?.Value ?? "0");
                var ymin = int.Parse(bndbox.Element("ymin")?.Value ?? "0");
                var ymax = int.Parse(bndbox.Element("ymax")?.Value ?? "0");
                Rectangle targetRect = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);

                var ocrEngine = new OCREngine("en");
                using var targetImage = new Bitmap(imagePath);

                var foundRects = ocrEngine.Find(targetImage, name);

                Assert.That(foundRects, Is.Not.Empty, $"No rectangles found for target '{name}' in image '{imageUnderTestNameWithoutExtension}'.");
                Assert.That(foundRects.Count(), Is.EqualTo(1), $"Multiple rectangles were found for the target {name}");

                var actualRectangle = foundRects.First();
                Assert.Multiple(() =>
                {
                    // Calculate centers
                    var expectedCenter = new Point(
                        targetRect.X + targetRect.Width / 2,
                        targetRect.Y + targetRect.Height / 2
                    );
                    var actualCenter = new Point(
                        actualRectangle.X + actualRectangle.Width / 2,
                        actualRectangle.Y + actualRectangle.Height / 2
                    );
                    const int positionTolerance = 10;

                    Assert.That(
                        Math.Abs(expectedCenter.X - actualCenter.X) <= positionTolerance &&
                        Math.Abs(expectedCenter.Y - actualCenter.Y) <= positionTolerance, Is.True,
                        $"Center position is incorrect for target '{name}' in image '{imageUnderTestNameWithoutExtension}'. Expected: {expectedCenter}, Actual: {actualCenter}"
                    );

                    // Width and Height assertions
                    Assert.That(
                        Math.Abs(targetRect.Width - actualRectangle.Width) <= positionTolerance, Is.True,
                        $"Width is incorrect for target '{name}' in image '{imageUnderTestNameWithoutExtension}'. Expected: {targetRect.Width}, Actual: {actualRectangle.Width}"
                    );
                    Assert.That(
                        Math.Abs(targetRect.Height - actualRectangle.Height) <= positionTolerance, Is.True,
                        $"Height is incorrect for target '{name}' in image '{imageUnderTestNameWithoutExtension}'. Expected: {targetRect.Height}, Actual: {actualRectangle.Height}"
                    );
                });
            }
        }
    }
}