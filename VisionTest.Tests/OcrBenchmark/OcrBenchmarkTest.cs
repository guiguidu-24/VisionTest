using System.Drawing;
using System.Xml.Linq;
using VisionTest.Core.Recognition;

namespace VisionTest.Tests.OcrBenchmark
{
    internal class OcrBenchmarkTest
    {
        private const int positionTolerance = 10; // Tolerance in pixels for the position of the center
        private static string ocrBenchmarkDirectory = @"..\..\..\OcrBenchmark\";

        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3", true)]
        [TestCase("4")]
        [TestCase("5")]
        [TestCase("6")]
        [TestCase("7")]
        [TestCase("8")]
        [TestCase("9")]
        [TestCase("10")]
        public void RunOCROnAnImageAndCheckThePositionOfTheRectangle(string imageUnderTestNameWithoutExtension, bool improveDPI = false, bool useThresholdFilter = false)
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

                OCREngine ocrEngine = new OCREngine(new OCREngineOptions {ImproveDPI = improveDPI });
                

                using var targetImage = new Bitmap(imagePath);

                var foundRects = ocrEngine.Find(targetImage, name);

                Assert.That(foundRects, Is.Not.Empty, $"No rectangles found for target '{name}' in image '{imageUnderTestNameWithoutExtension}'.");


                var actualRectangle = foundRects.First();
                Assert.Multiple(() =>
                {
                    //Assert.That(
                    // foundRects.Count(),
                    // Is.EqualTo(1),
                    // $"Multiple rectangles were found for the target '{name}' in image '{imageUnderTestNameWithoutExtension}'. " +
                    // $"Expected: {targetRect}. Found: [{string.Join(", ", foundRects.Select(r => r.ToString()))}]"
                    // );

                    // Calculate centers
                    var expectedCenter = new Point(
                        targetRect.X + targetRect.Width / 2,
                        targetRect.Y + targetRect.Height / 2
                    );

                    bool anyMatch = foundRects.Any(rect =>
                    {
                        var actualCenter = new Point(
                            rect.X + rect.Width / 2,
                            rect.Y + rect.Height / 2
                        );
                        return
                            Math.Abs(expectedCenter.X - actualCenter.X) <= positionTolerance &&
                            Math.Abs(expectedCenter.Y - actualCenter.Y) <= positionTolerance &&
                            Math.Abs(targetRect.Width - rect.Width) <= positionTolerance &&
                            Math.Abs(targetRect.Height - rect.Height) <= positionTolerance;
                    });

                    Assert.That(anyMatch, Is.True,
                        $"No rectangle matched for the target '{name}' in image '{imageUnderTestNameWithoutExtension}'. " +
                        $"Expected center: {expectedCenter}, width: {targetRect.Width}, height: {targetRect.Height}. " +
                        $"Found: [{string.Join(", ", foundRects.Select(r => r.ToString()))}]"
                    );
                });
            }
        }
    }
}