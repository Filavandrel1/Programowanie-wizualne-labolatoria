using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Projekt9.ViewModels;

/// <summary>
/// Konwerter bool ↔ int (0/1) dla wiązania <see cref="bool"/> do
/// <c>ComboBox.SelectedIndex</c>. <c>true</c> -> 0 (pierwsza opcja, np.
/// "Wyrażam zgodę"), <c>false</c> -> 1 (druga opcja).
/// </summary>
public class BoolToIndexConverter : IValueConverter
{
    public static readonly BoolToIndexConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b) return b ? 0 : 1;
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i) return i == 0;
        return true;
    }
}
