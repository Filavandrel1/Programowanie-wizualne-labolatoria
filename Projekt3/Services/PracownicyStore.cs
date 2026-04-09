using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Projekt3.Models;

namespace Projekt3.Services;

/// <summary>
/// Wspólne źródło danych dla pracowników - odpowiednik BindingSource + DataTable z WinForms.
/// Obie formy (okna) korzystają z tej samej instancji,
/// dzięki czemu dodanie pracownika w oknie 2 natychmiast pojawia się w oknie 1.
/// </summary>
public class PracownicyStore
{
    /// <summary>
    /// Główna kolekcja pracowników - odpowiednik DataTable.Rows.
    /// ObservableCollection automatycznie powiadamia DataGrid o zmianach (jak BindingSource).
    /// </summary>
    public ObservableCollection<Pracownik> Pracownicy { get; } = new();

    private int _nextId = 1;

    /// <summary>
    /// Dodaje nowego pracownika i przypisuje mu kolejne ID.
    /// Odpowiednik: dataTable.Rows.Add(...)
    /// </summary>
    public void Dodaj(Pracownik pracownik)
    {
        pracownik.Id = _nextId++;
        Pracownicy.Add(pracownik);
    }

    /// <summary>
    /// Usuwa pracownika z kolekcji.
    /// Odpowiednik: dataTable.Rows.Remove(...)
    /// </summary>
    public void Usun(Pracownik pracownik)
    {
        Pracownicy.Remove(pracownik);
    }

    /// <summary>
    /// Ładuje przykładowe dane - odpowiednik dataTable.Rows.Add(...) w konstruktorze Form1.
    /// </summary>
    public void ZaladujPrzykladoweDane()
    {
        Dodaj(new Pracownik { Imie = "Jan", Nazwisko = "Kowalski", Wiek = 35, Stanowisko = "Programista" });
        Dodaj(new Pracownik { Imie = "Anna", Nazwisko = "Nowak", Wiek = 42, Stanowisko = "Kierownik" });
        Dodaj(new Pracownik { Imie = "Piotr", Nazwisko = "Wiśniewski", Wiek = 28, Stanowisko = "Analityk" });
        Dodaj(new Pracownik { Imie = "Maria", Nazwisko = "Zielińska", Wiek = 31, Stanowisko = "Tester" });
        Dodaj(new Pracownik { Imie = "Tomasz", Nazwisko = "Wójcik", Wiek = 26, Stanowisko = "Stażysta" });
    }

    /// <summary>
    /// Eksportuje dane do pliku CSV - odpowiednik ExportToCSV(DataGridView, filePath).
    /// Tworzy nagłówek pliku CSV i dodaje dane z kolekcji pracowników,
    /// oddzielone przecinkami (jak w WinForms: string.Join(",", ...)).
    /// </summary>
    public void ExportToCSV(string filePath)
    {
        // Tworzenie nagłówka pliku CSV
        string csvContent = "Id,Imie,Nazwisko,Wiek,Stanowisko" + Environment.NewLine;

        // Dodawanie danych z kolekcji (odpowiednik iteracji po DataGridView.Rows)
        foreach (var p in Pracownicy)
        {
            // Dodaj kolejne wartości w wierszu, oddzielone przecinkami
            csvContent += string.Join(",", p.Id, p.Imie, p.Nazwisko, p.Wiek, p.Stanowisko)
                          + Environment.NewLine;
        }

        // Zapisanie zawartości do pliku CSV
        File.WriteAllText(filePath, csvContent, Encoding.UTF8);
    }

    /// <summary>
    /// Wczytuje dane z pliku CSV - odpowiednik LoadCSVToDataGridView(filePath).
    /// Odczytuje nagłówek, a następnie dodaje wiersze do kolekcji
    /// (jak w WinForms: DataTable + dataGridView.DataSource = dataTable).
    /// </summary>
    public void LoadCSVToDataGridView(string filePath)
    {
        // Sprawdź, czy plik istnieje
        if (!File.Exists(filePath))
            return;

        // Odczytaj zawartość pliku CSV
        string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

        if (lines.Length == 0)
            return;

        // Wyczyść bieżącą kolekcję (odpowiednik przypisania nowej DataTable)
        Pracownicy.Clear();
        _nextId = 1;

        // Dodanie kolumn na podstawie nagłówka - pomijamy nagłówek (lines[0])
        // string[] headers = lines[0].Split(',');

        // Dodawanie wierszy do kolekcji (odpowiednik dataTable.Rows.Add(values))
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length >= 5)
            {
                var pracownik = new Pracownik
                {
                    Imie = values[1],
                    Nazwisko = values[2],
                    Wiek = int.TryParse(values[3], out var wiek) ? wiek : 0,
                    Stanowisko = values[4]
                };
                Dodaj(pracownik); // Dodaj przypisuje ID automatycznie
            }
        }
    }

    /// <summary>
    /// Eksportuje dane pracowników do pliku XML.
    /// Transformuje obiekty z kolekcji na listę obiektów PracownikXml (DTO),
    /// a następnie serializuje je za pomocą XmlSerializer.
    /// </summary>
    public void ExportToXml(string filePath)
    {
        // Transformacja obiektów Pracownik → PracownikXml (klasa serializowalna)
        var listaDoSerializacji = Pracownicy.Select(p => new PracownikXml
        {
            Id = p.Id,
            Imie = p.Imie,
            Nazwisko = p.Nazwisko,
            Wiek = p.Wiek,
            Stanowisko = p.Stanowisko
        }).ToList();

        // Serializacja do pliku XML
        var serializer = new XmlSerializer(typeof(List<PracownikXml>));
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        serializer.Serialize(stream, listaDoSerializacji);
    }
}
