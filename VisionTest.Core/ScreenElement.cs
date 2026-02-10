using VisionTest.Core.Input;
using VisionTest.Core.Utils;

namespace VisionTest.Core;

public class ScreenElement
{
    public Rectangle Bounds { get; }

    private readonly IMouse _mouse;

    public ScreenElement(Rectangle bounds, IMouse mouse)
    {
        Bounds = bounds;
        _mouse = mouse;
    }

    public static implicit operator Rectangle(ScreenElement element)
    {
        return element?.Bounds ?? Rectangle.Empty;
    }

    public ScreenElement RightClick()
    {
        _mouse.MoveTo(Bounds.Center().X, Bounds.Center().Y);
        _mouse.RightClick();
        return this;
    }

    public ScreenElement DoubleClick()
    {
        _mouse.MoveTo(Bounds.Center().X, Bounds.Center().Y);
        _mouse.DoubleClick();
        return this;
    }

    public ScreenElement Hover()
    {
        _mouse.MoveTo(Bounds.Center().X, Bounds.Center().Y);
        return this;
    }

    public ScreenElement Click()
    {
        _mouse.MoveTo(Bounds.Center().X, Bounds.Center().Y);
        _mouse.LeftClick();
        return this;
    }
}
