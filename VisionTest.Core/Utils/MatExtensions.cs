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
}
