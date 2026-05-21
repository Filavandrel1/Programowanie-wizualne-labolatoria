using System.IO;
using Avalonia.Media.Imaging;
using QRCoder;

namespace Projekt11.Services;

/// <summary>
/// Generuje obrazy kodów QR przy użyciu biblioteki QRCoder.
/// </summary>
public static class QrService
{
    /// <summary>Zwraca dane PNG kodu QR dla zadanego tekstu.</summary>
    public static byte[] GeneratePng(string payload, int pixelsPerModule = 20)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        return png.GetGraphic(pixelsPerModule);
    }

    /// <summary>Tworzy <see cref="Bitmap"/> Avalonii z danych QR.</summary>
    public static Bitmap GenerateBitmap(string payload, int pixelsPerModule = 20)
    {
        var bytes = GeneratePng(payload, pixelsPerModule);
        using var ms = new MemoryStream(bytes);
        return new Bitmap(ms);
    }

    /// <summary>Zapisuje kod QR do pliku PNG na dysku.</summary>
    public static void SavePng(string payload, string filePath, int pixelsPerModule = 20)
    {
        var bytes = GeneratePng(payload, pixelsPerModule);
        File.WriteAllBytes(filePath, bytes);
    }
}
