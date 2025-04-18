using Tesseract;

namespace POC_Tesseract
{
    internal class OCREngine
    {
        private string language;
        private string datapath = @"..\..\..\tessdata\"; //TODO: make it relative 

        public OCREngine(string language)
        {
            this.language = language;
        }

        public OCREngine(string language,string datapath)
        {
            this.language = language;
            this.datapath = datapath;
        }



        public bool Find(Bitmap image, string text, out Rectangle area) //TODO implémenter la distance de levenstein
        {
            area = Rectangle.Empty;
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


        public bool Find(Bitmap image, string text, out Rectangle area, Rectangle inside)
        {
            return Find(image.Clone(inside, image.PixelFormat), text, out area);
        }
    }
}
