using System.Drawing.Drawing2D;

namespace VisionTest.Core.Utils;

public static class BitmapExtensions
{
    public static Bitmap LoadSafelyImage(string imagePath)
    {
        Bitmap bmp;
        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            bmp = new Bitmap(stream);
        }
        return bmp;
    }

    /// <summary>
    /// Returns a new Bitmap with at least the specified DPI.
    /// If the source already meets or exceeds that DPI, it is returned unchanged.
    /// Otherwise the image is resampled (upscaled) to achieve the target DPI.
    /// </summary>
    /// <param name="source">Input bitmap.</param>
    /// <param name="targetDpi">Desired DPI for both horizontal and vertical axes (default 300).</param>
    public static Bitmap ImproveDpi(this Bitmap source, float targetDpi = 300f)
    {
        // Check current resolution
        float srcDpiX = source.HorizontalResolution;
        float srcDpiY = source.VerticalResolution;

        // If already at or above target, return original
        if (srcDpiX >= targetDpi && srcDpiY >= targetDpi)
            return source;

        // Compute scale factors
        float scaleX = targetDpi / srcDpiX;
        float scaleY = targetDpi / srcDpiY;

        int newW = (int)Math.Round(source.Width * scaleX);
        int newH = (int)Math.Round(source.Height * scaleY);

        // Create a new bitmap at desired size & DPI
        var result = new Bitmap(newW, newH);
        result.SetResolution(targetDpi, targetDpi);

        // Draw the source into it with high-quality settings
        using (var g = Graphics.FromImage(result))
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImage(source,
                        new Rectangle(0, 0, newW, newH),
                        new Rectangle(0, 0, source.Width, source.Height),
                        GraphicsUnit.Pixel);
        }

        return result;
    }
}
