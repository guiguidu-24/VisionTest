using Tesseract;
using System.Reflection;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using VisionTest.Core.Utils;

namespace VisionTest.Core.Recognition;

public class OcrEngine : IRecognitionEngine<string>
{
    private string language;
    private string datapath; // vaut ./tessdata
    private int fuzzyTolerance = 1;

    public OcrEngine(string language)
    {
        this.language = language;
        string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("The assembly path is null");
        datapath = Path.Combine(assemblyDir, "tessdata");
    }

    public OcrEngine(string language, string datapath)
    {
        this.language = language;
        this.datapath = datapath;
    }

    public OcrEngine(OcrOptions options)
    {
        language = options.Lang.ToCode();
        datapath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        CharWhiteList = options.WhiteListChar;
        WordWhiteList = options.WordWhiteList ?? [];
        LstmOnly = options.LTSMOnly;
        UseThresholdFilter = options.UseThresholdFilter;
        ImproveDpi = options.ImproveDPI;
    }

    public bool LstmOnly {private get; set; } = true; // true for best accuracy on trained models, false for legacy Tesseract mode
    public string CharWhiteList { private get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "; 
    public IEnumerable<string> WordWhiteList {get; set; } = []; // e.g. ["MYTARGETWORD", "ANOTHERWORD"]
    public bool UseThresholdFilter { private get; set; } = false; // false by default to maintain existing behavior
    public bool ImproveDpi { private get; set; } = false; // false by default, set to true to improve DPI of input images //TODO DPI Value instead of boolean



    /// <summary>
    /// Searches for the given target in the image and returns all matching regions.
    /// </summary>
    /// <param name="image"></param>
    /// <param name="target"></param>
    /// <param name="LstmOnly"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IEnumerable<Rectangle> Find(Bitmap image, string target)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new ArgumentException("Target cannot be null or empty.", nameof(target));

        var result = new List<Rectangle>();
        var targetWords = target.Trim()
                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // 1. Init engine
        using var engine = new TesseractEngine(@"./tessdata", "eng",
                               LstmOnly ? EngineMode.LstmOnly : EngineMode.TesseractAndLstm);

        // 2. Optionally restrict charset
        var charWhiteList = AddCharacters(target);
        if (!string.IsNullOrEmpty(charWhiteList))
            engine.SetVariable("tessedit_char_whitelist", charWhiteList);

        // 3. User-words (to bias toward your phrase)
        string cfgDir = Path.Combine(datapath, "configs");
        Directory.CreateDirectory(cfgDir);
        string userWordsFileName = Guid.NewGuid() + "user-words.txt";
        string userWordsFile = Path.Combine(cfgDir, userWordsFileName);
        File.WriteAllLines(userWordsFile, WordWhiteList.Append(target));
        engine.SetVariable("user_words_file", userWordsFileName.AsSpan().TrimEnd(".txt").ToString());

        // Apply threshold filter if enabled
        using var processedImage = UseThresholdFilter ? ThresholdFilter(image) : image;

        using var processedImageDpi = ImproveDpi ? processedImage.ImproveDpi(600f) : processedImage;

        // 4. Always use SparseText for precise word boxes
        using var page = engine.Process(processedImageDpi, PageSegMode.SparseText);

        // 5. Pull out every single word + its box
        var words = new List<(string Text, Tesseract.Rect Box)>();
        using var iter = page.GetIterator();
        iter.Begin();
        do
        {
            if (iter.IsAtBeginningOf(PageIteratorLevel.Word) &&
                iter.TryGetBoundingBox(PageIteratorLevel.Word, out var r))
            {
                string w = iter.GetText(PageIteratorLevel.Word).Trim();
                if (!string.IsNullOrEmpty(w))
                    words.Add((w, r));
            }
        } while (iter.Next(PageIteratorLevel.Word));

        // 6. Slide a window of length N over the words list
        int N = targetWords.Length;
        for (int i = 0; i + N <= words.Count; i++)
        {
            bool match = true;
            for (int j = 0; j < N; j++)
            {
                if (!string.Equals(words[i + j].Text,
                                   targetWords[j],
                                   StringComparison.OrdinalIgnoreCase))
                {
                    match = false;
                    break;
                }
            }
            if (!match)
                continue;

            // 7. Compute union bbox of words[i..i+N-1]
            int x1 = words[i].Box.X1;
            int y1 = words[i].Box.Y1;
            int x2 = words[i].Box.X1 + words[i].Box.Width;
            int y2 = words[i].Box.Y1 + words[i].Box.Height;

            for (int j = 1; j < N; j++)
            {
                var b = words[i + j].Box;
                x1 = Math.Min(x1, b.X1);
                y1 = Math.Min(y1, b.Y1);
                x2 = Math.Max(x2, b.X1 + b.Width);
                y2 = Math.Max(y2, b.Y1 + b.Height);
            }

            result.Add(MapRectangleToOriginal(new Rectangle(x1, y1, x2 - x1, y2 - y1), image, processedImageDpi));
        }

        File.Delete(userWordsFile);
        return result;
    }


    private string AddCharacters(string text)
    {
        if (string.IsNullOrEmpty(text))
            return CharWhiteList;

        // Convert current whitelist to HashSet for efficient lookups
        var existingChars = new HashSet<char>(CharWhiteList);
        
        // Add new unique characters from the text
        foreach (char c in text)
        {
            if (!existingChars.Contains(c))
                existingChars.Add(c);
        }

        // Convert back to string and update CharWhiteList
        CharWhiteList = new string(existingChars.ToArray());
        return CharWhiteList;
    }

    // FuzzyMatch et Levenshtein comme précédemment :
    private bool IsFuzzyMatch(string word1, string word2, int tolerance) //TODO do it with a string comparer
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

    private Bitmap ThresholdFilter(Bitmap src)
    {
        Mat gray = src.ToMat().ConvertToGray();
        // Convert to grayscale 
        Mat bw = new Mat();
        Cv2.AdaptiveThreshold(gray, bw, 255,
            AdaptiveThresholdTypes.GaussianC,
            ThresholdTypes.BinaryInv, 11, 2);

        return bw.ToBitmap();
    }

    /// <summary>
    /// Maps a rectangle from the processed (e.g. upscaled) image back to the coordinate space of the original image.
    /// </summary>
    /// <param name="rectInProcessed">The rectangle in the processed image’s pixel coordinates.</param>
    /// <param name="original">The original bitmap.</param>
    /// <param name="processed">The processed (resampled) bitmap.</param>
    /// <returns>A Rectangle in the original image’s pixel coordinates.</returns>
    private static Rectangle MapRectangleToOriginal(Rectangle rectInProcessed, Bitmap original, Bitmap processed)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(processed);

        // Compute the scale factors between the two images
        double scaleX = (double)original.Width / processed.Width;
        double scaleY = (double)original.Height / processed.Height;

        // Map each component back
        int origX = (int)Math.Round(rectInProcessed.X * scaleX);
        int origY = (int)Math.Round(rectInProcessed.Y * scaleY);
        int origWidth = (int)Math.Round(rectInProcessed.Width * scaleX);
        int origHeight = (int)Math.Round(rectInProcessed.Height * scaleY);

        return new Rectangle(origX, origY, origWidth, origHeight);
    }


    public string GetText(Bitmap image)
    {
        using var engine = new TesseractEngine(datapath, language, EngineMode.Default);
        using Page page = engine.Process(image);
        return page.GetText();
    }

}
