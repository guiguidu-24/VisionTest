

namespace VisionTest.Core.Utils
{
    public static class RectangleExtensions
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static bool Inside(this Rectangle rectangle1, Rectangle rectangle2)
        {
            return rectangle1.X >= rectangle2.X &&
        rectangle1.Y >= rectangle2.Y &&
        rectangle1.Right <= rectangle2.Right &&
        rectangle1.Bottom <= rectangle2.Bottom;
        }
    }
}
