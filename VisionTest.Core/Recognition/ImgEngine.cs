using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Windows.Media;


namespace VisionTest.Core.Recognition
{
    public class ImgEngine : IRecognitionEngine<Bitmap,Bitmap>
    {
        /// <summary>
        /// The threshold for template matching.
        /// </summary>
        public float Threshold { get; set; } = 0.9f;
        public bool ColorMatch { get; set; } = true;

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
            using Mat bigImage = color ? ConvertToBGRA(image.ToMat()) : ConvertToGray(image.ToMat());
            using Mat smallImage = color ? ConvertToBGRA(target.ToMat()) : ConvertToGray(target.ToMat());


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

        /// <summary>
        /// Finds all occurrences of the target image in the source image using template matching.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<Rectangle> Find(Bitmap image, Bitmap target)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (target is null) throw new ArgumentNullException(nameof(target));


            using var sourceMat = ColorMatch ? ConvertToBGRA(image.ToMat()) : ConvertToGray(image.ToMat());
            using var templateMat = ColorMatch ? ConvertToBGRA(target.ToMat()) : ConvertToGray(target.ToMat());
            using var result = new Mat();

            // MatchTemplate method: CV_TM_CCOEFF_NORMED gives good normalized results
            Cv2.MatchTemplate(sourceMat, templateMat, result, TemplateMatchModes.CCoeffNormed);

            var matches = new List<Rectangle>();
            var templateSize = new OpenCvSharp.Size(templateMat.Width, templateMat.Height);

            while (true)
            {
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                if (maxVal < Threshold)
                    break;

                var matchRect = new Rectangle(maxLoc.X, maxLoc.Y, templateMat.Width, templateMat.Height);
                matches.Add(matchRect);

                // Suppress the found area to avoid duplicate detection (flood fill with a low value)
                Cv2.FloodFill(result, maxLoc, new Scalar(0), out _, new Scalar(0.1), new Scalar(1.0));
            }

            return matches;
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
