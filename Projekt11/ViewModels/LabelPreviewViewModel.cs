using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Projekt11.Models;
using Projekt11.Services;

namespace Projekt11.ViewModels;

/// <summary>
/// Podgląd etykiety z kodem QR oraz akcje eksportu do PNG i drukowania.
/// </summary>
public partial class LabelPreviewViewModel : ViewModelBase
{
    public event Action? BackRequested;

    public Sample Sample { get; }

    [ObservableProperty] private Bitmap? _qrImage;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public string Title => $"Etykieta: {Sample.SampleId} — {Sample.Name}";

    public LabelPreviewViewModel(Sample sample)
    {
        Sample = sample;
        Regenerate();
    }

    private void Regenerate()
    {
        try
        {
            QrImage = QrService.GenerateBitmap(Sample.ToQrPayload(), pixelsPerModule: 16);
        }
        catch (Exception ex)
        {
            StatusMessage = "❌ Błąd generowania QR: " + ex.Message;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task ExportPng()
    {
        try
        {
            var top = GetTopLevel();
            if (top is null)
            {
                StatusMessage = "❌ Brak okna do otwarcia dialogu zapisu.";
                return;
            }

            var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Zapisz kod QR jako PNG",
                SuggestedFileName = $"{Sample.SampleId}.png",
                DefaultExtension = "png",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Obraz PNG") { Patterns = new[] { "*.png" } }
                }
            });

            if (file is null) return;

            var path = file.TryGetLocalPath();
            if (string.IsNullOrEmpty(path))
            {
                StatusMessage = "❌ Nie można uzyskać ścieżki pliku.";
                return;
            }

            QrService.SavePng(Sample.ToQrPayload(), path, pixelsPerModule: 20);
            StatusMessage = $"✅ Zapisano PNG: {path}";
        }
        catch (Exception ex)
        {
            StatusMessage = "❌ Błąd eksportu: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Print()
    {
        try
        {
            var tmp = Path.Combine(Path.GetTempPath(), $"qr_{Sample.SampleId}_{Guid.NewGuid():N}.png");
            QrService.SavePng(Sample.ToQrPayload(), tmp, pixelsPerModule: 20);
            PrintService.PrintImageFile(tmp);
            StatusMessage = $"🖨️  Wysłano do drukarki: {tmp}";
        }
        catch (Exception ex)
        {
            StatusMessage = "❌ Błąd drukowania: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Back() => BackRequested?.Invoke();

    private static TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }
}
