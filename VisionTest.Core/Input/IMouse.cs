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
        void MoveTo(int x, int y);

        /// <summary>
        /// Moves the mouse relative to its current position.
        /// </summary>
        void MoveBy(int deltaX, int deltaY);

        /// <summary>
        /// Performs a left click (press and release).
        /// </summary>
        void LeftClick();

        /// <summary>
        /// Performs a right click (press and release).
        /// </summary>
        void RightClick();

        /// <summary>
        /// Performs a double click with the left button.
        /// </summary>
        void DoubleClick();

        /// <summary>
        /// Presses the left button down (useful for dragging).
        /// </summary>
        void LeftDown();

        /// <summary>
        /// Releases the left button (useful for dragging).
        /// </summary>
        void LeftUp();

        /// <summary>
        /// Presses the right button down.
        /// </summary>
        void RightDown();

        /// <summary>
        /// Releases the right button.
        /// </summary>
        void RightUp();

        /// <summary>
        /// Scrolls the mouse wheel vertically.
        /// Positive is up, negative is down.
        /// </summary>
        void ScrollVertical(int delta);

        /// <summary>
        /// Scrolls the mouse wheel horizontally.
        /// Positive is right, negative is left.
        /// </summary>
        void ScrollHorizontal(int delta);
    }
}
