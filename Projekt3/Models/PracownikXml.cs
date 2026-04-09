using System.Xml.Serialization;

namespace Projekt3.Models;

/// <summary>
/// Klasa DTO (Data Transfer Object) służąca do serializacji XML.
/// Zawiera te same pola co Pracownik, ale bez dziedziczenia po ObservableObject,
/// dzięki czemu XmlSerializer może ją bezproblemowo serializować.
/// </summary>
[XmlRoot("Pracownik")]
public class PracownikXml
{
    [XmlElement("Id")]
    public int Id { get; set; }

    [XmlElement("Imie")]
    public string Imie { get; set; } = string.Empty;

    [XmlElement("Nazwisko")]
    public string Nazwisko { get; set; } = string.Empty;

    [XmlElement("Wiek")]
    public int Wiek { get; set; }

    [XmlElement("Stanowisko")]
    public string Stanowisko { get; set; } = string.Empty;
}
