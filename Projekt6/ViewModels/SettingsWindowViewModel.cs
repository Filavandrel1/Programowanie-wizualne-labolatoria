using CommunityToolkit.Mvvm.ComponentModel;
using Projekt6.Models;

namespace Projekt6.ViewModels;

public partial class SettingsWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string _boardXText = "";
    [ObservableProperty] private string _boardYText = "";
    [ObservableProperty] private string _timeSecondsText = "";
    [ObservableProperty] private string _dydelfyText = "";
    [ObservableProperty] private string _szopyText = "";
    [ObservableProperty] private string _krokodyleText = "";

    [ObservableProperty] private string? _boardXError;
    [ObservableProperty] private string? _boardYError;
    [ObservableProperty] private string? _timeSecondsError;
    [ObservableProperty] private string? _dydelfyError;
    [ObservableProperty] private string? _szopyError;
    [ObservableProperty] private string? _krokodyleError;

    public SettingsWindowViewModel()
    {
        var s = GameSettings.Instance;
        _boardXText = s.BoardX.ToString();
        _boardYText = s.BoardY.ToString();
        _timeSecondsText = s.TimeSeconds.ToString();
        _dydelfyText = s.Dydelfy.ToString();
        _szopyText = s.Szopy.ToString();
        _krokodyleText = s.Krokodyle.ToString();
    }

    private static string? Validate(string text, int min, int max, string label, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text))
            return $"{label}: pole nie moze byc puste";
        if (!int.TryParse(text.Trim(), out value))
            return $"{label}: wprowadz liczbe calkowita";
        if (value < min || value > max)
            return $"{label}: dozwolony zakres {min}-{max}";
        return null;
    }

    partial void OnBoardXTextChanged(string value)
    {
        BoardXError = Validate(value, GameSettings.MinBoard, GameSettings.MaxBoard, "X", out var v);
        if (BoardXError is null) GameSettings.Instance.BoardX = v;
    }

    partial void OnBoardYTextChanged(string value)
    {
        BoardYError = Validate(value, GameSettings.MinBoard, GameSettings.MaxBoard, "Y", out var v);
        if (BoardYError is null) GameSettings.Instance.BoardY = v;
    }

    partial void OnTimeSecondsTextChanged(string value)
    {
        TimeSecondsError = Validate(value, GameSettings.MinTime, GameSettings.MaxTime, "Czas", out var v);
        if (TimeSecondsError is null) GameSettings.Instance.TimeSeconds = v;
    }

    partial void OnDydelfyTextChanged(string value)
    {
        DydelfyError = Validate(value, GameSettings.MinDydelfy, GameSettings.MaxDydelfy, "Dydelfy", out var v);
        if (DydelfyError is null) GameSettings.Instance.Dydelfy = v;
    }

    partial void OnSzopyTextChanged(string value)
    {
        SzopyError = Validate(value, GameSettings.MinSzopy, GameSettings.MaxSzopy, "Szopy", out var v);
        if (SzopyError is null) GameSettings.Instance.Szopy = v;
    }

    partial void OnKrokodyleTextChanged(string value)
    {
        KrokodyleError = Validate(value, GameSettings.MinKrokodyle, GameSettings.MaxKrokodyle, "Krokodyle", out var v);
        if (KrokodyleError is null) GameSettings.Instance.Krokodyle = v;
    }
}
