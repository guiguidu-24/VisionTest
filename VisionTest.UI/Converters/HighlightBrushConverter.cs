using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace VisionTest.UI.Converters;

public class HighlightBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isHighlighted = value is bool b && b;
        return isHighlighted ? Brushes.Lime : Brushes.Red;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
