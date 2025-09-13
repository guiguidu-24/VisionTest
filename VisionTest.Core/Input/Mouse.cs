using SharpHook;
using SharpHook.Data;
using System.Runtime.InteropServices;

namespace VisionTest.Core.Input;

public class Mouse : IMouse
{
    private const int defaultDelayBetweenClicksMs = 20;

    private readonly IScreen _screen = new WinScreen();
    private readonly EventSimulator _simulator = new();

    public void DoubleClick()
    {
        LeftClick();
        Thread.Sleep(defaultDelayBetweenClicksMs);
        LeftClick();
    }

    public void LeftClick()
    {
        LeftDown();
        Thread.Sleep(defaultDelayBetweenClicksMs);
        LeftUp();
    }

    public void LeftDown()
    {
        _simulator.SimulateMousePress(MouseButton.Button1);
    }

    public void LeftUp()
    {
        _simulator.SimulateMouseRelease(MouseButton.Button1);
    }

    public void MoveBy(int deltaX, int deltaY)
    {
        _simulator.SimulateMouseMovementRelative(CoordinateCorrection(deltaX), CoordinateCorrection(deltaY));
    }

    public void MoveTo(int x, int y)
    {
        _simulator.SimulateMouseMovement(CoordinateCorrection(x), CoordinateCorrection(y));
    }

    public void RightClick()
    {
        RightDown();
        Thread.Sleep(defaultDelayBetweenClicksMs);
        RightUp();
    }

    public void RightDown()
    {
        _simulator.SimulateMousePress(MouseButton.Button2);
    }

    public void RightUp()
    {
        _simulator.SimulateMouseRelease(MouseButton.Button2);
    }

    public void ScrollHorizontal(int delta) //TODO Test scrolling
    {
        int rotation;
        var type = MouseWheelScrollType.UnitScroll;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            rotation = 120 * delta;                     // Windows “step”
                                                        // type ignored on Windows
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            rotation = delta;                           // small values on macOS
            type = MouseWheelScrollType.BlockScroll;    // better for line scrolling
        }
        else
        {
            rotation = 100 * delta;                     // guideline for X11
                                                        // type ignored on Linux
        }

        _simulator.SimulateMouseWheel((short)rotation, MouseWheelScrollDirection.Horizontal, type);
    }

    public void ScrollVertical(int lines)
    {
        int rotation;
        var type = MouseWheelScrollType.UnitScroll;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            rotation = 120 * lines;                     // Windows “step”
                                                        // type ignored on Windows
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            rotation = lines;                           // small values on macOS
            type = MouseWheelScrollType.BlockScroll;    // better for line scrolling
        }
        else
        {
            rotation = 100 * lines;                     // guideline for X11
                                                        // type ignored on Linux
        }

        _simulator.SimulateMouseWheel((short)rotation, MouseWheelScrollDirection.Vertical, type);
    }

    private short CoordinateCorrection(int coordinate)
    {
        return (short)(coordinate / _screen.ScaleFactor);
    }
}