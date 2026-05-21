using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Projekt11.Services;

/// <summary>
/// Pomocnik drukowania pliku obrazu poprzez polecenia systemowe.
/// macOS/Linux: <c>lpr</c>, Windows: <c>mspaint /pt</c> jako prosty fallback.
/// </summary>
public static class PrintService
{
    public static void PrintImageFile(string filePath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "lpr",
                ArgumentList = { filePath },
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Skorzystaj z domyślnego shellowego "print" – działa dla wielu zarejestrowanych typów plików.
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                Verb = "print",
                UseShellExecute = true,
                CreateNoWindow = true,
            });
            return;
        }

        throw new PlatformNotSupportedException("Drukowanie nie jest wspierane na tej platformie.");
    }
}
