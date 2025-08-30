using VisionTest.Core.Input;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using VisionTest.Core.Utils;

namespace VisionTest.Core;

public class LocatorV : ILocatorV
{
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(15);

    private readonly IScreen _screen;
    private readonly IMouse _mouse = new Mouse();

    private SimpleLocatorV[] simpleLocators;

    public LocatorV(SimpleLocatorV[] simpleLocators, IScreen? screen = null)
    {
        ArgumentNullException.ThrowIfNull(simpleLocators, nameof(simpleLocators));
        if (simpleLocators.Length == 0)
            throw new ArgumentException("At least one SimpleLocatorV must be provided.", nameof(simpleLocators));

        this.simpleLocators = simpleLocators;

        if (screen is not null)
            _screen = screen;
        else
            _screen = new Input.Screen();
    }

    public LocatorV(SimpleLocatorV simpleLocator, IScreen? screen = null) : this([simpleLocator], screen) { }
    public LocatorV(string text, OcrOptions? ocrOption = null, Rectangle? region = null, IScreen? screen = null) 
        : this(new SimpleLocatorV(text: text, ocrOption: ocrOption, region: region), screen) { }
    public LocatorV(Bitmap image, ImgOptions? imgOption = null, Rectangle? region = null, IScreen? screen = null) 
        : this(new SimpleLocatorV(image: image, imgOption: imgOption, region: region), screen) { }
    

    public async Task ClickAsync()
    {
        var area = await WaitForAsync();
        await _mouse.MoveTo(area.Center().X, area.Center().Y);
        await _mouse.LeftClick();
    }

    public async Task ClickAsync(TimeSpan timeout)
    {
        var area = await WaitForAsync(timeout);
        await _mouse.MoveTo(area.Center().X, area.Center().Y);
        await _mouse.LeftClick();
    }

    public Task<(bool success, Rectangle? area)> TryWaitForAsync()
    {
        return TryWaitForAsync(_defaultTimeout);
    }

    public async Task<(bool success, Rectangle? area)> TryWaitForAsync(TimeSpan timeout)
    {
        var cts = new CancellationTokenSource(timeout);
        var tasks = new Task<Rectangle?>[simpleLocators.Length];

        for (int i = 0; i < simpleLocators.Length; i++)
        {
            var locator = simpleLocators[i];
            if (locator.Text is not null)
            {
                tasks[i] = Task.Run(async () =>
                {
                    return await WaitForAsync(new OcrEngine(locator.OcrOption), locator.Text, locator.Region, cts.Token);
                }, cts.Token);
            }
            else if (locator.Image is not null)
            {
                tasks[i] = Task.Run(async () =>
                {
                    return await WaitForAsync(new ImgEngine(locator.ImgOption), locator.Image, locator.Region, cts.Token);
                }, cts.Token);
            }
        }

        var taskFinished = await Task.WhenAny(tasks);
        cts.Cancel(); // Cancel all other tasks once one is finished
        var result = await taskFinished;
        if (result.HasValue)
            return (true, result.Value);
        
        return (false, null);
    }

    public Task<Rectangle> WaitForAsync()
    {
        return WaitForAsync(_defaultTimeout);
    }

    /// <summary>
    /// Waits for a specific screen element to appear on the screen asynchronously.
    /// </summary>
    /// <param name="timeout">After the timeout, the Task completes</param>
    /// <returns>The area around the target if found, null if not found</returns>
    public async Task<Rectangle> WaitForAsync(TimeSpan timeout)
    {
        if(await TryWaitForAsync(timeout) is (true, var area))
            return area!.Value;
        throw new TimeoutException("Object not found within the specified timeout.");
    }

    private async Task<Rectangle?> WaitForAsync<TTarget>(IRecognitionEngine<TTarget> engine, TTarget target, Rectangle? box, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(engine, nameof(engine));
        ArgumentNullException.ThrowIfNull(target, nameof(target));


        const int interval = 100; // Check every 100 milliseconds

        Bitmap image;

        // Wait for the text to appear on the screen
        IEnumerable<Rectangle> recognitionResult;
        do
        {
            image = _screen.CaptureScreen();
            if (box.HasValue)
            {
                image = image.Clone(box.Value, image.PixelFormat); // Crop the image to the specified box if provided
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }
            await Task.Delay(interval, cancellationToken);
        }
        while (!(recognitionResult = engine.Find(image, target)).Any());


        // TODO: Warning if more than one result is found
        return recognitionResult.First();
    }
}
