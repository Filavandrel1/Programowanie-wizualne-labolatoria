using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Projekt4.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Wybrany kąt obrotu (90, 180, 270) - powiązany z RadioButton.
    /// </summary>
    [ObservableProperty]
    private int _wybranyKat = 90;

    /// <summary>
    /// Obraz wyświetlany po prawej stronie okna.
    /// </summary>
    [ObservableProperty]
    private Bitmap? _obraz;

    /// <summary>
    /// Oryginalna bitmapa załadowana z pliku (potrzebna do operacji).
    /// </summary>
    private Bitmap? _oryginalnyObraz;

    /// <summary>
    /// Pomocnicza metoda zwracająca główne okno aplikacji.
    /// </summary>
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    /// <summary>
    /// Wczytuje obraz z pliku - przycisk "Load".
    /// </summary>
    [RelayCommand]
    private async Task Zaladuj()
    {
        var window = GetMainWindow();
        if (window == null) return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Wybierz obraz do wczytania",
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Obrazy") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" } },
                new("Wszystkie pliki") { Patterns = new[] { "*.*" } }
            },
            AllowMultiple = false
        });

        if (files.Count > 0)
        {
            await using var stream = await files[0].OpenReadAsync();
            _oryginalnyObraz = new Bitmap(stream);
            Obraz = _oryginalnyObraz;
        }
    }

    /// <summary>
    /// Obraca obraz o wybrany kąt - przycisk "Rotate".
    /// </summary>
    [RelayCommand]
    private void Obrot()
    {
        // Funkcja zostanie zaimplementowana w następnych zapytaniach
    }

    /// <summary>
    /// Odwraca kolory obrazu - przycisk "Invert Colors".
    /// </summary>
    [RelayCommand]
    private void OdwrocKolory()
    {
        // Funkcja zostanie zaimplementowana w następnych zapytaniach
    }

    /// <summary>
    /// Odwraca obraz do góry nogami - przycisk "Upside Down".
    /// </summary>
    [RelayCommand]
    private void DoGoryNogami()
    {
        // Funkcja zostanie zaimplementowana w następnych zapytaniach
    }

    /// <summary>
    /// Pozostawia tylko kanał zielony - przycisk "Only Green".
    /// </summary>
    [RelayCommand]
    private void TylkoZielony()
    {
        // Funkcja zostanie zaimplementowana w następnych zapytaniach
    }
}
