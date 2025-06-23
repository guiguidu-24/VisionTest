using Tesseract;
using System.Configuration;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace VisionTest.Core.Recognition
{
    public class OCREngine : IRecognitionEngine<string>
    {
        private string language;
        private string datapath; // vaut ./tessdata
        private int fuzzyTolerance = 1;

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
        /// Searches for the given target in the image and returns all matching regions.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<Rectangle> Find(Bitmap image, string target)
        {
            var result = new List<Rectangle>();
            if (string.IsNullOrWhiteSpace(target))
                throw new ArgumentException("Text cannot be empty.", nameof(target));

            List<string> targetWords = target.Split(' ').ToList();

            // Charger le dico user_words si présent dans tessdata
            var configFiles = new[] { "user_words" };
            using var engine = new TesseractEngine(datapath, language, EngineMode.Default, configFiles);
            using Page page = engine.Process(image);
            using var iterator = page.GetIterator();

            iterator.Begin();

            // Parcours de chaque ligne
            do
            {
                int matchedWords = 0;
                var lineBoxes = new List<Rectangle>();

                // On positionne l’iterator sur le premier mot de la ligne
                bool inLine = true;
                do
                {
                    string ocrWord = iterator.GetText(PageIteratorLevel.Word);

                    if (IsFuzzyMatch(ocrWord, targetWords[matchedWords], fuzzyTolerance))
                    {
                        if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                        {
                            lineBoxes.Add(new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height));
                        }
                        matchedWords++;
                        if (matchedWords == targetWords.Count)
                        {
                            // Tous les mots trouvés consécutivement
                            int x = lineBoxes.Min(b => b.X);
                            int y = lineBoxes.Min(b => b.Y);
                            int w = lineBoxes.Max(b => b.Right) - x;
                            int h = lineBoxes.Max(b => b.Bottom) - y;
                            result.Add(new Rectangle(x, y, w, h));
                            break;
                        }
                    }
                    else
                    {
                        // Redémarrer la recherche de la séquence
                        lineBoxes.Clear();
                        matchedWords = 0;
                    }
                    // Avance au mot suivant, mais ne sort pas de la ligne
                    inLine = iterator.Next(PageIteratorLevel.Word)
                             && !iterator.IsAtBeginningOf(PageIteratorLevel.TextLine);

                } while (inLine);

            } while (iterator.Next(PageIteratorLevel.TextLine));

            return result;
        }

        // FuzzyMatch et Levenshtein comme précédemment :
        private bool IsFuzzyMatch(string word1, string word2, int tolerance)
        {
            if (string.IsNullOrEmpty(word1) || string.IsNullOrEmpty(word2)) return false;
            if (word1.Equals(word2, StringComparison.OrdinalIgnoreCase)) return true;
            return LevenshteinDistance(word1, word2) <= tolerance;
        }

        private int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t?.Length ?? 0;
            if (string.IsNullOrEmpty(t)) return s.Length;

            var d = new int[s.Length + 1, t.Length + 1];
            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[s.Length, t.Length];
        }



        public string GetText(Bitmap image)
        {
            using var engine = new TesseractEngine(datapath, language, EngineMode.Default);
            using Page page = engine.Process(image);
            return page.GetText();
        }

    }
}
