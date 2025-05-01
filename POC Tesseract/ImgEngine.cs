using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;


namespace POC_Tesseract
{
    internal class ImgEngine
    {
        //private float threshold = 0.9f; // Default threshold for template matching
        

        public ImgEngine()
        {
            // Default constructor with default threshold
        }

        /// <summary>
        /// Finds the target image in the source image using template matching.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="target"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool Find(Bitmap image, Bitmap target, out Rectangle area, bool color = false, float threshold = 0.9f)
        {
            area = Rectangle.Empty;

            // Load images
            using Mat bigImage = color ? ConvertToBGRA(BitmapConverter.ToMat(image)) : ConvertToGray(BitmapConverter.ToMat(image));
            using Mat smallImage = color ? ConvertToBGRA(BitmapConverter.ToMat(target)) : ConvertToGray(BitmapConverter.ToMat(target));


            // Result image to store match confidence
            using Mat result = new Mat();

            // Match template
            Cv2.MatchTemplate(bigImage, smallImage, result, TemplateMatchModes.CCoeffNormed);

            // Get best match location
            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);


            if (maxVal >= threshold) // You can tweak this threshold
            {
                area = new Rectangle(maxLoc.X, maxLoc.Y, smallImage.Width, smallImage.Height);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the target image in the source image part highlighted using template matching 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="target"></param>
        /// <param name="boxToSearchIn">The area to search the target in</param>
        /// <param name="area"></param>
        /// <param name="color"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool Find(Bitmap image, Bitmap target, Rectangle boxToSearchIn, out Rectangle area, bool color = false, float threshold = 0.9f)
        {
            return Find(image.Clone(boxToSearchIn, image.PixelFormat), target, out area, color, threshold);
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
        /// Converts an image Mat OpenCV to grayscale.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static Mat ConvertToGray(Mat src)
        {
            return src.Channels() switch
            {
                3 => ConvertTo(src, ColorConversionCodes.BGR2GRAY),
                1 => src.Clone(), // déjà en gris
                4 => ConvertTo(src, ColorConversionCodes.BGRA2GRAY),
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
