namespace VisionTest.Core.Utils
{
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
    }
}
