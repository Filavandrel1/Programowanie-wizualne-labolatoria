using System;

namespace Projekt11.Models;

/// <summary>
/// Typ próbki biologicznej.
/// </summary>
public enum SampleType
{
    DNA,
    RNA,
    Bialko,
    Inny
}

/// <summary>
/// Próbka biologiczna przechowywana w bazie.
/// </summary>
public class Sample
{
    /// <summary>Wewnętrzny klucz w bazie (autoincrement).</summary>
    public int RowId { get; set; }

    /// <summary>Unikalny identyfikator próbki nadawany przez użytkownika (np. "DNA-001").</summary>
    public string SampleId { get; set; } = string.Empty;

    /// <summary>Nazwa próbki.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Typ próbki (DNA/RNA/Białko/Inny).</summary>
    public SampleType Type { get; set; } = SampleType.DNA;

    /// <summary>Data pobrania próbki.</summary>
    public DateTime CollectionDate { get; set; } = DateTime.Today;

    /// <summary>Opis / uwagi.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Dane do zakodowania w kodzie QR.</summary>
    public string ToQrPayload() =>
        $"ID: {SampleId}\nNazwa: {Name}\nTyp: {Type}\nData: {CollectionDate:yyyy-MM-dd}\nOpis: {Description}";

    public override string ToString() => $"{SampleId} — {Name}";
}
