using Tesseract;
using System.Reflection;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using VisionTest.Core.Utils;

namespace VisionTest.Core.Recognition;

public class OcrEngine : IRecognitionEngine<string>
{
    private string datapath; // vaut ./tessdata
    OcrOptions ocrOptions;

    public OcrEngine()
    {
        datapath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        ocrOptions = new OcrOptions();
    }

    public OcrEngine(OcrOptions options, string datapath)
    {
        this.datapath = datapath;
        ocrOptions = options;
    }

    public OcrEngine(OcrOptions options)
    {
        ocrOptions = options;
        datapath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
    }


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
        using var engine = new TesseractEngine(datapath, ocrOptions.Lang.ToCode(), (EngineMode)ocrOptions.OEM ); //FIXIT #6

        // 2. Optionally restrict charset
        var mergedWhiteList = ocrOptions.GetMergedWhiteList(target);
        if (!string.IsNullOrEmpty(mergedWhiteList))
            engine.SetVariable("tessedit_char_whitelist", mergedWhiteList);

        var mergedBlackList = ocrOptions.GetMergedBlackList(target);
        if (!string.IsNullOrEmpty(mergedBlackList))
            engine.SetVariable("tessedit_char_blacklist", mergedBlackList);

        // 3. User-words (to bias toward your phrase)
        string cfgDir = Path.Combine(datapath, "configs");
        Directory.CreateDirectory(cfgDir);
        string userWordsFileName = Guid.NewGuid() + "user-words.txt";
        string userWordsFile = Path.Combine(cfgDir, userWordsFileName);
        File.WriteAllLines(userWordsFile, ocrOptions.WordWhiteList.Append(target));
        engine.SetVariable("user_words_file", Path.GetFileNameWithoutExtension(userWordsFileName));


        // 4. Always use SparseText for precise word boxes
        using var page = engine.Process(image, (PageSegMode) ocrOptions.PSM);

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

            result.Add(MapRectangleToOriginal(new Rectangle(x1, y1, x2 - x1, y2 - y1), image, image));
        }

        File.Delete(userWordsFile);
        return result;
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
        if (original == null)
            throw new ArgumentNullException(nameof(original));
        if (original == null)
            throw new ArgumentNullException(nameof(processed));

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
        using var engine = new TesseractEngine(datapath, ocrOptions.Lang.ToCode(), EngineMode.Default);
        using Page page = engine.Process(image);
        return page.GetText();
    }

}
