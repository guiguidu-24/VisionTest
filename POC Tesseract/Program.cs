using System.Drawing;
using Tesseract;


var imagePath = "C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\POC Tesseract\\LoginPage.png";
using var img = new Bitmap(imagePath);

//using var imgSample = img.Clone(new Rectangle(706, 448, 1210-706, 653-448), img.PixelFormat);
//
using var engine = new TesseractEngine("C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\POC Tesseract\\tessdata\\", "fra", EngineMode.Default);
//
//using var container = engine.Process(imgSample);
//
//Console.WriteLine("Text inside the part of the image:");
//Console.WriteLine(container.GetText());


using Page page = engine.Process(img);
var iterator = page.GetIterator();
iterator.Begin();

do
{
    string lineText = iterator.GetText(PageIteratorLevel.TextLine);
    if (!string.IsNullOrEmpty(lineText) && lineText.Contains("se connecter", StringComparison.OrdinalIgnoreCase))
    {
        if (iterator.TryGetBoundingBox(PageIteratorLevel.TextLine, out var rect))
        {
            Console.WriteLine($"Found text: {lineText}");
            Console.WriteLine($"Bounding box: {rect.X1}, {rect.Y1}, {rect.X2}, {rect.Y2}");
            return;
        }
    }
} while (iterator.Next(PageIteratorLevel.TextLine));

