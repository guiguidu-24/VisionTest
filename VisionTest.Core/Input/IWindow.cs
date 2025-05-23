

namespace VisionTest.Core.Input
{
    public interface IWindow
    {
        /// <summary>
        /// Gets the title of the window.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the position and size of the window.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        /// Brings the window to the foreground and gives it focus.
        /// </summary>
        void Activate();

        /// <summary>
        /// Maximizes the window.
        /// </summary>
        void Maximize();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();

        /// <summary>
        /// Resizes the window to a new size.
        /// </summary>
        void Resize(int width, int height);
    }
}
