
namespace VisionTest.Core.Recognition
{
    /// <summary>
    /// Represents a generic recognition engine that can search for a specific target inside an image.
    /// </summary>
    /// <typeparam name="TTarget">The type of target to search for, e.g., string for OCR, Bitmap for image matching.</typeparam>
    public interface IRecognitionEngine<TDomain,TTarget>
    {
        /// <summary>
        /// Searches for the given target in the image and returns all matching regions.
        /// </summary>
        IEnumerable<Rectangle> Find(TDomain domain, TTarget target);
    }
}
