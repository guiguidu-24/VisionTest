using CommunityToolkit.Mvvm.ComponentModel;
using System.Drawing;

namespace VisionTest.UI.ViewModels
{
    public partial class DetectedElementViewModel : ObservableObject
    {
        public string Text { get; }
        public Rectangle Bounds { get; }

        public string BoundsDisplay =>
            $"X: {Bounds.X}, Y: {Bounds.Y} [{Bounds.Width} x {Bounds.Height}]";

        // Placeholder for confidence if your OCR engine provides it (0-100%)
        [ObservableProperty] private float _confidence;
        public string ConfidenceDisplay => $"{Confidence:P1}"; // Formats as 95.5%

        // State for the UI to know if this is the one highlighted in the center
        [ObservableProperty] private bool _isHighlighted;

        public DetectedElementViewModel(Rectangle bounds, string text, float confidence = 1.0f)
        {
            Bounds = bounds;
            Text = text;
            Confidence = confidence;
        }
    }
}
