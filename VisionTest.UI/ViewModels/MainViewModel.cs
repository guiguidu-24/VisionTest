using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using VisionTest.Core.Recognition;
using VisionTest.UI.Helpers;

namespace VisionTest.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public OcrSettingsViewModel OcrSettings { get; } = new();
    public ObservableCollection<DetectedElementViewModel> DetectedElements { get; } = new();

    private string? _currentFilePath;

    [ObservableProperty]
    private Bitmap? _loadedImage;

    [ObservableProperty]
    private bool _isImageLoaded;

    [ObservableProperty]
    private object? _selectedElement;

    [RelayCommand]
    public async Task ImportImageAsync()
    {
        // 1. Get the TopLevel (the window/screen container)
        var topLevel = TopLevel.GetTopLevel(
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

        if (topLevel == null) return;

        // 2. Open the Picker
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Image for OCR Test",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll } // Built-in filter for jpg, png, etc.
        });

        if (files.Count > 0)
        {
            // 3. Get the path and load the image
            string filePath = files[0].Path.LocalPath;
            await LoadImageToDisplay(filePath);
        }
    }
    [ObservableProperty] private double _imageWidth;
    [ObservableProperty] private double _imageHeight;
    private async Task LoadImageToDisplay(string path)
    {
        _currentFilePath = path;

        using var stream = File.OpenRead(path);
        LoadedImage = new Bitmap(stream);
        IsImageLoaded = true;

        // Set the dimensions for the Canvas to match the Pixels
        ImageWidth = LoadedImage.Size.Width;
        ImageHeight = LoadedImage.Size.Height;

        // Clear previous results
        DetectedElements.Clear();
    }

    [RelayCommand]
    private async Task RunOcrAsync()
    {
        if (string.IsNullOrEmpty(_currentFilePath) || LoadedImage is null) return;

        DetectedElements.Clear();

        var results = await Task.Run(() =>
        {
            var ocrEngine = new OcrEngine(OcrSettings.CreateOptions());
            using var systemImage = LoadedImage.ToSystemDrawingBitmap();
            return ocrEngine.Find(systemImage, OcrSettings.TargetText);
        });
        
        foreach (var res in results)
        {
            DetectedElements.Add(new DetectedElementViewModel(res, OcrSettings.TargetText));
        }
    }
}