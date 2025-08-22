using WindowsInput.Events;
using WindowsInput;

namespace VisionTest.Core.Input;

public class Mouse : IMouse
{
    private readonly IScreen _screen = new Screen();

    
    public Task DoubleClick()
    {
        return Simulate.Events().DoubleClick(ButtonCode.Left).Invoke();
    }

    public Task LeftClick()
    {
        return Simulate.Events().Click(ButtonCode.Left).Invoke();
    }

    public Task LeftDown()
    {
        return Simulate.Events().Hold(ButtonCode.Left).Invoke();
    }

    public Task LeftUp()
    {
        return Simulate.Events().Release(ButtonCode.Left).Invoke();
    }

    public Task MoveBy(int deltaX, int deltaY)
    {
        return Simulate.Events().MoveBy(CoordinateCorrection(deltaX), CoordinateCorrection(deltaY)).Invoke();
    }

    public Task MoveTo(int x, int y)
    {
        return Simulate.Events().MoveTo(CoordinateCorrection(x), CoordinateCorrection(y)).Invoke();
    }

    public Task RightClick()
    {
        return Simulate.Events().Click(ButtonCode.Right).Invoke();
    }

    public Task RightDown()
    {
        return Simulate.Events().Hold(ButtonCode.Right).Invoke();
    }

    public Task RightUp()
    {
        return Simulate.Events().Release(ButtonCode.Right).Invoke();
    }

    [Obsolete("⚠️ Not tested — use with caution.", false)]
    public Task ScrollHorizontal(int delta) //TODO Test scrolling
    {
        return Simulate.Events().Scroll(ButtonCode.None, ButtonScrollDirection.Right, delta).Invoke();
    }

    [Obsolete("⚠️ Not tested — use with caution.", false)]
    public Task ScrollVertical(int delta)
    {
        return Simulate.Events().Scroll( ButtonCode.None, ButtonScrollDirection.Up, delta).Invoke();
    }

    private int CoordinateCorrection(int coordinate)
    {
        return (int)(coordinate / _screen.ScaleFactor);
    }
}