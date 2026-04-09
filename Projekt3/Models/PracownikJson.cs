using System.Text.Json.Serialization;

namespace Projekt3.Models;

/// <summary>
/// Klasa DTO (Data Transfer Object) służąca do serializacji JSON.
/// Zawiera te same pola co Pracownik, ale bez dziedziczenia po ObservableObject,
/// dzięki czemu JsonSerializer może ją bezproblemowo serializować.
/// </summary>
public class PracownikJson
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("imie")]
    public string Imie { get; set; } = string.Empty;

    [JsonPropertyName("nazwisko")]
    public string Nazwisko { get; set; } = string.Empty;

    [JsonPropertyName("wiek")]
    public int Wiek { get; set; }

    [JsonPropertyName("stanowisko")]
    public string Stanowisko { get; set; } = string.Empty;
}
