using System.Drawing;
using System.IO;

namespace POC_Tesseract.Database
{
    internal static class ImageConverter
    {
        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();
            return imageBytes;
        }

        public static byte[] PngFileToBytes(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image file not found at {imagePath}");
            }
            if(Path.GetExtension(imagePath)?.ToLower() != ".png")
            {
                throw new InvalidDataException($"File at {imagePath} is not a PNG image.");
            }

            return File.ReadAllBytes(imagePath);
        }
    }
}
