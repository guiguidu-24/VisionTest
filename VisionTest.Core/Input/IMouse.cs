using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionTest.Core.Input
{
    public interface IMouse
    {
        // <summary>
        /// Moves the mouse to the absolute screen coordinates.
        /// </summary>
        Task MoveTo(int x, int y);

        /// <summary>
        /// Moves the mouse relative to its current position.
        /// </summary>
        Task MoveBy(int deltaX, int deltaY);

        /// <summary>
        /// Performs a left click (press and release).
        /// </summary>
        Task LeftClick();

        /// <summary>
        /// Performs a right click (press and release).
        /// </summary>
        Task RightClick();

        /// <summary>
        /// Performs a double click with the left button.
        /// </summary>
        Task DoubleClick();

        /// <summary>
        /// Presses the left button down (useful for dragging).
        /// </summary>
        Task LeftDown();

        /// <summary>
        /// Releases the left button (useful for dragging).
        /// </summary>
        Task LeftUp();

        /// <summary>
        /// Presses the right button down.
        /// </summary>
        Task RightDown();

        /// <summary>
        /// Releases the right button.
        /// </summary>
        Task RightUp();

        /// <summary>
        /// Scrolls the mouse wheel vertically.
        /// Positive is up, negative is down.
        /// </summary>
        Task ScrollVertical(int delta);

        /// <summary>
        /// Scrolls the mouse wheel horizontally.
        /// Positive is right, negative is left.
        /// </summary>
        Task ScrollHorizontal(int delta);
    }
}
