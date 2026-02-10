# VisionTest

UI test automation framework powered by computer vision (OpenCV) and OCR (Tesseract).

The primary library is `VisionTest.Core`, which provides screen capture, OCR/image recognition, and input simulation (mouse/keyboard) to drive UI workflows based on what’s visible on screen.

## VisionTest.Core (library)

High-level capabilities
- Screen capture with DPI-awareness
- OCR text search using Tesseract
- Image template matching using OpenCV
- Input automation (mouse and keyboard)
- Locator abstraction for waiting/clicking visual targets, supporting multiple strategies (text and/or image) and regions

## VisionTest Import

To install the package, use the following command:

```bash
dotnet add package VisionTest.Core --version 1.0.0
```

### Target/runtime
- Target Framework: .NET Standard 2.0
- Windows only for the moment

### Key namespaces and types
- Recognition (`VisionTest.Core.Recognition`)
	- `IRecognitionEngine<TTarget>`: contract returning matching rectangles for a given target.
	- `OcrEngine` (TTarget = string): OCR-based search using Tesseract.
		- Options: `OcrOptions` (whiteListChar, blackListChar, wordList, lang, PSM, OEM, useDictionnary, regexPattern).
		- Languages: `Language` enum with `English` (eng) and `French` (fra).
		- Page segmentation modes: `PageSegmentationMode` enum (Auto, SparseText, SingleBlock, etc.).
		- Engine modes: `OcrEngineMode` enum (Auto, Legacy, LSTM, LegacyAndLSTM).
	- `ImgEngine` (TTarget = Bitmap): OpenCV template matching using `MatchTemplate` with `CCoeffNormed`.
		- Options: `ImgOptions` (threshold, colorMatch). Default threshold is 0.9f.
- Input (`VisionTest.Core.Input`)
	- Screen: `IScreen`, `WinScreen` for screen capture; exposes `ScreenSize` and `ScaleFactor` (DPI scaling).
	- Mouse: `IMouse`, `Mouse` for MoveTo/MoveBy/Click/Down/Up/Scroll; auto-corrects coordinates for DPI using SharpHook.
	- Keyboard: `IKeyboard`, `Keyboard` for key presses, text typing, and modified keystrokes using SharpHook.
- Locators (`VisionTest.Core`)
	- `ILocatorV` / `LocatorV`: waits for and clicks a visual target using one or more `SimpleLocatorV` strategies. Supports ClickAsync, RightClickAsync, DoubleClickAsync, HoverAsync, WaitForAsync, and TryWaitForAsync.
	- `SimpleLocatorV` (record class in `VisionTest.Core.Models`): a concrete descriptor using either a `string` text (OCR) or a `Bitmap` image (template match), plus optional region and options.
	- `ScreenElement`: represents a found element with its bounds (Rectangle) and provides chaining methods (Click, RightClick, DoubleClick, Hover). Implicitly converts to Rectangle.
- Utils (`VisionTest.Core.Utils`)
	- `BitmapExtensions`: `LoadSafelyImage` (safe file loading).
	- `MatExtensions`: color space conversion helpers (`ConvertToBGRA`, `ConvertToGray`) for OpenCV operations.
	- `RectangleExtensions`: position helpers (`Center`, `UpperLeft`, `LowerRight`, `UpperRight`, `LowerLeft`) and `ToScreenElement` (converts Rectangle to ScreenElement).
	- `RectangleFactory`: `FromPoints` (creates Rectangle from two points).

### How it works (overview)
1) Capture: `Screen.CaptureScreen()` grabs the desktop bitmap (DPI-aware). Optional `region` crops to a sub-rectangle.
2) Recognize: a recognition engine runs on the captured bitmap:
	 - OCR: `OcrEngine.Find(image, "text to find")` enumerates occurrences by scanning the page with Tesseract in SparseText mode and grouping word boxes.
	 - Image: `ImgEngine.Find(image, referenceBitmap)` uses OpenCV `MatchTemplate` to find matches above a threshold.
3) Act: `LocatorV` orchestrates capture → recognition → wait/return area and can `ClickAsync()` the center via `Mouse`.

### Quickstart

OCR: Wait and Click a Label

```csharp
using VisionTest.Core;
using VisionTest.Core.Recognition;

// Define the locator strategy
var locator = new LocatorV(
    text: "OK", 
    ocrOption: new OcrOptions(LTSMOnly: true, Lang: Language.English));

// Find and click in one go (waits up to 10s by default)
await locator.ClickAsync(TimeSpan.FromSeconds(10));
```

OCR within a Region

```csharp
using VisionTest.Core;
using VisionTest.Core.Recognition;

var region = new Rectangle(0, 0, 800, 600);
var locator = new LocatorV("Hello World", new OcrOptions(Lang: Language.English), region);

// Returns a ScreenElement
ScreenElement element = await locator.WaitForAsync(TimeSpan.FromSeconds(5));

// Because of implicit conversion, you can pass it to any method expecting a Rectangle
float centerX = element.X + (element.Width / 2);
```

Image Matching (Template)

```csharp
using VisionTest.Core;
using VisionTest.Core.Utils;

// Clean path handling
string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "button.png");
using var refImage = new Bitmap(path);

var locator = new LocatorV(
    image: refImage, 
    imgOption: new ImgOptions(threshold: 0.92f, colorMatch: true));

var (success, element) = await locator.TryWaitForAsync(TimeSpan.FromSeconds(3));

if (success)
{
    // Chain actions directly on the element
    element.Hover().Click();
}
```

Advanced: Dynamic Interaction Areas

```csharp
// 1. Find two elements
var start = await new LocatorV("Header").WaitForAsync();
var end = await new LocatorV("Footer").WaitForAsync();

// 2. Compute a new area between them (Rectangle)
var customArea = RectangleFactory.FromPoints(start.Bounds.Location, end.Bounds.Location);

// 3. Turn it back into a ScreenElement using an existing locator's mouse context
var actionableZone = customArea.ToScreenElement(start); 

actionableZone.DoubleClick();
```

### Dependencies (VisionTest.Core)
- OpenCvSharp4.Windows (4.10.0.20241108)
- OpenCvSharp4.Extensions (4.10.0.20241108)
- Tesseract (5.2.0)
- Tesseract.Drawing (5.2.0)
- System.Drawing.Common (9.0.4)
- SharpHook (7.0.1)
- Microsoft.CodeAnalysis.CSharp (4.14.0)

## Other projects (brief)
- VisionTest.ConsoleInterop: small console utilities/samples around interop/storage.
- VisionTest.Tests: unit/functional tests and test assets.
- VisionTest.TestsImplementation: example test scripts.
- VisionTest.VSExtension: Visual Studio extension that helps capture/store screen elements.

## License
See `LICENSE` for details.
