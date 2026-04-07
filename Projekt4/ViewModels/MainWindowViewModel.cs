using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    /// Pozostawia tylko piksele zielone - resztę zamienia na czarne.
    /// Piksel uznajemy za "zielony" gdy kanał G jest dominujący
    /// (G > R i G > B).
    /// Format pikseli BGRA: [0]=B, [1]=G, [2]=R, [3]=A.
    /// </summary>
    [RelayCommand]
    private void TylkoZielony()
    {
        if (Obraz == null) return;

        var src = Obraz;
        int w = src.PixelSize.Width;
        int h = src.PixelSize.Height;
        int stride = w * 4;
        int bufferSize = stride * h;
        byte[] pixels = new byte[bufferSize];

        // Odczytaj piksele źródłowe
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            src.CopyPixels(new PixelRect(0, 0, w, h), handle.AddrOfPinnedObject(), bufferSize, stride);
        }
        finally
        {
            handle.Free();
        }

        // Przetwórz piksele - zostaw zielone, resztę zamień na czarne
        for (int i = 0; i < bufferSize; i += 4)
        {
            byte b = pixels[i + 0]; // Blue
            byte g = pixels[i + 1]; // Green
            byte r = pixels[i + 2]; // Red
            // pixels[i + 3] to Alpha - nie zmieniamy

            // Jeśli piksel NIE jest zielony (G nie dominuje), zamień na czarny
            if (!(g > r && g > b))
            {
                pixels[i + 0] = 0; // B = 0
                pixels[i + 1] = 0; // G = 0
                pixels[i + 2] = 0; // R = 0
                // Alpha pozostaje bez zmian
            }
        }

        // Utwórz nową bitmapę z przetworzonymi pikselami
        var dst = new WriteableBitmap(
            new PixelSize(w, h),
            src.Dpi,
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Premul);

        using (var dstLock = dst.Lock())
        {
            unsafe
            {
                byte* dstPtr = (byte*)dstLock.Address;
                int dstStride = dstLock.RowBytes;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int arrOffset = y * stride + x * 4;
                        int lockOffset = y * dstStride + x * 4;

                        dstPtr[lockOffset + 0] = pixels[arrOffset + 0];
                        dstPtr[lockOffset + 1] = pixels[arrOffset + 1];
                        dstPtr[lockOffset + 2] = pixels[arrOffset + 2];
                        dstPtr[lockOffset + 3] = pixels[arrOffset + 3];
                    }
                }
            }
        }

        Obraz = dst;
    }
}
