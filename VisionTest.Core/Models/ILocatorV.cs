using VisionTest.Core.Recognition;

namespace VisionTest.Core.Models
{
    public interface ILocatorV //TODO : an element can have multiple images of reference and image treatment settings for each one
    {
        public Task ClickAsync();
        public Task ClickAsync(TimeSpan timeout);

        public Task<Rectangle> WaitForAsync();
        public Task<Rectangle> WaitForAsync(TimeSpan timeout);

        public Task<(bool success, Rectangle? area)> TryWaitForAsync();
        public Task<(bool success, Rectangle? area)> TryWaitForAsync(TimeSpan timeout);
    }
}
