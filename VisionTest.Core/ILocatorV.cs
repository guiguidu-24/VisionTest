namespace VisionTest.Core;

public interface ILocatorV //TODO : an element can have multiple images of reference and image treatment settings for each one
{
    public Task ClickAsync();
    public Task ClickAsync(TimeSpan timeout);

    public Task RightClickAsync();
    public Task RightClickAsync(TimeSpan timeout);

    public Task DoubleClickAsync();
    public Task DoubleClickAsync(TimeSpan timeout);

    public Task HoverAsync();
    public Task HoverAsync(TimeSpan timeout);

    public Task<ScreenElement> WaitForAsync();
    public Task<ScreenElement> WaitForAsync(TimeSpan timeout);

    public Task<(bool success, ScreenElement? area)> TryWaitForAsync();
    public Task<(bool success, ScreenElement? area)> TryWaitForAsync(TimeSpan timeout);
}
