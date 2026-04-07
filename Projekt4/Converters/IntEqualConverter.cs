using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Projekt4.Converters;

/// <summary>
/// Konwerter dla RadioButton - porównuje wartość z parametrem.
/// Użycie: IsChecked="{Binding WybranyKat, Converter={StaticResource IntEqualConverter}, ConverterParameter=90}"
/// </summary>
public class IntEqualConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue && parameter is string strParam && int.TryParse(strParam, out var paramValue))
            return intValue == paramValue;
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true && parameter is string strParam && int.TryParse(strParam, out var paramValue))
            return paramValue;
        return 90; // domyślna wartość
    }
}
