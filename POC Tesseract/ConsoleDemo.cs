using System.Diagnostics;
using OpenCvSharp;
using Tesseract;


namespace POC_Tesseract
{
    internal class ConsoleDemo
    {
        public static void Run()
        {
            var testImagePath = "C:\\Users\\guill\\Programmation\\dotNET_doc\\POC_Tesseract\\POC Tesseract\\phototest.tif";

            try
            {
                using (var engine = new TesseractEngine(@"C:/Users/guill/Programmation/dotNET_doc/POC_Tesseract/POC Tesseract/tessdata/", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                            Console.WriteLine("Text (GetText): \r\n{0}", text);
                            Console.WriteLine("Text (iterator):");
                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();

                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                                {
                                                    Console.WriteLine("<BLOCK>");
                                                }

                                                Console.Write(iter.GetText(PageIteratorLevel.Word));
                                                Console.Write(" ");

                                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                                {
                                                    Console.WriteLine();
                                                }
                                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                            {
                                                Console.WriteLine();
                                            }
                                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                } while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        public static void findingtext()
        {
            var imagePath = "E:\\Projects data\\POC_Tesseract\\POC Tesseract\\LoginPage.png";
            using var img = new Bitmap(imagePath);

            //using var imgSample = img.Clone(new Rectangle(706, 448, 1210-706, 653-448), img.PixelFormat);
            //
            using var engine = new TesseractEngine("E:\\Projects data\\POC_Tesseract\\POC Tesseract\\tessdata\\", "fra", EngineMode.Default);
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
        }

        public static void findingWords()
        {
            using var img = new Bitmap("E:\\Projects data\\POC_Tesseract\\POC Tesseract\\Rectangle.png");
            var ocrEngine = new OCREngine("eng");


            if (ocrEngine.Find(img, "the following", out Rectangle area))
            {
                Pen pen = new Pen(Color.Red, 2);
                using Graphics g = Graphics.FromImage(img);
                g.DrawRectangle(pen, area);

                img.Save("E:\\Projects data\\POC_Tesseract\\POC Tesseract\\Rectangle_highlight.png");
            }
            else
            {
                Console.WriteLine("Text not found");
            }
        }

        public static void notePadOpening()
        {
            var appli = new Appli("notepad", new string[] { "E:\\Projects data\\POC_Tesseract\\TestTesseract\\engText.txt" });

            var point = appli.WaitFor("cotton-like");
            appli.Click(point);


            Pen pen = new Pen(Color.Red, 3);
            var screen = appli.GetScreen();

            var graphics = Graphics.FromImage(screen);
            graphics.DrawIcon(SystemIcons.Shield, point.X, point.Y);
            screen.Save("E:\\Projects data\\POC_Tesseract\\TestTesseract\\screenshot.png", System.Drawing.Imaging.ImageFormat.Png);


            appli.Close();
        }

        public static void imageRun()
        {
            // Load images
            using Mat bigImage = Cv2.ImRead("E:\\Projects data\\POC_Tesseract\\POC Tesseract\\big.png", ImreadModes.Color);
            using Mat smallImage = Cv2.ImRead("E:\\Projects data\\POC_Tesseract\\POC Tesseract\\small.png", ImreadModes.Color);

            // Result image to store match confidence
            using Mat result = new Mat();

            // Match template
            Cv2.MatchTemplate(bigImage, smallImage, result, TemplateMatchModes.CCoeffNormed);

            // Get best match location
            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

            Console.WriteLine($"Match confidence: {maxVal}");

            if (maxVal >= 0.9) // You can tweak this threshold
            {
                Console.WriteLine($"Match found at: {maxLoc.X}, {maxLoc.Y}");

                // Optional: Draw rectangle on the match
                Cv2.Rectangle(bigImage, new OpenCvSharp.Rect(maxLoc, smallImage.Size()), Scalar.Red, 2);
                Cv2.ImWrite("E:\\Projects data\\POC_Tesseract\\TestTesseract\\match_result.png", bigImage);
            }
            else
            {
                Console.WriteLine("No strong match found.");
            }
        }
    }
}
