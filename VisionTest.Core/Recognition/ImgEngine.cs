using OpenCvSharp;
using OpenCvSharp.Extensions;
using VisionTest.Core.Utils;


namespace VisionTest.Core.Recognition
{
    public class ImgEngine : IRecognitionEngine<Bitmap>
    {
        private float threshold;
        private bool colorMatch;
        public ImgEngine(ImgOptions options)
        {
            threshold = options.Threshold;
            colorMatch = options.ColorMatch;
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


            using var sourceMat = colorMatch ? image.ToMat().ConvertToBGRA() : image.ToMat().ConvertToGray();
            using var templateMat = colorMatch ? target.ToMat().ConvertToBGRA() : target.ToMat().ConvertToGray();
            using var result = new Mat();

            // MatchTemplate method: CV_TM_CCOEFF_NORMED gives good normalized results
            Cv2.MatchTemplate(sourceMat, templateMat, result, TemplateMatchModes.CCoeffNormed);

            var matches = new List<Rectangle>();
            var templateSize = new OpenCvSharp.Size(templateMat.Width, templateMat.Height);

            while (true)
            {
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                if (maxVal < threshold)
                    break;

                var matchRect = new Rectangle(maxLoc.X, maxLoc.Y, templateMat.Width, templateMat.Height);
                matches.Add(matchRect);

                // Suppress the found area to avoid duplicate detection (flood fill with a low value)
                Cv2.FloodFill(result, maxLoc, new Scalar(0), out _, new Scalar(0.1), new Scalar(1.0));
            }

            return matches;
        }
    }
}
