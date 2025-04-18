using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace POC_Tesseract
{
    internal class ImgEngine
    {
        private float threshold = 0.9f; // Default threshold for template matching
        public ImgEngine(float threshold)
        {
            if (threshold < 0 || threshold > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be between 0 and 1.");
            }
            this.threshold = threshold;
        }

        private Mat BitmapToMat(Bitmap image)
        {
            // Convert Bitmap to Mat
            Mat mat = new Mat();
            BitmapConverter.ToMat(image, mat);
            return mat;
        }

        public bool Find(Bitmap image, Bitmap target, out Rectangle area)
        {
            // Load images
            using Mat bigImage = BitmapConverter.ToMap(image); //TODO trouver la bonne fonction de conversion bitmap-mat
            using Mat smallImage = BitmapToMat(target);

            // Result image to store match confidence
            using Mat result = new Mat();

            // Match template
            Cv2.MatchTemplate(bigImage, smallImage, result, TemplateMatchModes.CCoeffNormed);

            // Get best match location
            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);


            if (maxVal >= threshold) // You can tweak this threshold
            {
                Console.WriteLine($"Match found at: {maxLoc.X}, {maxLoc.Y}");

                area = new Rectangle(maxLoc.X, maxLoc.Y, smallImage.Width, smallImage.Height);
                return true;
            }
            else
            {
                area = Rectangle.Empty;
                return false;
            }
        }
    }
}
