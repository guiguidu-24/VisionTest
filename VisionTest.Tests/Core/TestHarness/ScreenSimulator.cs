using System;
using System.Drawing;
using VisionTest.Core.Input;

namespace VisionTest.Tests.Core.TestHarness;

public class ScreenSimulator : IScreen
{
    public Bitmap? NextCapture { private get; set; }
    public Size ScreenSize => NextCapture?.Size ?? throw new InvalidOperationException("NextCapture is not set or has no size.");
    public float ScaleFactor => 1.0f;

    public Bitmap CaptureScreen()
    {
        return NextCapture ?? throw new InvalidOperationException("NextCapture is not set.");
    }
}
