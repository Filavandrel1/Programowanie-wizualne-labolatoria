using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt2.ViewModels;

public partial class PlatnoscViewModel : ViewModelBase
{
    private readonly Action<string> _navigate;

    [ObservableProperty]
    private bool _isBlik;

    [ObservableProperty]
    private bool _isKarta;

    [ObservableProperty]
    private bool _isGotowka;

    [ObservableProperty]
    private bool _isPlatnoscWybrana;

    public PlatnoscViewModel(Action<string> navigate)
    {
        _navigate = navigate;
    }

    partial void OnIsBlikChanged(bool value) => UpdatePlatnoscWybrana();
    partial void OnIsKartaChanged(bool value) => UpdatePlatnoscWybrana();
    partial void OnIsGotowkaChanged(bool value) => UpdatePlatnoscWybrana();

    private void UpdatePlatnoscWybrana()
    {
        IsPlatnoscWybrana = IsBlik || IsKarta || IsGotowka;
    }

    [RelayCommand]
    private void Zaplac()
    {
        if (!IsPlatnoscWybrana) return;

        // Zamknij aplikację
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    [RelayCommand]
    private void Anuluj()
    {
        IsBlik = false;
        IsKarta = false;
        IsGotowka = false;
        _navigate("Koszyk");
    }
}
