namespace VisionTest.Core.Input;

public interface IScreen
{
    /// <summary>
    /// Gets the size of the primary screen in pixels.
    /// </summary>
    Size ScreenSize { get; }

    /// <summary>
    /// Gets the scale factor (DPI scaling) of the primary screen.
    /// For example, 1.0 means 100%, 1.25 means 125%.
    /// </summary>
    float ScaleFactor { get; }

    /// <summary>
    /// Captures the entire screen and returns a Bitmap image.
    /// </summary>
    Bitmap CaptureScreen();
}
