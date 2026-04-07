using System;
using System.Collections.Generic;
using System.IO;
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
    /// Obraca obraz o wybrany kąt (90, 180, 270) - przycisk "Rotate".
    /// Operuje na pikselach: odczytuje źródłowy obraz i tworzy nowy z obróconym układem.
    /// </summary>
    [RelayCommand]
    private void Obrot()
    {
        if (Obraz == null) return;

        var src = Obraz;
        int srcW = src.PixelSize.Width;
        int srcH = src.PixelSize.Height;

        // Określ wymiary docelowe w zależności od kąta
        int dstW, dstH;
        if (WybranyKat == 180)
        {
            dstW = srcW;
            dstH = srcH;
        }
        else // 90 lub 270 - zamiana szerokości i wysokości
        {
            dstW = srcH;
            dstH = srcW;
        }

        // Odczytaj piksele źródłowe do tablicy bajtów (BGRA, 4 bajty na piksel)
        int srcStride = srcW * 4;
        int srcBufferSize = srcStride * srcH;
        byte[] srcPixels = new byte[srcBufferSize];

        var handle = GCHandle.Alloc(srcPixels, GCHandleType.Pinned);
        try
        {
            src.CopyPixels(new PixelRect(0, 0, srcW, srcH), handle.AddrOfPinnedObject(), srcBufferSize, srcStride);
        }
        finally
        {
            handle.Free();
        }

        // Przygotuj docelową tablicę pikseli
        int dstStride = dstW * 4;
        byte[] dstPixels = new byte[dstStride * dstH];

        // Obrót pikseli
        for (int y = 0; y < srcH; y++)
        {
            for (int x = 0; x < srcW; x++)
            {
                int newX, newY;
                switch (WybranyKat)
                {
                    case 90:
                        // Obrót o 90° w prawo: (x,y) -> (srcH-1-y, x)
                        newX = srcH - 1 - y;
                        newY = x;
                        break;
                    case 180:
                        // Obrót o 180°: (x,y) -> (srcW-1-x, srcH-1-y)
                        newX = srcW - 1 - x;
                        newY = srcH - 1 - y;
                        break;
                    case 270:
                        // Obrót o 270° w prawo (90° w lewo): (x,y) -> (y, srcW-1-x)
                        newX = y;
                        newY = srcW - 1 - x;
                        break;
                    default:
                        newX = x;
                        newY = y;
                        break;
                }

                // Kopiuj 4 bajty piksela (BGRA)
                int srcOffset = y * srcStride + x * 4;
                int dstOffset = newY * dstStride + newX * 4;

                dstPixels[dstOffset + 0] = srcPixels[srcOffset + 0]; // B
                dstPixels[dstOffset + 1] = srcPixels[srcOffset + 1]; // G
                dstPixels[dstOffset + 2] = srcPixels[srcOffset + 2]; // R
                dstPixels[dstOffset + 3] = srcPixels[srcOffset + 3]; // A
            }
        }

        // Utwórz nową bitmapę z obróconych pikseli
        var dst = new WriteableBitmap(
            new PixelSize(dstW, dstH),
            src.Dpi,
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Premul);

        using (var dstLock = dst.Lock())
        {
            unsafe
            {
                byte* dstPtr = (byte*)dstLock.Address;
                int realDstStride = dstLock.RowBytes;

                for (int row = 0; row < dstH; row++)
                {
                    for (int col = 0; col < dstW; col++)
                    {
                        int arrOffset = row * dstStride + col * 4;
                        int lockOffset = row * realDstStride + col * 4;

                        dstPtr[lockOffset + 0] = dstPixels[arrOffset + 0];
                        dstPtr[lockOffset + 1] = dstPixels[arrOffset + 1];
                        dstPtr[lockOffset + 2] = dstPixels[arrOffset + 2];
                        dstPtr[lockOffset + 3] = dstPixels[arrOffset + 3];
                    }
                }
            }
        }

        Obraz = dst;
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
