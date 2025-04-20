using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;


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

        public ImgEngine()
        {
            // Default constructor with default threshold
        }


        public bool Find(Bitmap image, Bitmap target, out Rectangle area)
        {
            area = Rectangle.Empty;

            // Load images
            using Mat bigImage = ConvertToBGRA(BitmapConverter.ToMat(image));
            using Mat smallImage = ConvertToBGRA(BitmapConverter.ToMat(target));


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
                return false;
            }
        }



        #region helper preprocessing methods
        /// <summary>
        /// Convertit une image Bitmap en Mat OpenCV au format BGRA.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static Mat ConvertToBGRA(Mat src)
        {
            return src.Channels() switch
            {
                3 => ConvertTo(src, ColorConversionCodes.BGR2BGRA),
                1 => ConvertTo(src, ColorConversionCodes.GRAY2BGRA),
                4 => src.Clone(), // déjà en BGRA
                _ => throw new NotSupportedException($"Nombre de canaux non supporté : {src.Channels()}")
            };
        }

        /// <summary>
        /// Convertit une image Mat OpenCV en une autre couleur.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static Mat ConvertTo(Mat src, ColorConversionCodes code)
        {
            Mat dst = new();
            Cv2.CvtColor(src, dst, code);
            return dst;
        }

        #endregion
    }

}
