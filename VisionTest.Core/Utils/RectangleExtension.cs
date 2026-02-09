using VisionTest.Core.Input;

namespace VisionTest.Core.Utils;

public static class RectangleExtensions
{
    private static readonly IMouse _mouse = new Mouse();

    public static Point Center(this Rectangle rect)
    {
        return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    }

    public static Rectangle RightClick(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        _mouse.RightClick();
        return area;
    }

    public static Rectangle DoubleClick(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        _mouse.DoubleClick();
        return area;
    }

    public static Rectangle Hover(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        return area;
    }

    public static Rectangle Click(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        _mouse.LeftClick();
        return area;
    }

    public static Point UpperLeft(this Rectangle rect) => new Point(rect.X, rect.Y);
    public static Point LowerRight(this Rectangle rect) => new Point(rect.X + rect.Width, rect.Y + rect.Height);
    public static Point UpperRight(this Rectangle rect) => new Point(rect.X + rect.Width, rect.Y);
    public static Point LowerLeft(this Rectangle rect) => new Point(rect.X, rect.Y + rect.Height);

}
