using Tesseract;
using System.Configuration;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace VisionTest.Core.Recognition
{
    public class OCREngine : IRecognitionEngine<Bitmap,string>
    {
        private string language;
        private string datapath; // vaut ./tessdata

        public OCREngine(string language)
        {
            this.language = language;

            // Retrieving the Tesseract data path from App.config
            //this.datapath = ConfigurationManager.AppSettings["TesseractDataPath"]
            //                ?? throw new ConfigurationErrorsException("La clé 'TesseractDataPath' est manquante dans App.config.");

            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("The assembly path is null");
            datapath = Path.Combine(assemblyDir, "tessdata");

            

        }

        public OCREngine(string language, string datapath)
        {
            this.language = language;
            this.datapath = datapath;
        }

        /// <summary>
        /// Finds the text in the image and returns the area of the text.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="text"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool Find(Bitmap image, string text, out Rectangle area) //TODO do it async
        {
            area = Rectangle.Empty;
            if (text == string.Empty)
            {
                throw new ArgumentException("Text cannot be empty.", nameof(text));
            }

            List<string> words = text.Split(' ').ToList();
            using var engine = new TesseractEngine(datapath, language, EngineMode.Default);

            using Page page = engine.Process(image);
            var iterator = page.GetIterator();
            iterator.Begin();

            do
            {
                string lineText = iterator.GetText(PageIteratorLevel.TextLine);

                // Check if the line contains the text we're looking for
                if (!string.IsNullOrEmpty(lineText) && lineText.Contains(text, StringComparison.OrdinalIgnoreCase))
                {
                    var boxes = new List<Rectangle>();

                    // Iterate through the words in the line
                    do
                    {
                        foreach (string word in words)
                        {
                            string wordText = iterator.GetText(PageIteratorLevel.Word);

                            if (wordText.Equals(word, StringComparison.OrdinalIgnoreCase))
                            {
                                // Get the bounding box of the word
                                if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                                {
                                    boxes.Add(new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height));
                                }
                                if (!iterator.Next(PageIteratorLevel.Word))
                                    break;
                            }
                            else
                            {
                                // If the word is not found, break out of the loop
                                boxes = new List<Rectangle>();
                                break;
                            }
                        }
                        if (boxes.Count == words.Count)
                        {
                            // Get the bounding box of the group of Words
                            var xArea = boxes.Select(box => box.X).Min();
                            var yArea = boxes.Select(box => box.Y).Min();
                            var wArea = boxes.Select(box => box.Right).Max() - xArea;
                            var hArea = boxes.Select(box => box.Bottom).Max() - yArea;

                            area = new Rectangle(xArea, yArea, wArea, hArea);
                            return true;
                        }
                    } while (iterator.Next(PageIteratorLevel.Word));
                }
            } while (iterator.Next(PageIteratorLevel.TextLine));

            return false;
        } 

        /// <summary>
        /// Finds the text in the image and returns the area of the text in the searching area.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="text"></param>
        /// <param name="boxToSearchIn">the area in the screen it is allowed to find the text</param>
        /// <param name="area">The box around the text if the text is found</param>
        /// <returns></returns>
        public bool Find(Bitmap image, string text, Rectangle boxToSearchIn, out Rectangle area)
        {
            return Find(image.Clone(boxToSearchIn, image.PixelFormat), text, out area);
        }

        public IEnumerable<Rectangle> Find(Bitmap image, string target)
        {
            var result = new List<Rectangle>();
            if (target == string.Empty)
            {
                throw new ArgumentException("Text cannot be empty.", nameof(target));
            }

            List<string> words = target.Split(' ').ToList();
            using var engine = new TesseractEngine(datapath, language, EngineMode.Default);

            using Page page = engine.Process(image);
            var iterator = page.GetIterator();
            iterator.Begin();

            do
            {
                string lineText = iterator.GetText(PageIteratorLevel.TextLine);

                // Check if the line contains the text we're looking for
                if (!string.IsNullOrEmpty(lineText) && lineText.Contains(target, StringComparison.OrdinalIgnoreCase))
                {
                    var boxes = new List<Rectangle>();

                    // Iterate through the words in the line
                    do
                    {
                        foreach (string word in words)
                        {
                            string wordText = iterator.GetText(PageIteratorLevel.Word);

                            if (wordText.Equals(word, StringComparison.OrdinalIgnoreCase))
                            {
                                // Get the bounding box of the word
                                if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                                {
                                    boxes.Add(new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height));
                                }
                                if (!iterator.Next(PageIteratorLevel.Word))
                                    break;
                            }
                            else
                            {
                                // If the word is not found, break out of the loop
                                boxes = new List<Rectangle>();
                                break;
                            }
                        }
                        if (boxes.Count == words.Count)
                        {
                            // Get the bounding box of the group of Words
                            var xArea = boxes.Select(box => box.X).Min();
                            var yArea = boxes.Select(box => box.Y).Min();
                            var wArea = boxes.Select(box => box.Right).Max() - xArea;
                            var hArea = boxes.Select(box => box.Bottom).Max() - yArea;

                            result.Add(new Rectangle(xArea, yArea, wArea, hArea));
                        }
                    } while (iterator.Next(PageIteratorLevel.Word));
                }
            } while (iterator.Next(PageIteratorLevel.TextLine));

            return result;
        }

        public string GetText(Bitmap image)
        {
            using var engine = new TesseractEngine(datapath, language, EngineMode.Default);
            using Page page = engine.Process(image);
            return page.GetText();
        }

    }
}
