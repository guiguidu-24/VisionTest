# VisionTest (This project is still in preview)

UI test automation framework powered by computer vision (OpenCV) and OCR (Tesseract).

The primary library is `VisionTest.Core`, which provides screen capture, OCR/image recognition, and input simulation (mouse/keyboard) to drive UI workflows based on what’s visible on screen.

Other projects in this repository (briefly mentioned below) provide a console sample, tests, and a Visual Studio extension for capturing/storing elements.

## VisionTest.Core (library)

High-level capabilities
- Screen capture with DPI-awareness
- OCR text search using Tesseract
- Image template matching using OpenCV
- Input automation (mouse and keyboard)
- Locator abstraction for waiting/clicking visual targets, supporting multiple strategies (text and/or image) and regions

### Target/runtime
- Target Framework: net9.0-windows
- Windows only for the moment (uses Windows-specific APIs and WindowsInput)

### Key namespaces and types
- Recognition
	- `IRecognitionEngine<TTarget>`: contract returning matching rectangles for a given target.
	- `OcrEngine` (TTarget = string): OCR-based search with Tesseract.
		- Options: `OcrOptions` (language, whitelist, LSTM-only, threshold filter, DPI improvement).
		- Languages: `Language` enum with `English` (eng) and `French` (fra). Traineddata files are copied from `tessdata` at build.
	- `ImgEngine` (TTarget = Bitmap): OpenCV template matching.
		- Options: `ImgOptions` (threshold, color match vs grayscale).
- Input
	- Screen: `IScreen`, `Screen` for full-screen or region capture; exposes `ScreenSize` and `ScaleFactor` (DPI scaling).
	- Mouse: `IMouse`, `Mouse` for Move/Click/Down/Up/Scroll; auto-corrects coordinates for DPI.
	- Keyboard: `IKeyboard`, `Keyboard` for key presses and typing.
- Locators
	- `ILocatorV` / `LocatorV`: waits for and optionally clicks a visual target using one or more `SimpleLocatorV` strategies.
	- `SimpleLocatorV`: a concrete descriptor using either a `string` text (OCR) or a `Bitmap` image (template match), plus optional region and options.
- Utils
	- `BitmapExtensions`: `ImproveDpi`, `LoadSafelyImage`.
	- `MatExtensions`: color space helpers (BGRA/Gray) used by OpenCV flow.
	- `RectangleExtensions`: e.g., `Center()`.

### How it works (overview)
1) Capture: `Screen.CaptureScreen()` grabs the desktop bitmap (DPI-aware). Optional `region` crops to a sub-rectangle.
2) Recognize: a recognition engine runs on the captured bitmap:
	 - OCR: `OcrEngine.Find(image, "text to find")` enumerates occurrences by scanning the page with Tesseract in SparseText mode and grouping word boxes.
	 - Image: `ImgEngine.Find(image, referenceBitmap)` uses OpenCV `MatchTemplate` to find matches above a threshold.
3) Act: `LocatorV` orchestrates capture → recognition → wait/return area and can `ClickAsync()` the center via `Mouse`.

### Quickstart

OCR: wait and click a label on screen

```csharp
using VisionTest.Core;
using VisionTest.Core.Recognition;

// Look for the text "OK" (English) anywhere on screen, up to 10 seconds
var locator = new LocatorV(
		text: "OK",
		ocrOption: new OcrOptions(LTSMOnly: true, Lang: Language.English)
);
await locator.ClickAsync(TimeSpan.FromSeconds(10));
```

OCR within a region and just wait for the area

```csharp
using VisionTest.Core;
using VisionTest.Core.Recognition;

var region = new Rectangle(0, 0, 800, 600);
var locator = new LocatorV("Hello World", new OcrOptions(Lang: Language.English), region);
Rectangle area = await locator.WaitForAsync(TimeSpan.FromSeconds(5));
```

Image matching (template)

```csharp
using VisionTest.Core;
using VisionTest.Core.Recognition;
using VisionTest.Core.Utils;

var refImage = BitmapExtensions.LoadSafelyImage(@"C:\\path\\to\\button.png");
var locator = new LocatorV(
		image: refImage,
		imgOption: new ImgOptions(threshold: 0.92f, colorMatch: true)
);
var (success, area) = await locator.TryWaitForAsync(TimeSpan.FromSeconds(3));
if (success)
{
		// do something with area.Value
}
```

Multiple strategies (try OCR OR Image, whichever appears first)

```csharp
using VisionTest.Core;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using VisionTest.Core.Utils;

var img = BitmapExtensions.LoadSafelyImage(@"C:\\path\\to\\icon.png");
var composite = new LocatorV(new []
{
		new SimpleLocatorV(text: "OPEN", ocrOption: new OcrOptions(Lang: Language.English)),
		new SimpleLocatorV(image: img, imgOption: new ImgOptions(0.9f, true))
});
var area = await composite.WaitForAsync(TimeSpan.FromSeconds(8));
```

### Notes and behavior
- DPI scaling: `Screen.ScaleFactor` is applied to mouse moves so logical coordinates match the physical cursor position.
- Tesseract data: `eng.traineddata`, `fra.traineddata`, and `pdf.ttf` under `VisionTest.Core/tessdata` are copied to output. Add other languages if needed.
- OCR mode: `OcrOptions.LTSMOnly` uses LSTM engine for best accuracy on modern models.
- Whitelists: `OcrOptions.WhiteListChar` and `WordWhiteList` bias recognition to improve reliability.
- Pre-filters: `UseThresholdFilter` (adaptive threshold) and `ImproveDPI` can help low-contrast/low-DPI images.
- Timeouts: `WaitForAsync(timeout)` throws on timeout; `TryWaitForAsync(timeout)` returns `(success, area)`.

### Dependencies (VisionTest.Core)
- OpenCvSharp4.Windows (4.10.x)
- OpenCvSharp4.Extensions (4.10.x)
- Tesseract (5.2.0) and Tesseract.Drawing (5.2.0)
- System.Drawing.Common (9.0.x)
- WindowsInput (6.4.x)
- Microsoft.CodeAnalysis.CSharp (4.14.0) [internal use]
- NUnit (3.14.0) [test-related]

## Other projects (brief)
- VisionTest.ConsoleInterop: small console utilities/samples around interop/storage.
- VisionTest.Tests: unit/functional tests and test assets.
- VisionTest.TestsImplementation: example test scripts.
- VisionTest.VSExtension: Visual Studio extension that helps capture/store screen elements.

## Requirements
- Windows
- .NET SDK 9.0

## License
See `LICENSE` for details.
