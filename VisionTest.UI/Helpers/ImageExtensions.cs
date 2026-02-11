using System.Drawing;
using System.IO;

namespace VisionTest.UI.Helpers;

public static class ImageExtensions
{
    // <summary>
    /// Converts an Avalonia Bitmap to a System.Drawing.Bitmap.
    /// </summary>
    public static Bitmap ToSystemDrawingBitmap(this Avalonia.Media.Imaging.Bitmap avaloniaBitmap)
    {
        using (var ms = new MemoryStream())
        {
            // Save the Avalonia bitmap into the stream as a PNG
            avaloniaBitmap.Save(ms);

            // Reset stream position for the System.Drawing reader
            ms.Seek(0, SeekOrigin.Begin);

            return new Bitmap(ms);
        }
    }
}
