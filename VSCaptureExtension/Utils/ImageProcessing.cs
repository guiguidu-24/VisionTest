using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VSExtension.Utils
{
    public static class ImageProcessing
    {
        /// <summary>
        /// Convert a Bitmap to BitmapImage for WPF usage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage ConvertToBitmapImage(this Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // Freeze for UI thread safety

            return bitmapImage;
        }

        /// <summary>
        /// Convert a BitmapImage to Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Bitmap ConvertToBitmap(this BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
                throw new ArgumentNullException(nameof(bitmapImage));

            // Create a new memory stream and copy image data into it
            using MemoryStream ms = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder(); // Or BmpBitmapEncoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(ms);

            ms.Position = 0; // Reset stream position
            return new Bitmap(ms); // The Bitmap will internally clone the stream
        }
    }
}
