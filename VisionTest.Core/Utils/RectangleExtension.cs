using VisionTest.Core.Input;

namespace VisionTest.Core.Utils;

public static class RectangleExtensions
{
    private static readonly IMouse _mouse = new Mouse();

    public static Point Center(this Rectangle rect)
    {
        return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    }

    public static void RightClick(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        _mouse.RightClick();
    }

    public static void DoubleClick(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        _mouse.DoubleClick();
    }

    public static void Hover(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
    }

    public static void Click(this Rectangle area)
    {
        _mouse.MoveTo(area.Center().X, area.Center().Y);
        _mouse.LeftClick();
    }
}
