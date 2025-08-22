using OpenCvSharp;

namespace VisionTest.Core.Utils;

public static class MatExtensions
{
    public static Mat ConvertToBGRA(this Mat src)
    {
        return src.Channels() switch
        {
            3 => ConvertTo(src, ColorConversionCodes.BGR2BGRA),
            1 => ConvertTo(src, ColorConversionCodes.GRAY2BGRA),
            4 => src.Clone(), // déjà en BGRA
            _ => throw new NotSupportedException($"Nombre de canaux non supporté : {src.Channels()}")
        };
    }

    public static Mat ConvertToGray(this Mat src)
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
    /// Converts an OpenCV Mat image to another color space using the specified color conversion code.
    /// </summary>
    /// <param name="src">The source Mat image to convert.</param>
    /// <param name="code">The color conversion code to use for the transformation.</param>
    /// <returns>A new Mat image in the target color space.</returns>
    private static Mat ConvertTo(Mat src, ColorConversionCodes code)
    {
        Mat dst = new();
        Cv2.CvtColor(src, dst, code);
        return dst;
    }
}
