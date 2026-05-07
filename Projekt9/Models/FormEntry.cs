using System;

namespace Projekt9.Models;

/// <summary>
/// Reprezentacja jednego zapisanego formularza
/// "Wniosek o przeprowadzenie egzaminu komisyjnego".
///
/// Dokładnie 15 pól wniosku odpowiada polom z papierowego wzoru:
///  Strona 1 (sekcja studenta) — pola 1..10
///  Strona 2 (sekcja DECYZJA)  — pola 11..15
/// </summary>
public class FormEntry
{
    public int Id { get; set; }

    /// <summary>Nazwa formularza nadawana przez użytkownika przy zapisie.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Moment zapisu wpisu w bazie.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ----- 15 pól wniosku -----

    /// <summary>1. Data wniosku ("Poznań, dnia ......").</summary>
    public DateTime? RequestDate { get; set; }

    /// <summary>2. Numer albumu.</summary>
    public string AlbumNumber { get; set; } = string.Empty;

    /// <summary>3. Nazwisko i imię.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>4. Semestr, rok.</summary>
    public string SemesterYear { get; set; } = string.Empty;

    /// <summary>5. Kierunek i stopień studiów.</summary>
    public string FieldAndDegree { get; set; } = string.Empty;

    /// <summary>6. Przedmiot, z którego ma być przeprowadzony egzamin.</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>7. Punkty (ECTS) — trzymamy jako tekst, bo wzór pozwala na puste.</summary>
    public string Points { get; set; } = string.Empty;

    /// <summary>8. Prowadzący przedmiot.</summary>
    public string Lecturer { get; set; } = string.Empty;

    /// <summary>9. Uzasadnienie wniosku (wieloliniowe).</summary>
    public string Justification { get; set; } = string.Empty;

    /// <summary>10. Data i podpis studenta — przechowujemy datę.</summary>
    public DateTime? StudentSignDate { get; set; }

    /// <summary>10b. Treść podpisu studenta (np. odręczny podpis wpisany tekstowo).</summary>
    public string StudentSignature { get; set; } = string.Empty;

    /// <summary>11. Decyzja: true = "Wyrażam zgodę", false = "Nie wyrażam zgody".</summary>
    public bool DecisionApproved { get; set; }

    /// <summary>12. Skład komisji — pozycja 1.</summary>
    public string CommitteeMember1 { get; set; } = string.Empty;

    /// <summary>13. Skład komisji — pozycja 2.</summary>
    public string CommitteeMember2 { get; set; } = string.Empty;

    /// <summary>14. Skład komisji — pozycja 3.</summary>
    public string CommitteeMember3 { get; set; } = string.Empty;

    /// <summary>15. Data decyzji prodziekana ("Poznań, dnia ......").</summary>
    public DateTime? DecisionDate { get; set; }
}
