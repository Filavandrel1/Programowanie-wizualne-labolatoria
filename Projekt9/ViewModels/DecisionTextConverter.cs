using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Projekt9.ViewModels;

/// <summary>
/// Konwerter <see cref="bool"/> -> tekst decyzji prodziekana,
/// używany na ekranie podglądu zapisanych formularzy.
/// </summary>
public class DecisionTextConverter : IValueConverter
{
    public static readonly DecisionTextConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? "Wyrażam zgodę na przeprowadzenie egzaminu komisyjnego"
                    : "Nie wyrażam zgody na przeprowadzenie egzaminu komisyjnego";
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
